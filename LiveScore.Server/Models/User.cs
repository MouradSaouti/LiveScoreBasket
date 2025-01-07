//User.cs
namespace LiveScore.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty; // Identifiant unique
        public string Password { get; set; } = string.Empty; 
        public string Role { get; set; } = "Spectateur"; // Exemple : Admin, Préparateur, ResponsableTempsReel
    }
}
