// Models/Post.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SteamRater.Models
{
    public class Root
    {
        public ResponseData Response { get; set; }
    }

    public class ResponseData
    {
        public List<Player> Players { get; set; }
        public List<Game> Games { get; set; }
    }

    public class Player
    {
        public string personaname { get; set; } = string.Empty;
    }

    public class Game
    {
        public int appid { get; set; }
        public string name { get; set; } = string.Empty;
        public Double playtime_forever { get; set; }
        public Double playtime_disconnected { get; set; }

    }
}

namespace SteamRater.Pages
{
    using SteamRater.Models;
    public class SteamIDModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SteamIDModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public ResponseData GamesToken = new ResponseData();
        public ResponseData PlayerToken = new ResponseData();

        public async Task OnGetAsync()
        {
            try
            {
                // API to query steam family share instead of just steam profile owned games (my id: 76561198308578397, Emily: 76561199043218536)
                var client = _httpClientFactory.CreateClient();
                // Keep base address shortened and use jsonasync to add pages and params
                client.BaseAddress = new System.Uri("https://api.steampowered.com/");

                Root data1 = await client.GetFromJsonAsync<Root>("ISteamUser/GetPlayerSummaries/v0002/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamids=76561198308578397&format=json");

                if (data1?.Response?.Players != null)
                {
                    PlayerToken.Players = data1.Response.Players;
                    Console.WriteLine(PlayerToken.Players[0].personaname);
                }

                // Call API and deserialize JSON into List<Post>
                Root data2 = await client.GetFromJsonAsync<Root>("IPlayerService/GetOwnedGames/v1/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamid=76561198308578397&include_appinfo=true&include_played_free_games=true&appids_filter=&format=json");

                if (data2?.Response?.Games.Count != 0)
                {
                    GamesToken.Games = data2.Response.Games;
                }

                foreach (Game game in GamesToken.Games)
                {
                    // Add with playtime disconnected for real total time
                    game.playtime_forever += game.playtime_disconnected;
                    game.playtime_forever = Math.Round(game.playtime_forever / 60, 1);
                }

                // sort by playtime (Use Linq)
                GamesToken.Games.Sort((g1, g2) => g2.playtime_forever.CompareTo(g1.playtime_forever));

                Console.WriteLine(GamesToken.Games.Count);

                //restrict list to top 5
                while (GamesToken.Games.Count > 5)
                {
                    Console.WriteLine(GamesToken.Games[5].name);
                    GamesToken.Games.RemoveAt(5);                    
                }

                Console.WriteLine(GamesToken.Games.Count);

                /*
                if (Games.Games.Count > 5)
                {
                    Games.Games = Games.Games.GetRange(0, 5);
                }
                */
            }
            catch (HttpRequestException ex)
            {
                // Handle network errors
                ModelState.AddModelError(string.Empty, $"Error calling API: {ex.Message}");
            }
        }
    }
}
