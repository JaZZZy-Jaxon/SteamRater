namespace SteamRater.Models
{

        public class PlayerSummary
        {
            public PlayerSummaryResponse? Response { get; set; }
        }

        public class PlayerSummaryResponse
        {
            public List<Player>? Players { get; set; }
        }

        public class Player
        {
            public string personaname { get; set; } = string.Empty;
        }

}
