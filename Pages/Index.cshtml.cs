using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SteamRater.Services;
using SteamRater.Models;

namespace SteamRater.Pages
{
    public class IndexModel : PageModel
    {
        public SteamService steamService = new SteamService();
        public Root Player = new Root();
        public Root Games = new Root();

        public async Task OnGetAsync()
        {
            Player = await steamService.GetPlayers();
            Games = await steamService.GetGames();

            foreach (Game game in Games.Response.Games)
            {
                // Add with playtime disconnected for real total time
                game.playtime_forever += game.playtime_disconnected;
                game.playtime_forever = Math.Round(game.playtime_forever / 60, 1);
            }

            // sort by playtime (Use Linq)
            Games.Response.Games.Sort((g1, g2) => g2.playtime_forever.CompareTo(g1.playtime_forever));

            Console.WriteLine(Games.Response.Games.Count);

            //restrict list to top 5
            while (Games.Response.Games.Count > 5)
            {
                Console.WriteLine(Games.Response.Games[5].name);
                Games.Response.Games.RemoveAt(5);
            }

            Console.WriteLine(Games.Response.Games.Count);
        }
    }
}
