namespace LiveScore.Server.DTO
{
    public class SubPlayerDto
    {
        public int MatchId { get; set; }
        public int PlayerOutId { get; set; }
        public int PlayerInId { get; set; }
        public TimeSpan Time { get; set; }
    }
}
