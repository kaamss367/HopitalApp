using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HopitalApp.WPF
{
    public class LoginResponse
    {
        public string? Message { get; set; }
        public string? Role    { get; set; }
    }

    public class DashboardStats
    {
        public int NombrePatients   { get; set; }
        public int NombreMedecins   { get; set; }
        public int NombreSpecialites { get; set; }
        public int NombreRendezVous  { get; set; }
    }

    public class Specialite
    {
        public int    Id  { get; set; }
        public string? Nom { get; set; }
    }

    public class Medecin
    {
        public int       Id          { get; set; }
        public string?   Nom         { get; set; }
        public string?   Prenom      { get; set; }
        public int       SpecialiteId { get; set; }
        public Specialite? Specialite { get; set; }
    }

    public class Patient
    {
        public int     Id        { get; set; }
        public string? Nom       { get; set; }
        public string? Prenom    { get; set; }
        public string? Telephone { get; set; }
        public string? Email     { get; set; }
    }

    public class RendezVous
    {
        public int       Id                         { get; set; }
        public int       PatientId                  { get; set; }
        public PatientRdv? Patient                  { get; set; }
        public int       MedecinId                  { get; set; }
        public MedecinRdv? Medecin                  { get; set; }
        public System.DateTime DateDebut            { get; set; }
        public System.DateTime DateFin              { get; set; }
        public string?   InformationsComplementaires { get; set; }
    }

    public class PatientRdv
    {
        public int     Id     { get; set; }
        public string? Nom    { get; set; }
        public string? Prenom { get; set; }
        public string NomComplet => $"{Nom} {Prenom}";
    }

    public class MedecinRdv
    {
        public int       Id          { get; set; }
        public string?   Nom         { get; set; }
        public string?   Prenom      { get; set; }
        public int       SpecialiteId { get; set; }
        public Specialite? Specialite { get; set; }
        public string NomComplet => $"{Nom} {Prenom}";
    }

    public class RandomUserResponse
    {
        [JsonPropertyName("results")]
        public List<RandomUserItem>? Results { get; set; }
    }

    public class RandomUserItem
    {
        [JsonPropertyName("name")]  public RandomUserName Name  { get; set; } = new();
        [JsonPropertyName("email")] public string Email         { get; set; } = "";
        [JsonPropertyName("phone")] public string Phone         { get; set; } = "";
    }

    public class RandomUserName
    {
        [JsonPropertyName("first")] public string First { get; set; } = "";
        [JsonPropertyName("last")]  public string Last  { get; set; } = "";
    }
}
