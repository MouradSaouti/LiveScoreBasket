public class MatchDto
{
    public DateTime DateMatch { get; set; }
    public int EquipeDomicileId { get; set; }
    public int EquipeExterieurId { get; set; }
    public int ScoreDomicile { get; set; }
    public int ScoreExterieur { get; set; }
    public string Statut { get; set; } = "A Venir";
}
