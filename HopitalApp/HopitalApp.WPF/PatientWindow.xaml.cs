using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace HopitalApp.WPF
{
    public partial class PatientWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7101/")
        };

        private Patient? patientSelectionne;

        public PatientWindow()
        {
            InitializeComponent();
            ChargerPatients();
        }

        private async void ChargerPatients()
        {
            var patients = await _httpClient.GetFromJsonAsync<List<Patient>>("api/Patients");
            dgPatients.ItemsSource = patients;
        }

        private void dgPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            patientSelectionne = dgPatients.SelectedItem as Patient;

            if (patientSelectionne != null)
            {
                txtNom.Text = patientSelectionne.Nom;
                txtPrenom.Text = patientSelectionne.Prenom;
                txtTelephone.Text = patientSelectionne.Telephone;
                txtEmail.Text = patientSelectionne.Email;
            }
        }

        private async void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            var patient = new Patient
            {
                Nom = txtNom.Text,
                Prenom = txtPrenom.Text,
                Telephone = txtTelephone.Text,
                Email = txtEmail.Text
            };

            var response = await _httpClient.PostAsJsonAsync("api/Patients", patient);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Patient ajouté avec succès.");
                ViderChamps();
                ChargerPatients();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout du patient.");
            }
        }

        private async void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (patientSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un patient.");
                return;
            }

            patientSelectionne.Nom = txtNom.Text;
            patientSelectionne.Prenom = txtPrenom.Text;
            patientSelectionne.Telephone = txtTelephone.Text;
            patientSelectionne.Email = txtEmail.Text;

            var response = await _httpClient.PutAsJsonAsync(
                $"api/Patients/{patientSelectionne.Id}",
                patientSelectionne
            );

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Patient modifié avec succès.");
                ViderChamps();
                ChargerPatients();
            }
            else
            {
                MessageBox.Show("Erreur lors de la modification du patient.");
            }
        }

        private async void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (patientSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un patient.");
                return;
            }

            var response = await _httpClient.DeleteAsync($"api/Patients/{patientSelectionne.Id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Patient supprimé avec succès.");
                ViderChamps();
                ChargerPatients();
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression du patient.");
            }
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e)
        {
            ViderChamps();
        }

        private void ViderChamps()
        {
            patientSelectionne = null;
            dgPatients.SelectedItem = null;

            txtNom.Clear();
            txtPrenom.Clear();
            txtTelephone.Clear();
            txtEmail.Clear();
        }
    }

    public class Patient
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
    }
}