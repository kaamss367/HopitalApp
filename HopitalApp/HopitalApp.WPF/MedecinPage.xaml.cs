using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace HopitalApp.WPF
{
    public partial class MedecinPage : UserControl
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7101/") };
        private Medecin? _selected;

        public MedecinPage()
        {
            InitializeComponent();
            ChargerSpecialites();
            Charger();
        }

        private async void ChargerSpecialites()
        {
            cbSpecialite.ItemsSource = await _http.GetFromJsonAsync<List<Specialite>>("api/Specialites");
        }

        private async void Charger()
        {
            dgMedecins.ItemsSource = await _http.GetFromJsonAsync<List<Medecin>>("api/Medecins");
        }

        private void dgMedecins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = dgMedecins.SelectedItem as Medecin;
            if (_selected == null) return;
            txtNom.Text    = _selected.Nom;
            txtPrenom.Text = _selected.Prenom;
            cbSpecialite.SelectedValue = _selected.SpecialiteId;
        }

        private async void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (cbSpecialite.SelectedValue == null) { MessageBox.Show("Choisis une spécialité."); return; }
            var m = new Medecin { Nom = txtNom.Text, Prenom = txtPrenom.Text,
                                  SpecialiteId = (int)cbSpecialite.SelectedValue };
            var r = await _http.PostAsJsonAsync("api/Medecins", m);
            if (r.IsSuccessStatusCode) { Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de l'ajout.");
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne un médecin."); return; }
            if (cbSpecialite.SelectedValue == null) { MessageBox.Show("Choisis une spécialité."); return; }
            _selected.Nom = txtNom.Text; _selected.Prenom = txtPrenom.Text;
            _selected.SpecialiteId = (int)cbSpecialite.SelectedValue;
            var r = await _http.PutAsJsonAsync($"api/Medecins/{_selected.Id}", _selected);
            if (r.IsSuccessStatusCode) { Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de la modification.");
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne un médecin."); return; }
            var r = await _http.DeleteAsync($"api/Medecins/{_selected.Id}");
            if (r.IsSuccessStatusCode) { Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de la suppression.");
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e) => Vider();

        private void Vider()
        {
            _selected = null;
            dgMedecins.SelectedItem = null;
            txtNom.Clear(); txtPrenom.Clear();
            cbSpecialite.SelectedIndex = -1;
        }
    }
}
