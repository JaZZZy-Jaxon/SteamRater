using Microsoft.Extensions.Options;
using SteamRater.Contracts;
using SteamRater.Models;
using System.Net;
using System.Text.Json;

namespace SteamRater.Services
{
    public class SteamService : ISteamService
    {

        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;

        // API to query steam family share instead of just steam profile owned games (my id: 76561198308578397, Emily: 76561199043218536) 
        public SteamService(IOptions<ApiSettings> apiSettings, HttpClient httpClient)
        {
            _apiSettings = apiSettings ?? throw new ArgumentNullException(nameof(apiSettings));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            var baseUrl = _apiSettings.Value.BaseUrl ?? throw new InvalidOperationException("BaseUrl is not configured in appsettings.json");

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<Player?> GetPlayerSummary(string steamId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"ISteamUser/GetPlayerSummaries/v0002/?key={_apiSettings.Value.ApiKey}&steamids={steamId}");

                response.EnsureSuccessStatusCode();

                // Deserialize JSON into ApiResponse object
                PlayerSummary? data = await response.Content.ReadFromJsonAsync<PlayerSummary>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data is null || data.Response is null || data.Response.Players is null)
                    return null;

                Player player = data.Response.Players.Single();

                return player;
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

        public async Task<List<Game>?> GetOwnedGames(string steamId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"IPlayerService/GetOwnedGames/v1/?key={_apiSettings.Value.ApiKey}&steamid={steamId}&include_appinfo=true&include_played_free_games=true&appids_filter=");

                response.EnsureSuccessStatusCode();

                // Deserialize JSON into ApiResponse object
                OwnedGames? data = await response.Content.ReadFromJsonAsync<OwnedGames>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data is null || data.Response is null || data.Response.Games is null)
                    return null;

                List<Game> games = data.Response.Games;

                return games;
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