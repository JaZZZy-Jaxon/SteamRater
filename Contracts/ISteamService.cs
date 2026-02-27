using SteamRater.Models;

namespace SteamRater.Contracts
{
    public interface ISteamService
    {
        public Task<Player?> GetPlayerSummary(string steamId);
        public Task<List<Game>?> GetOwnedGames(string steamId);
    }
}