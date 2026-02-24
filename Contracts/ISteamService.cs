using SteamRater.Models;

namespace SteamRater.Contracts
{
    public interface ISteamService
    {
        public Task<Root?> GetPlayers();

        public Task<Root?> GetGames();
    }
}
