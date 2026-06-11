using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace HopitalApp.WPF
{
    public partial class SpecialitePage : UserControl
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7101/") };
        private Specialite? _selected;

        public SpecialitePage()
        {
            InitializeComponent();
            Charger();
        }

        private async void Charger()
        {
            dgSpecialites.ItemsSource = await _http.GetFromJsonAsync<List<Specialite>>("api/Specialites");
        }

        private void dgSpecialites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = dgSpecialites.SelectedItem as Specialite;
            if (_selected != null) txtNom.Text = _selected.Nom;
        }

        private async void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            var r = await _http.PostAsJsonAsync("api/Specialites", new Specialite { Nom = txtNom.Text });
            if (r.IsSuccessStatusCode) { Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de l'ajout.");
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne une spécialité."); return; }
            _selected.Nom = txtNom.Text;
            var r = await _http.PutAsJsonAsync($"api/Specialites/{_selected.Id}", _selected);
            if (r.IsSuccessStatusCode) { Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de la modification.");
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) { MessageBox.Show("Sélectionne une spécialité."); return; }
            var r = await _http.DeleteAsync($"api/Specialites/{_selected.Id}");
            if (r.IsSuccessStatusCode) { Vider(); Charger(); }
            else MessageBox.Show("Erreur lors de la suppression.");
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e) => Vider();

        private void Vider()
        {
            _selected = null;
            dgSpecialites.SelectedItem = null;
            txtNom.Clear();
        }
    }
}
