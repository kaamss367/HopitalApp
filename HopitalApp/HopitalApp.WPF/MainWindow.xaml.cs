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
                // AJOUT LOG
                Logger.Log($"Échec connexion : {txtLogin.Text}");

                MessageBox.Show("Identifiants incorrects.");
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result?.Role == "Administration" || result?.Role == "Accueil")
            {
                Logger.Log($"Connexion {result.Role} : {txtLogin.Text}");
                var shell = new ShellWindow(result.Role, txtLogin.Text);
                shell.Show();
                this.Close();
            }
            else
            {
                // AJOUT LOG
                Logger.Log($"Rôle non autorisé : {txtLogin.Text}");

                MessageBox.Show("Rôle non autorisé.");
            }
        }
    }

}