namespace LiveScore.Server.DTO
{
    public class ConfigurationDto
    {
        public int Id { get; set; }
        public string NomMatch { get; set; } = string.Empty;
        public string DateHeure { get; set; } = string.Empty;
        public int NombreQuartTemps { get; set; }
        public string DureeQuartTemps { get; set; } = string.Empty;
        public string DureeTimeout { get; set; } = string.Empty;
        public int? EquipeDomicileId { get; set; }
        public int? EquipeExterieurId { get; set; }
        public string? EquipeDomicileNom { get; set; }
        public string? EquipeExterieurNom { get; set; }
        public int UserId { get; set; }
    }


}


