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
        public string Login { get; set; }

        [Column("mot_de_passe")]
        public string MotDePasse { get; set; }

        [Column("role")]
        public string Role { get; set; }
    }
}