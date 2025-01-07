using LiveScore.Server.Models;
using System.Text.Json.Serialization;

public class ConfigurationMatch
{
    public int Id { get; set; }
    public string NomMatch { get; set; } = string.Empty;
    public int NombreQuartTemps { get; set; } = 4;
    public TimeSpan DureeQuartTemps { get; set; } = TimeSpan.FromMinutes(12);
    public TimeSpan DureeTimeout { get; set; } = TimeSpan.FromSeconds(59);
    public DateTime DateHeure { get; set; }

    public int? EquipeDomicileId { get; set; }
    public Equipe? EquipeDomicile { get; set; }
    public int? EquipeExterieurId { get; set; }
    public Equipe? EquipeExterieur { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string? Statut { get; set; } // Ajouter ce champ
    [JsonIgnore] // Évite les cycles infinis avec Match
    public Match? Match { get; set; }
    public int? MatchId { get; set; } // Clé étrangère explicite
}
