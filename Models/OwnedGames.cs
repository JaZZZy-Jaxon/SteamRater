namespace SteamRater.Models
{

        public class OwnedGames
        {
            public OwnedGamesResponse? Response { get; set; }
        }

        public class OwnedGamesResponse
        {
            public List<Game>? Games { get; set; }
        }

        public class Game
        {
            public int appid { get; set; }
            public string name { get; set; } = string.Empty;
            public Double playtime_forever { get; set; }
            public Double playtime_disconnected { get; set; }

        }

}
