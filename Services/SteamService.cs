using SteamRater.Contracts;
using System.Text.Json;
using SteamRater.Models;

namespace SteamRater.Services
{
    public class SteamService : ISteamService
    {

        private readonly HttpClient _httpClient;
        private readonly string baseAddress = "https://api.steampowered.com/";

        // API to query steam family share instead of just steam profile owned games (my id: 76561198308578397, Emily: 76561199043218536)
        public SteamService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<Root?> GetPlayers()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("ISteamUser/GetPlayerSummaries/v0002/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamids=76561198308578397&format=json");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                // Deserialize JSON into ApiResponse object
                Root data = await response.Content.ReadFromJsonAsync<Root>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return data;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Request timed out.");
                return null;
            }
        }

        public async Task<Root?> GetGames()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("IPlayerService/GetOwnedGames/v1/?key=143AFF0EF09A8CF4A63A88A102C79E9E&steamid=76561198308578397&include_appinfo=true&include_played_free_games=true&appids_filter=&format=json");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                // Deserialize JSON into ApiResponse object
                Root data = await response.Content.ReadFromJsonAsync<Root>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return data;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Request timed out.");
                return null;
            }
        }

    }
}
