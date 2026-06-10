using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace HopitalApp.WPF
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7101/")
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            var data = new
            {
                login = txtLogin.Text,
                motDePasse = txtPassword.Password
            };

            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", data);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Identifiants incorrects.");
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result?.Role == "Administration")
            {
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Rôle non autorisé.");
            }
        }
    }

    public class LoginResponse
    {
        public string? Message { get; set; }
        public string? Role { get; set; }
    }
}