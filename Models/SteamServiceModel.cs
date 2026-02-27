namespace SteamRater.Models
{
   
    public class Root
    {
        public ResponseData Response { get; set; }
    }

    public class ResponseData
    {
        public List<Player> Players { get; set; }
        public List<Game> Games { get; set; }
    }

    public class Player
    {
        public string personaname { get; set; } = string.Empty;
    }

    public class Game
    {
        public int appid { get; set; }
        public string name { get; set; } = string.Empty;
        public Double playtime_forever { get; set; }
        public Double playtime_disconnected { get; set; }

    }
    
}
