namespace LiveScore.Server.DTO
{
    public class ScoreUpdateDto
    {
        public int MatchId { get; set; }
        public int ScoreDomicile { get; set; }
        public int ScoreExterieur { get; set; }
    }
}
