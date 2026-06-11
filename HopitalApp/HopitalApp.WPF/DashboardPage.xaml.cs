using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Controls;

namespace HopitalApp.WPF
{
    public partial class DashboardPage : UserControl
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7101/") };

        public DashboardPage()
        {
            InitializeComponent();
            Charger();
        }

        private async void Charger()
        {
            var stats = await _http.GetFromJsonAsync<DashboardStats>("api/Dashboard");
            if (stats == null) return;

            txtNbPatients.Text    = stats.NombrePatients.ToString();
            txtNbMedecins.Text    = stats.NombreMedecins.ToString();
            txtNbSpecialites.Text = stats.NombreSpecialites.ToString();
            txtNbRendezVous.Text  = stats.NombreRendezVous.ToString();
        }
    }
}
