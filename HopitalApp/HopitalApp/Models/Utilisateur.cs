using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopitalApp.Models
{
    [Table("Utilisateur")]
    public class Utilisateur
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("login")]
        [MaxLength(100)]
        public string? Login { get; set; }

        [Column("mot_de_passe")]
        [MaxLength(255)]
        public string? MotDePasse { get; set; }

        [Column("role")]
        [MaxLength(20)]
        public string? Role { get; set; }
    }
}