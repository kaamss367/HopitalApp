using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopitalApp.Models
{
    [Table("RendezVous")]
    public class RendezVous
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("patient_id")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [Required]
        [Column("medecin_id")]
        public int MedecinId { get; set; }

        [ForeignKey("MedecinId")]
        public Medecin? Medecin { get; set; }

        [Required]
        [Column("date_debut")]
        public DateTime DateDebut { get; set; }

        [Required]
        [Column("date_fin")]
        public DateTime DateFin { get; set; }

        [Column("informations_complementaires")]
        public string? InformationsComplementaires { get; set; }
    }
}
