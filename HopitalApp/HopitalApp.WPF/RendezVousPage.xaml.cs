using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HopitalApp.WPF
{
    public partial class RendezVousPage : UserControl
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7101/") };
        private RendezVous? _selected;
        private List<RendezVous> _tous = new();

        public RendezVousPage()
        {
            InitializeComponent();
            ChargerPatients();
            ChargerMedecins();
            ChargerRendezVous();
        }

        private async void ChargerPatients()
        {
            var p = await _http.GetFromJsonAsync<List<PatientRdv>>("api/Patients");
            cbPatient.ItemsSource        = p;
            cbFiltrePatient.ItemsSource  = p;
        }

        private async void ChargerMedecins()
        {
            var m = await _http.GetFromJsonAsync<List<MedecinRdv>>("api/Medecins");
            cbMedecin.ItemsSource        = m;
            cbFiltreMedecin.ItemsSource  = m;
        }

        private async void ChargerRendezVous()
        {
            _tous = await _http.GetFromJsonAsync<List<RendezVous>>("api/RendezVous") ?? new();
            AfficherAVenir();
        }

        private void AfficherAVenir()
        {
            dgRendezVous.ItemsSource = _tous
                .Where(r => r.DateDebut >= DateTime.Now)
                .OrderBy(r => r.DateDebut)
                .ToList();
        }

        private void dgRendezVous_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = dgRendezVous.SelectedItem as RendezVous;
            if (_selected == null) return;

            cbPatient.SelectedValue  = _selected.PatientId;
            cbMedecin.SelectedValue  = _selected.MedecinId;
            dpDebut.SelectedDate     = _selected.DateDebut.Date;
            txtHeureDebut.Text       = _selected.DateDebut.ToString("HH:mm");
            dpFin.SelectedDate       = _selected.DateFin.Date;
            txtHeureFin.Text         = _selected.DateFin.ToString("HH:mm");
            txtInfos.Text            = _selected.InformationsComplementaires;
        }

        private void dgRendezVous_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selected == null) return;
            var detail = new RendezVousDetailWindow(_selected);
            detail.ShowDialog();
        }

        private void BtnFiltrer_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<RendezVous> r = _tous;

            if (chkHistorique.IsChecked != true)
                r = r.Where(x => x.DateDebut >= DateTime.Now);

            if (cbFiltreMedecin.SelectedValue != null)
                r = r.Where(x => x.MedecinId == (int)cbFiltreMedecin.SelectedValue);

            if (cbFiltrePatient.SelectedValue != null)
                r = r.Where(x => x.PatientId == (int)cbFiltrePatient.SelectedValue);

            if (dpFiltreDebut.SelectedDate != null)
                r = r.Where(x => x.DateDebut.Date >= dpFiltreDebut.SelectedDate.Value.Date);

            if (dpFiltreFin.SelectedDate != null)
                r = r.Where(x => x.DateDebut.Date <= dpFiltreFin.SelectedDate.Value.Date);

            dgRendezVous.ItemsSource = r.OrderBy(x => x.DateDebut).ToList();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            cbFiltreMedecin.SelectedIndex = -1;
            cbFiltrePatient.SelectedIndex = -1;
            dpFiltreDebut.SelectedDate    = null;
            dpFiltreFin.SelectedDate      = null;
            chkHistorique.IsChecked       = false;
            AfficherAVenir();
        }

        private async void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (!ConstruireRdv(out var rdv)) return;
            var r = await _http.PostAsJsonAsync("api/RendezVous", rdv);
            if (r.IsSuccessStatusCode) { Vider(); ChargerRendezVous(); }
            else MessageBox.Show(await r.Content.ReadAsStringAsync(), "Erreur");
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne un rendez-vous."); return; }
            if (!ConstruireRdv(out var rdv)) return;
            rdv.Id = _selected.Id;
            var r = await _http.PutAsJsonAsync($"api/RendezVous/{rdv.Id}", rdv);
            if (r.IsSuccessStatusCode) { Vider(); ChargerRendezVous(); }
            else MessageBox.Show(await r.Content.ReadAsStringAsync(), "Erreur");
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne un rendez-vous."); return; }
            var r = await _http.DeleteAsync($"api/RendezVous/{_selected.Id}");
            if (r.IsSuccessStatusCode) { Vider(); ChargerRendezVous(); }
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e) => Vider();

        private void BtnGenererPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne un rendez-vous."); return; }

            QuestPDF.Settings.License = LicenseType.Community;
            var rdv   = _selected;
            string chemin = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"Fiche_RendezVous_{rdv.Id}.pdf");

            Document.Create(c => c.Page(page =>
            {
                page.Margin(40);
                page.Header().Text("FICHE RENDEZ-VOUS").FontSize(24).Bold().AlignCenter();
                page.Content().Column(col =>
                {
                    col.Spacing(12);
                    col.Item().Text($"Patient : {rdv.Patient?.NomComplet}");
                    col.Item().Text($"Médecin : {rdv.Medecin?.NomComplet}");
                    col.Item().Text($"Début   : {rdv.DateDebut:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Fin     : {rdv.DateFin:dd/MM/yyyy HH:mm}");
                    col.Item().PaddingTop(10).Text("Informations complémentaires :").Bold();
                    col.Item().Text(rdv.InformationsComplementaires ?? "Aucune.");
                });
                page.Footer().AlignCenter()
                    .Text($"Document généré le {DateTime.Now:dd/MM/yyyy à HH:mm}");
            })).GeneratePdf(chemin);

            Logger.Log($"PDF généré pour le RDV {rdv.Id}");
            MessageBox.Show($"PDF généré sur le bureau :\nFiche_RendezVous_{rdv.Id}.pdf");
        }

        private bool ConstruireRdv(out RendezVous rdv)
        {
            rdv = new RendezVous();
            if (cbPatient.SelectedValue == null || cbMedecin.SelectedValue == null)
            { MessageBox.Show("Choisis un patient et un médecin."); return false; }
            if (dpDebut.SelectedDate == null || dpFin.SelectedDate == null)
            { MessageBox.Show("Choisis les dates."); return false; }
            if (!TimeSpan.TryParse(txtHeureDebut.Text, out var hd))
            { MessageBox.Show("Heure de début invalide. Ex : 09:00"); return false; }
            if (!TimeSpan.TryParse(txtHeureFin.Text, out var hf))
            { MessageBox.Show("Heure de fin invalide. Ex : 09:30"); return false; }

            rdv.PatientId  = (int)cbPatient.SelectedValue;
            rdv.MedecinId  = (int)cbMedecin.SelectedValue;
            rdv.DateDebut  = dpDebut.SelectedDate.Value.Date + hd;
            rdv.DateFin    = dpFin.SelectedDate.Value.Date   + hf;
            rdv.InformationsComplementaires = txtInfos.Text;
            return true;
        }

        private void Vider()
        {
            _selected = null;
            dgRendezVous.SelectedItem  = null;
            cbPatient.SelectedIndex    = -1;
            cbMedecin.SelectedIndex    = -1;
            dpDebut.SelectedDate       = null;
            dpFin.SelectedDate         = null;
            txtHeureDebut.Text         = "09:00";
            txtHeureFin.Text           = "09:30";
            txtInfos.Clear();
        }
    }
}
