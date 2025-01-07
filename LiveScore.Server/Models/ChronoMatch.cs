namespace LiveScore.Server.Models
{
    public class ChronoMatch
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        public string TempsRestant { get; set; } = "00:00:00"; // Initialisé en string

        public string Etat { get; set; } = "Stopped";
    }


}
