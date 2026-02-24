using SteamRater.Models;

namespace SteamRater.Contracts
{
    public interface ISteamService
    {
        public Task<Root?> GetAPIResponse(string endpoint);
    }
}
