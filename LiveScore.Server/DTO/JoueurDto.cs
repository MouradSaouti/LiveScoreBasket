using System.Text.Json.Serialization;

namespace LiveScore.Server.DTO
{
    public class JoueurDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = null!;
        public string Taille { get; set; } = null!;
        public bool estCapitaine { get; set; } = false;
        public bool estEnJeu { get; set; } = false;
        public int Numero { get; set; }
        public string? Position { get; set; }
        public int EquipeId { get; set; }
        public string? EquipeNom { get; set; } // Nom de l'équipe
    }

}
