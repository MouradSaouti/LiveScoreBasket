using LiveScore.Server.Models;
using System.Text.Json.Serialization;

public class Equipe
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Code { get; set; } = "123";

    public List<Joueur> Joueurs { get; set; } = new(); // Liste des joueurs

    [JsonIgnore] // Empêche la sérialisation de la relation inverse
    public Coach? Coach { get; set; }
    public bool EstEquipeDomicile { get; set; }

    [JsonIgnore] // Empêche les boucles avec les relations Match
    public List<Match> Matches { get; set; } = new();
}
