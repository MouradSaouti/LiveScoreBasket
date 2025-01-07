namespace LiveScore.Server.DTO
{
    public class ChronoUpdateDto
    {
        public string TempsRestant { get; set; } // Représente TimeSpan en ticks
        public int QuartTempsActuel { get; set; }
        public string Etat { get; set; } = "Stopped";
    }
}
