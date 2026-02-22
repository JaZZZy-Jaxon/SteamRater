// Models/Post.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SteamRater.Models
{
    public class SteamID
    {
        public ResponseData Response { get; set; }
    }

    public class ResponseData
    {
        public List<Player> Players { get; set; }
    }

    public class Player
    {
        public string steamid { get; set; } = string.Empty;
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

        public Player Player1 = new Player();

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new System.Uri("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamids=76561198308578397");

                // Call API and deserialize JSON into List<Post>
                SteamID data = await client.GetFromJsonAsync<SteamID>("");

                if (data?.Response?.Players != null)
                {
                    foreach (var player in data.Response.Players)
                    {
                        Player1.steamid = player.steamid;
                        Console.WriteLine($"Player ID: {player.steamid}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle network errors
                ModelState.AddModelError(string.Empty, $"Error calling API: {ex.Message}");
            }
        }
    }
}
