using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Joueur
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nom { get; set; } = string.Empty;
    [Required]
    public string Prenom { get; set; } = string.Empty;
    [Required]
    public string Taille { get; set; } = string.Empty;
    public bool estCapitaine { get; set; } = false;
    public bool estEnJeu { get; set; } = false;
    public int Numero { get; set; }
    public string? Position { get; set; }

    // Foreign Key pour l'Équipe
    public int EquipeId { get; set; }

    [ForeignKey("EquipeId")]
    [JsonIgnore] // Ignore la relation lors de la sérialisation
    public Equipe Equipe { get; set; } = null!;
}