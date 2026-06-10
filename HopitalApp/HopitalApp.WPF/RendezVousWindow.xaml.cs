using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HopitalApp.WPF
{
    public partial class RendezVousWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7101/")
        };

        private RendezVous? rendezVousSelectionne;
        private List<RendezVous> tousLesRendezVous = new();

        public RendezVousWindow()
        {
            InitializeComponent();

            ChargerPatients();
            ChargerMedecins();
            ChargerRendezVous();
        }

        private async void ChargerPatients()
        {
            var patients = await _httpClient.GetFromJsonAsync<List<PatientRdv>>("api/Patients");

            cbPatient.ItemsSource = patients;
            cbFiltrePatient.ItemsSource = patients;
        }

        private async void ChargerMedecins()
        {
            var medecins = await _httpClient.GetFromJsonAsync<List<MedecinRdv>>("api/Medecins");

            cbMedecin.ItemsSource = medecins;
            cbFiltreMedecin.ItemsSource = medecins;
        }

        private async void ChargerRendezVous()
        {
            var rdv = await _httpClient.GetFromJsonAsync<List<RendezVous>>("api/RendezVous");

            tousLesRendezVous = rdv ?? new List<RendezVous>();

            AfficherRendezVousAVenir();
        }

        private void AfficherRendezVousAVenir()
        {
            dgRendezVous.ItemsSource = tousLesRendezVous
                .Where(r => r.DateDebut >= DateTime.Now)
                .OrderBy(r => r.DateDebut)
                .ToList();
        }

        private void dgRendezVous_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            rendezVousSelectionne = dgRendezVous.SelectedItem as RendezVous;

            if (rendezVousSelectionne == null)
                return;

            cbPatient.SelectedValue = rendezVousSelectionne.PatientId;
            cbMedecin.SelectedValue = rendezVousSelectionne.MedecinId;

            dpDebut.SelectedDate = rendezVousSelectionne.DateDebut.Date;
            txtHeureDebut.Text = rendezVousSelectionne.DateDebut.ToString("HH:mm");

            dpFin.SelectedDate = rendezVousSelectionne.DateFin.Date;
            txtHeureFin.Text = rendezVousSelectionne.DateFin.ToString("HH:mm");

            txtInfos.Text = rendezVousSelectionne.InformationsComplementaires;
        }

        private async void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (!ConstruireRendezVous(out RendezVous rdv))
                return;

            var response = await _httpClient.PostAsJsonAsync("api/RendezVous", rdv);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Rendez-vous ajouté.");
                Vider();
                ChargerRendezVous();
            }
            else
            {
                var erreur = await response.Content.ReadAsStringAsync();
                MessageBox.Show(erreur, "Erreur");
            }
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (rendezVousSelectionne == null)
            {
                MessageBox.Show("Sélectionne un rendez-vous.");
                return;
            }

            if (!ConstruireRendezVous(out RendezVous rdv))
                return;

            rdv.Id = rendezVousSelectionne.Id;

            var response = await _httpClient.PutAsJsonAsync($"api/RendezVous/{rdv.Id}", rdv);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Rendez-vous modifié.");
                Vider();
                ChargerRendezVous();
            }
            else
            {
                var erreur = await response.Content.ReadAsStringAsync();
                MessageBox.Show(erreur, "Erreur");
            }
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (rendezVousSelectionne == null)
            {
                MessageBox.Show("Sélectionne un rendez-vous.");
                return;
            }

            var response = await _httpClient.DeleteAsync($"api/RendezVous/{rendezVousSelectionne.Id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Rendez-vous supprimé.");
                Vider();
                ChargerRendezVous();
            }
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e)
        {
            Vider();
        }

        private void BtnFiltrer_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<RendezVous> resultats = tousLesRendezVous;

            if (!chkHistorique.IsChecked != true)
            {
                resultats = resultats.Where(r => r.DateDebut >= DateTime.Now);
            }

            if (cbFiltreMedecin.SelectedValue != null)
            {
                resultats = resultats.Where(r => r.MedecinId == (int)cbFiltreMedecin.SelectedValue);
            }

            if (cbFiltrePatient.SelectedValue != null)
            {
                resultats = resultats.Where(r => r.PatientId == (int)cbFiltrePatient.SelectedValue);
            }

            if (dpFiltreDebut.SelectedDate != null)
            {
                resultats = resultats.Where(r => r.DateDebut.Date >= dpFiltreDebut.SelectedDate.Value.Date);
            }

            if (dpFiltreFin.SelectedDate != null)
            {
                resultats = resultats.Where(r => r.DateDebut.Date <= dpFiltreFin.SelectedDate.Value.Date);
            }

            dgRendezVous.ItemsSource = resultats
                .OrderBy(r => r.DateDebut)
                .ToList();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            cbFiltreMedecin.SelectedIndex = -1;
            cbFiltrePatient.SelectedIndex = -1;
            dpFiltreDebut.SelectedDate = null;
            dpFiltreFin.SelectedDate = null;
            chkHistorique.IsChecked = false;

            AfficherRendezVousAVenir();
        }

        private void dgRendezVous_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (rendezVousSelectionne == null)
                return;

            MessageBox.Show(
                $"Patient : {rendezVousSelectionne.Patient?.NomComplet}\n" +
                $"Médecin : {rendezVousSelectionne.Medecin?.NomComplet}\n" +
                $"Début : {rendezVousSelectionne.DateDebut:dd/MM/yyyy HH:mm}\n" +
                $"Fin : {rendezVousSelectionne.DateFin:dd/MM/yyyy HH:mm}\n\n" +
                $"Infos : {rendezVousSelectionne.InformationsComplementaires}",
                "Détail du rendez-vous"
            );
        }

        private bool ConstruireRendezVous(out RendezVous rdv)
        {
            rdv = new RendezVous();

            if (cbPatient.SelectedValue == null || cbMedecin.SelectedValue == null)
            {
                MessageBox.Show("Choisis un patient et un médecin.");
                return false;
            }

            if (dpDebut.SelectedDate == null || dpFin.SelectedDate == null)
            {
                MessageBox.Show("Choisis les dates de début et de fin.");
                return false;
            }

            if (!TimeSpan.TryParse(txtHeureDebut.Text, out TimeSpan heureDebut))
            {
                MessageBox.Show("Heure de début invalide. Exemple : 09:00");
                return false;
            }

            if (!TimeSpan.TryParse(txtHeureFin.Text, out TimeSpan heureFin))
            {
                MessageBox.Show("Heure de fin invalide. Exemple : 09:30");
                return false;
            }

            rdv.PatientId = (int)cbPatient.SelectedValue;
            rdv.MedecinId = (int)cbMedecin.SelectedValue;
            rdv.DateDebut = dpDebut.SelectedDate.Value.Date + heureDebut;
            rdv.DateFin = dpFin.SelectedDate.Value.Date + heureFin;
            rdv.InformationsComplementaires = txtInfos.Text;

            return true;
        }

        private void Vider()
        {
            rendezVousSelectionne = null;

            cbPatient.SelectedIndex = -1;
            cbMedecin.SelectedIndex = -1;

            dpDebut.SelectedDate = null;
            dpFin.SelectedDate = null;

            txtHeureDebut.Text = "09:00";
            txtHeureFin.Text = "09:30";
            txtInfos.Clear();

            dgRendezVous.SelectedItem = null;
        }
    }

    public class RendezVous
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public PatientRdv? Patient { get; set; }

        public int MedecinId { get; set; }
        public MedecinRdv? Medecin { get; set; }

        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }

        public string? InformationsComplementaires { get; set; }
    }

    public class PatientRdv
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }

        public string NomComplet => $"{Nom} {Prenom}";
    }

    public class MedecinRdv
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public int SpecialiteId { get; set; }

        public Specialite? Specialite { get; set; }

        public string NomComplet => $"{Nom} {Prenom}";
    }
}