using Microsoft.AspNetCore.Mvc.RazorPages;
using SteamRater.Services;
using SteamRater.Models;

namespace SteamRater.Pages
{
    public class IndexModel : PageModel
    {
        public SteamService steamService = new SteamService();
        public Root PlayerRequest = new Root();
        public Root GamesRequest = new Root();

        public async Task OnGetAsync()
        {
            PlayerRequest = await steamService.GetAPIResponse("ISteamUser/GetPlayerSummaries/v0002/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamids=76561198308578397&format=json");
            GamesRequest = await steamService.GetAPIResponse("IPlayerService/GetOwnedGames/v1/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamid=76561198308578397&include_appinfo=true&include_played_free_games=true&appids_filter=&format=json");

            foreach (Game game in GamesRequest.Response.Games)
            {
                // Add with playtime disconnected for real total time
                game.playtime_forever += game.playtime_disconnected;
                game.playtime_forever = Math.Round(game.playtime_forever / 60, 1);
            }

            // sort by playtime (Use Linq)
            GamesRequest.Response.Games.Sort((g1, g2) => g2.playtime_forever.CompareTo(g1.playtime_forever));

            //restrict list to top 5
            while (GamesRequest.Response.Games.Count > 5)
            {
                GamesRequest.Response.Games.RemoveAt(5);
            }
        }
    }
}
