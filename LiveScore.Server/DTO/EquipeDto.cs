using System.Text.Json.Serialization;

namespace LiveScore.Server.DTO
{
    public class EquipeDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Code { get; set; } = "123";
        [JsonIgnore]
        public List<JoueurDto> Joueurs { get; set; } = new();
        public string CoachNom { get; set; } = string.Empty;
        public bool EstEquipeDomicile { get; set; }
        public int? EquipeDomicileId { get; set; } // Propriété inexistante
        public int? EquipeExterieurId { get; set; } // Propriété inexistante
    }

}
