using LiveScore.Server.Models;
using System.Text.Json.Serialization;

namespace LiveScore.Server.Models
{
 public class EvenementMatch
{
    public int Id { get; set; }
    public int? MatchId { get; set; } // Rend MatchId optionnel
    public int? JoueurId { get; set; } // Rend JoueurId optionnel
    public string TypeEvenement { get; set; } = string.Empty;
    public int? Points { get; set; }
    public string? TypeFaute { get; set; }
    public string? Temps { get; set; } // Optionnel
    public DateTime Timestamp { get; set; }
    public int? EncodageUserId { get; set; }
    public int? EquipeId { get; set; }
    public int? JoueurSortantId { get; set; }
    public int? JoueurEntrantId { get; set; }
    public int? JoueurVictimeId { get; set; }

    // Relations
    [JsonIgnore] public User? EncodageUser { get; set; }
    [JsonIgnore] public Equipe? Equipe { get; set; }
    [JsonIgnore] public Match? Match { get; set; }
    [JsonIgnore] public Joueur? Joueur { get; set; }
    [JsonIgnore] public Joueur? JoueurVictime { get; set; }
}

}

