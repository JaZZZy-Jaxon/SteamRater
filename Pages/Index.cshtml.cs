using Microsoft.AspNetCore.Mvc.RazorPages;
using SteamRater.Models;
using SteamRater.Contracts;
using Microsoft.Extensions.Options;

namespace SteamRater.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ISteamService _steamService;
        private readonly IOptions<ApiSettings> _apiSettings;

        public IndexModel(ISteamService steamService, IOptions<ApiSettings> apiSettings)
        {
            _steamService = steamService ?? throw new ArgumentNullException(nameof(steamService));
            _apiSettings = apiSettings ?? throw new ArgumentNullException(nameof(apiSettings));
        }

        public Player? player;
        public List<Game>? games;

        public async Task OnGetAsync()
        {
            string apiKey = _apiSettings.Value.ApiKey ?? throw new InvalidOperationException("ApiKey is not configured in appsettings.json");

            // TODO: Store API key in string variable
            player = await _steamService.GetPlayerSummary("76561198308578397");
            games = await _steamService.GetOwnedGames("76561198308578397");

            if (games is null)
                return;

            foreach (Game game in games)
            {
                // Add with playtime disconnected for real total time
                game.playtime_forever += game.playtime_disconnected;
                game.playtime_forever = Math.Round(game.playtime_forever / 60, 1);
            }

            games = games.OrderByDescending(t => t.playtime_forever).ToList();

            //restrict list to top 5
            if (games.Count > 5)
                games = games.Take(5).ToList();
            }
    }
}