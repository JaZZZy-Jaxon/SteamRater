using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SteamRater.Contracts;
using SteamRater.Models;

namespace SteamRater.Controllers
{
    //[Route("")]
    public class IndexController : Controller
    {
        protected readonly ILogger _logger;
        protected readonly ISteamService _steamService;
        protected readonly ISteamStoreService _steamStoreService;
        protected readonly IOptions<ApiSettings> _apiSettings;

        public IndexController(ILogger<IndexController> logger, ISteamService steamService, ISteamStoreService steamStoreService, IOptions<ApiSettings> apiSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steamService = steamService ?? throw new ArgumentNullException(nameof(steamService));
            _steamStoreService = steamStoreService ?? throw new ArgumentNullException(nameof(steamStoreService));
            _apiSettings = apiSettings ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new IndexViewModel
                {
                    apiKey = _apiSettings.Value.ApiKey ?? throw new InvalidOperationException("ApiKey is not configured in appsettings.json"),
                    player = await _steamService.GetPlayerSummary("76561198308578397"),
                    games = await _steamService.GetOwnedGames("76561198308578397"),
                    appReviews = new List<AppReview?>(),
                    gameReviews = new List<Double>()
                };

                var vGames = viewModel.games;
                var vAppReviews = viewModel.appReviews;
                var vGameReviews = viewModel.gameReviews;

                if (vGames is null)
                    throw new KeyNotFoundException($"Games not found");

                vGames = vGames.OrderByDescending(t => t.playtime_forever).ToList();

                //restrict list to top 5
                if (vGames.Count > 5)
                    vGames = vGames.Take(5).ToList();

                foreach (Game game in vGames)
                {
                    // Add with playtime disconnected for real total time
                    game.playtime_forever += game.playtime_disconnected;
                    game.playtime_forever = Math.Round(game.playtime_forever / 60, 1);

                    vAppReviews.Add(await _steamStoreService.GetAppReviews(game.appid));
                }

                vGameReviews = new List<Double>();

                foreach (AppReview? app in vAppReviews)
                {
                    if (app is null)
                        continue;

                    // Check why review score doen't line up 1-1 to steam
                    Double score = Math.Round((100.0 / app.total_reviews) * app.total_positive, 2);
                    vGameReviews.Add(score);
                }

                viewModel.games = vGames;
                viewModel.appReviews = vAppReviews;
                viewModel.gameReviews = vGameReviews;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Something went wrong: {ex.Message}");
            }

        }
    }
}