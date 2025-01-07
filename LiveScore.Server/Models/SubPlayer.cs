namespace LiveScore.Server.Models
{
    public class SubPlayer
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int PlayerOutId { get; set; }
        public Joueur PlayerOut { get; set; } = null!;
        public int PlayerInId { get; set; }
        public Joueur PlayerIn { get; set; } = null!;
        public TimeSpan Time { get; set; }
    }
}
