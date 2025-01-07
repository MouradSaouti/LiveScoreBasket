namespace LiveScore.Server.Models
{
    public class Coach
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty; // Nom du coach
        public int EquipeId { get; set; } // Référence à l'équipe
        public Equipe Equipe { get; set; } = null!; // Relation avec l'équipe
    }
}
