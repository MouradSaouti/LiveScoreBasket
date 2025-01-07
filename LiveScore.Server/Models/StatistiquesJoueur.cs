namespace LiveScore.Server.Models
{
    public class StatistiquesJoueur
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int JoueurId { get; set; }
        public Joueur Joueur { get; set; } = null!;
        public int Points { get; set; }
        public int Rebonds { get; set; }
        public int Passes { get; set; }
        public int Interceptions { get; set; }
        public int Contres { get; set; }
    }

}
