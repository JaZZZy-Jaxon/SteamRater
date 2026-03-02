using Microsoft.AspNetCore.Mvc.RazorPages;
using SteamRater.Models;
using SteamRater.Contracts;
using Microsoft.Extensions.Options;

namespace SteamRater.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ISteamService _steamService;
        private readonly ISteamStoreService _steamStoreService;
        private readonly IOptions<ApiSettings> _apiSettings;

        public IndexModel(ISteamService steamService, ISteamStoreService steamStoreService, IOptions<ApiSettings> apiSettings)
        {
            _steamService = steamService ?? throw new ArgumentNullException(nameof(steamService));
            _steamStoreService = steamStoreService ?? throw new ArgumentNullException(nameof(steamStoreService));
            _apiSettings = apiSettings ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        public Player? player;
        public List<Game>? games;
        private List<AppReview?>? appReviews;
        public List<Double>? gameReviews;

        public async Task OnGetAsync()
        {
            string apiKey = _apiSettings.Value.ApiKey ?? throw new InvalidOperationException("ApiKey is not configured in appsettings.json");

            player = await _steamService.GetPlayerSummary("76561198308578397");
            games = await _steamService.GetOwnedGames("76561198308578397");
            appReviews = new List<AppReview?>();

            if (games is null)
                return;

            games = games.OrderByDescending(t => t.playtime_forever).ToList();

            //restrict list to top 5
            if (games.Count > 5)
                games = games.Take(5).ToList();

            foreach (Game game in games)
            {
                // Add with playtime disconnected for real total time
                game.playtime_forever += game.playtime_disconnected;
                game.playtime_forever = Math.Round(game.playtime_forever / 60, 1);
                    
                appReviews.Add(await _steamStoreService.GetAppReviews(game.appid));
            }

            gameReviews = new List<Double>();

            foreach (AppReview? app in appReviews)
            {
                if (app is null)
                    continue;

                // Check why review score doen't line up 1-1 to steam
                Double score = Math.Round((100.0 / app.total_reviews) * app.total_positive, 2);
                gameReviews.Add(score);
            }
        }
    }
}