using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopitalApp.Models
{
    [Table("Patient")]
    public class Patient
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

        [Column("telephone")]
        [MaxLength(20)]
        public string? Telephone { get; set; }

        [Column("email")]
        [MaxLength(150)]
        public string? Email { get; set; }

        public ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();
    }
}