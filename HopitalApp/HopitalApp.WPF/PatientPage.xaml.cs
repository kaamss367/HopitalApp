using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace HopitalApp.WPF
{
    public partial class PatientPage : UserControl
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7101/") };
        private Patient? _selected;

        public PatientPage()
        {
            InitializeComponent();
            Charger();
        }

        private async void Charger()
        {
            dgPatients.ItemsSource = await _http.GetFromJsonAsync<List<Patient>>("api/Patients");
        }

        private void dgPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = dgPatients.SelectedItem as Patient;
            if (_selected == null) return;
            txtNom.Text       = _selected.Nom;
            txtPrenom.Text    = _selected.Prenom;
            txtTelephone.Text = _selected.Telephone;
            txtEmail.Text     = _selected.Email;
        }

        private async void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            var p = new Patient { Nom = txtNom.Text, Prenom = txtPrenom.Text,
                                  Telephone = txtTelephone.Text, Email = txtEmail.Text };
            var r = await _http.PostAsJsonAsync("api/Patients", p);
            if (r.IsSuccessStatusCode) { Logger.Log($"Ajout patient : {p.Nom} {p.Prenom}"); Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de l'ajout.");
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne un patient."); return; }
            _selected.Nom = txtNom.Text; _selected.Prenom = txtPrenom.Text;
            _selected.Telephone = txtTelephone.Text; _selected.Email = txtEmail.Text;
            var r = await _http.PutAsJsonAsync($"api/Patients/{_selected.Id}", _selected);
            if (r.IsSuccessStatusCode) { Logger.Log($"Modif patient ID={_selected.Id}"); Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de la modification.");
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne un patient."); return; }
            var r = await _http.DeleteAsync($"api/Patients/{_selected.Id}");
            if (r.IsSuccessStatusCode) { Logger.Log($"Suppression patient ID={_selected.Id}"); Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de la suppression.");
        }

        private async void BtnImporterRandomUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rc = new HttpClient();
                var res = await rc.GetFromJsonAsync<RandomUserResponse>("https://randomuser.me/api/?results=10&nat=fr");
                if (res?.Results == null) { MessageBox.Show("Erreur RandomUser."); return; }
                foreach (var u in res.Results)
                    await _http.PostAsJsonAsync("api/Patients", new Patient
                    { Nom = u.Name.Last, Prenom = u.Name.First, Telephone = u.Phone, Email = u.Email });
                Logger.Log("Import 10 patients RandomUser");
                MessageBox.Show("10 patients importés.");
                Charger();
            }
            catch (Exception ex)
            {
                Logger.Log($"Erreur RandomUser : {ex.Message}");
                MessageBox.Show("Erreur appel RandomUser.");
            }
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e) => Vider();

        private void Vider()
        {
            _selected = null;
            dgPatients.SelectedItem = null;
            txtNom.Clear(); txtPrenom.Clear(); txtTelephone.Clear(); txtEmail.Clear();
        }
    }
}
