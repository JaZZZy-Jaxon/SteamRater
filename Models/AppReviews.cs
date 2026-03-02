namespace SteamRater.Models
{

    public class AppReviews 
    {
        public AppReview? query_summary { get; set; }
    }

    public class AppReview
    {
        public int total_positive { get; set; }
        public int total_reviews { get; set; }
    }

}
