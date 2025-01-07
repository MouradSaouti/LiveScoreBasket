using System.Text.Json.Serialization;

namespace LiveScore.Server.Models
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime DateMatch { get; set; }
        public int EquipeDomicileId { get; set; }
        public Equipe EquipeDomicile { get; set; } = null!;
        public int EquipeExterieurId { get; set; }
        public Equipe EquipeExterieur { get; set; } = null!;
        public int ScoreDomicile { get; set; } = 0;
        public int ScoreExterieur { get; set; } = 0;
        public int CurrentQuarter { get; set; } = 1;
        public string Statut { get; set; } = "A venir";

        [JsonIgnore] // Évite les cycles infinis avec ChronoMatch
        public ChronoMatch? ChronoMatch { get; set; }

        [JsonIgnore] // Évite les cycles infinis avec ConfigurationMatch
        public ConfigurationMatch? Configuration { get; set; }

        [JsonIgnore] // Ignore les relations lors de la sérialisation
        public ICollection<MatchTimeout> Timeouts { get; set; } = new List<MatchTimeout>();

        [JsonIgnore]
        public ICollection<SubPlayer> SubPlayers { get; set; } = new List<SubPlayer>();
    }
}
