using SteamRater.Models;

namespace SteamRater.Contracts
{
    public interface ISteamStoreService
    {
        public Task<AppReview?> GetAppReviews(int appId);
    }
}
