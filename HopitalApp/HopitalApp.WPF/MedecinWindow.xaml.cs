using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace HopitalApp.WPF
{
    public partial class MedecinWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7101/")
        };

        private Medecin? medecinSelectionne;

        public MedecinWindow()
        {
            InitializeComponent();

            ChargerSpecialites();
            ChargerMedecins();
        }

        private async void ChargerSpecialites()
        {
            var specialites =
                await _httpClient.GetFromJsonAsync<List<Specialite>>("api/Specialites");

            cbSpecialite.ItemsSource = specialites;
        }

        private async void ChargerMedecins()
        {
            var medecins =
                await _httpClient.GetFromJsonAsync<List<Medecin>>("api/Medecins");

            dgMedecins.ItemsSource = medecins;
        }

        private void dgMedecins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            medecinSelectionne = dgMedecins.SelectedItem as Medecin;

            if (medecinSelectionne != null)
            {
                txtNom.Text = medecinSelectionne.Nom;
                txtPrenom.Text = medecinSelectionne.Prenom;
                cbSpecialite.SelectedValue = medecinSelectionne.SpecialiteId;
            }
        }

        private async void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            var medecin = new Medecin
            {
                Nom = txtNom.Text,
                Prenom = txtPrenom.Text,
                SpecialiteId = (int)cbSpecialite.SelectedValue
            };

            var response =
                await _httpClient.PostAsJsonAsync("api/Medecins", medecin);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Médecin ajouté.");
                Vider();
                ChargerMedecins();
            }
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (medecinSelectionne == null)
                return;

            medecinSelectionne.Nom = txtNom.Text;
            medecinSelectionne.Prenom = txtPrenom.Text;
            medecinSelectionne.SpecialiteId = (int)cbSpecialite.SelectedValue;

            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/Medecins/{medecinSelectionne.Id}",
                    medecinSelectionne);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Médecin modifié.");
                Vider();
                ChargerMedecins();
            }
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (medecinSelectionne == null)
                return;

            var response =
                await _httpClient.DeleteAsync(
                    $"api/Medecins/{medecinSelectionne.Id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Médecin supprimé.");
                Vider();
                ChargerMedecins();
            }
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e)
        {
            Vider();
        }

        private void Vider()
        {
            medecinSelectionne = null;

            txtNom.Clear();
            txtPrenom.Clear();

            cbSpecialite.SelectedIndex = -1;

            dgMedecins.SelectedItem = null;
        }
    }

}