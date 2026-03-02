using Microsoft.Extensions.Options;
using SteamRater.Contracts;
using SteamRater.Models;
using System.Text.Json;

namespace SteamRater.Services
{
    public class SteamStoreService : ISteamStoreService
    {

        private readonly HttpClient _httpClient;
        private readonly IOptions<ApiSettings> _apiSettings;

        // API to query steam family share instead of just steam profile owned games (my id: 76561198308578397, Emily: 76561199043218536) 
        public SteamStoreService(IOptions<ApiSettings> apiSettings, HttpClient httpClient)
        {
            _apiSettings = apiSettings ?? throw new ArgumentNullException(nameof(apiSettings));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            var baseUrl = _apiSettings.Value.BaseUrl2 ?? throw new InvalidOperationException("BaseUrl2 is not configured in appsettings.json");

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<AppReview?> GetAppReviews(int appId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"appreviews/{appId}?json=1");

                response.EnsureSuccessStatusCode();

                // Deserialize JSON into ApiResponse object
                AppReviews? data = await response.Content.ReadFromJsonAsync<AppReviews>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (data is null)
                    return null;

                AppReview? appReview = data.query_summary;

                return appReview;
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
