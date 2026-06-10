using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace HopitalApp.WPF
{
    public partial class SpecialiteWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7101/")
        };

        private Specialite? specialiteSelectionnee;

        public SpecialiteWindow()
        {
            InitializeComponent();
            ChargerSpecialites();
        }

        private async void ChargerSpecialites()
        {
            var specialites =
                await _httpClient.GetFromJsonAsync<List<Specialite>>("api/Specialites");

            dgSpecialites.ItemsSource = specialites;
        }

        private void dgSpecialites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            specialiteSelectionnee = dgSpecialites.SelectedItem as Specialite;

            if (specialiteSelectionnee != null)
            {
                txtNom.Text = specialiteSelectionnee.Nom;
            }
        }

        private async void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            var specialite = new Specialite
            {
                Nom = txtNom.Text
            };

            var response =
                await _httpClient.PostAsJsonAsync("api/Specialites", specialite);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Spécialité ajoutée.");
                ViderChamps();
                ChargerSpecialites();
            }
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (specialiteSelectionnee == null)
                return;

            specialiteSelectionnee.Nom = txtNom.Text;

            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/Specialites/{specialiteSelectionnee.Id}",
                    specialiteSelectionnee);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Spécialité modifiée.");
                ViderChamps();
                ChargerSpecialites();
            }
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (specialiteSelectionnee == null)
                return;

            var response =
                await _httpClient.DeleteAsync(
                    $"api/Specialites/{specialiteSelectionnee.Id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Spécialité supprimée.");
                ViderChamps();
                ChargerSpecialites();
            }
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e)
        {
            ViderChamps();
        }

        private void ViderChamps()
        {
            specialiteSelectionnee = null;

            txtNom.Clear();

            dgSpecialites.SelectedItem = null;
        }
    }

    public class Specialite
    {
        public int Id { get; set; }

        public string? Nom { get; set; }
    }
}