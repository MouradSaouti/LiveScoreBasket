namespace LiveScore.Server.Models
{
    public class CinqDeBase
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int EquipeId { get; set; }
        public Equipe Equipe { get; set; } = null!;
        public int JoueurId { get; set; }
        public Joueur Joueur { get; set; } = null!;
        
    }
}
