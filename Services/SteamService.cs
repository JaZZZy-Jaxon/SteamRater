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

        public async Task<Root?> GetAPIResponse(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

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
