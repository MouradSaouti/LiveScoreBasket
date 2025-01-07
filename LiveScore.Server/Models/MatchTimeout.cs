namespace LiveScore.Server.Models
{
    public class MatchTimeout
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int TeamId { get; set; }
        public TimeSpan Time { get; set; }
    }
}
