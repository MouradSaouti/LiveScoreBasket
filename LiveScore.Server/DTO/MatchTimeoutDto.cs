namespace LiveScore.Server.DTO
{
    public class MatchTimeoutDto
    {
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public TimeSpan Time { get; set; }
    }
}
