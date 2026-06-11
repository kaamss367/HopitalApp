using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace HopitalApp.WPF
{
    public partial class AdminWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7101/")
        };

        public AdminWindow()
        {
            InitializeComponent();
            ChargerDashboard();
        }

        private async void ChargerDashboard()
        {
            var stats = await _httpClient.GetFromJsonAsync<DashboardStats>("api/Dashboard");

            if (stats != null)
            {
                txtNbPatients.Text = stats.NombrePatients.ToString();
                txtNbMedecins.Text = stats.NombreMedecins.ToString();
                txtNbSpecialites.Text = stats.NombreSpecialites.ToString();
                txtNbRendezVous.Text = stats.NombreRendezVous.ToString();
            }
        }

        private void BtnPatients_Click(object sender, RoutedEventArgs e)
        {
            PatientWindow patientWindow = new PatientWindow();
            patientWindow.Show();
        }

        private void BtnMedecins_Click(object sender, RoutedEventArgs e)
        {
            MedecinWindow window = new MedecinWindow();
            window.Show();
        }

        private void BtnSpecialites_Click(object sender, RoutedEventArgs e)
        {
            SpecialiteWindow window = new SpecialiteWindow();
            window.Show();
        }

        private void BtnRendezVous_Click(object sender, RoutedEventArgs e)
        {
            RendezVousWindow window = new RendezVousWindow();
            window.Show();
        }
    }

}