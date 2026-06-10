using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HopitalApp.Models
{
    [Table("Medecin")]
    public class Medecin
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nom")]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [Column("prenom")]
        [MaxLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [Column("specialite_id")]
        public int SpecialiteId { get; set; }

        [ForeignKey("SpecialiteId")]
        public Specialite? Specialite { get; set; }

        public ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();
    }
}