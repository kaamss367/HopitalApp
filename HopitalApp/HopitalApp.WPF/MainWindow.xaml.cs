using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace HopitalApp.WPF
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7101/");
        }

        private async void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text;
            var motDePasse = txtPassword.Password;

            var data = new
            {
                login = login,
                motDePasse = motDePasse
            };

            var json = JsonSerializer.Serialize(data);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "api/Auth/Login",
                content);

            var result = await response.Content.ReadAsStringAsync();

            MessageBox.Show(result);
        }
    }
}