using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopitalApp.Models
{
    [Table("Specialite")]
    public class Specialite
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nom")]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;

        public ICollection<Medecin> Medecins { get; set; } = new List<Medecin>();
    }
}