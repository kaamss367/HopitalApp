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
                Logger.Log($"Ajout patient : {patient.Nom} {patient.Prenom}");
                MessageBox.Show("Patient ajouté avec succès.");
                ViderChamps();
                ChargerPatients();
            }
            else
            {
                Logger.Log("Erreur lors de l'ajout d'un patient");
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
                Logger.Log($"Modification patient ID={patientSelectionne.Id}");
                MessageBox.Show("Patient modifié avec succès.");
                ViderChamps();
                ChargerPatients();
            }
            else
            {
                Logger.Log($"Erreur modification patient ID={patientSelectionne.Id}");
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
                Logger.Log($"Suppression patient ID={patientSelectionne.Id}");
                MessageBox.Show("Patient supprimé avec succès.");
                ViderChamps();
                ChargerPatients();
            }
            else
            {
                Logger.Log($"Erreur suppression patient ID={patientSelectionne.Id}");
                MessageBox.Show("Erreur lors de la suppression du patient.");
            }
        }

        // ✅ AJOUT RANDOMUSER
        private async void BtnImporterRandomUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var randomClient = new HttpClient();

                var result = await randomClient.GetFromJsonAsync<RandomUserResponse>(
                    "https://randomuser.me/api/?results=10&nat=fr"
                );

                if (result?.Results == null)
                {
                    Logger.Log("Erreur import RandomUser : résultat vide");
                    MessageBox.Show("Erreur lors de l'import RandomUser.");
                    return;
                }

                foreach (var user in result.Results)
                {
                    var patient = new Patient
                    {
                        Nom = user.Name.Last,
                        Prenom = user.Name.First,
                        Telephone = user.Phone,
                        Email = user.Email
                    };

                    await _httpClient.PostAsJsonAsync("api/Patients", patient);
                }

                Logger.Log("Import de 10 patients depuis RandomUser");
                MessageBox.Show("10 patients importés avec succès.");

                ChargerPatients();
            }
            catch (Exception ex)
            {
                Logger.Log($"Erreur RandomUser : {ex.Message}");
                MessageBox.Show("Erreur lors de l'appel à l'API RandomUser.");
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

}