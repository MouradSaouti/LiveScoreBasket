namespace LiveScore.Server.DTO
{
    public class EventDto
    {
        public int MatchId { get; set; }
        public int JoueurId { get; set; }
        public string TypeEvenement { get; set; } // Panier, Faute, Changement
        public string? Temps { get; set; } // Ajout de ? pour rendre le champ optionnel
        public int? Points { get; set; }
        public string? TypeFaute { get; set; } // Exemple : P1, P2, etc.
        public int EquipeId { get; set; }
        public int EncodageUserId { get; set; } // ID de l'utilisateur qui enregistre l'événement
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Champs spécifiques au changement
        public int? JoueurSortantId { get; set; } // Joueur remplacé
        public int? JoueurEntrantId { get; set; } // Joueur entrant

        // Nouveau champ : Joueur victime (J2) dans une faute
        public int? JoueurVictimeId { get; set; } // ID du joueur victime (ex : J2)
    }
}

