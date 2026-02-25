using Microsoft.AspNetCore.Mvc.RazorPages;
using SteamRater.Models;
using SteamRater.Contracts;

namespace SteamRater.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ISteamService _steamService;

        public IndexModel(ISteamService steamService)
        {
            _steamService = steamService ?? throw new ArgumentNullException(nameof(steamService));
        }

        public Player player;
        public List<Game> games;

        public async Task OnGetAsync()
        {
            // TODO: Store API key in string variable
            Root PlayerRequest = await _steamService.GetAPIResponse("ISteamUser/GetPlayerSummaries/v0002/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamids=76561198308578397&format=json");
            Root GamesRequest = await _steamService.GetAPIResponse("IPlayerService/GetOwnedGames/v1/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamid=76561198308578397&include_appinfo=true&include_played_free_games=true&appids_filter=&format=json");

            player = PlayerRequest.Response.Players[0];
            games = GamesRequest.Response.Games;

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
