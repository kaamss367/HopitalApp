using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HopitalApp.WPF
{
    public partial class UtilisateurPage : UserControl
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri("https://localhost:7101/") };

        public UtilisateurPage() => InitializeComponent();

        private async void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            string login   = txtLogin.Text.Trim();
            string mdp     = txtMotDePasse.Password;
            string confirm = txtConfirm.Password;
            string role    = (cbRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Accueil";

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(mdp))
            {
                AfficherResultat("Login et mot de passe obligatoires.", "#C2185B");
                return;
            }
            if (mdp.Length < 6)
            {
                AfficherResultat("Le mot de passe doit contenir au moins 6 caractères.", "#C2185B");
                return;
            }
            if (mdp != confirm)
            {
                AfficherResultat("Les mots de passe ne correspondent pas.", "#C2185B");
                return;
            }

            var dto = new { login, motDePasse = mdp, role };
            var r   = await _http.PostAsJsonAsync("api/Auth/CreerUtilisateur", dto);

            if (r.IsSuccessStatusCode)
            {
                AfficherResultat($"Compte « {login} » créé avec succès !", "#2E7D32");
                txtLogin.Clear();
                txtMotDePasse.Clear();
                txtConfirm.Clear();
                cbRole.SelectedIndex = 0;
            }
            else
            {
                var msg = await r.Content.ReadAsStringAsync();
                AfficherResultat($"Erreur : {msg}", "#C2185B");
            }
        }

        private async void BtnMigrer_Click(object sender, RoutedEventArgs e)
        {
            var r = await _http.PostAsync("api/Auth/MigreMotsDePasse", null);
            if (r.IsSuccessStatusCode)
            {
                var json = await r.Content.ReadAsStringAsync();
                var doc  = JsonDocument.Parse(json);
                var msg  = doc.RootElement.GetProperty("message").GetString() ?? json;
                MessageBox.Show(msg, "Migration réussie");
            }
            else
            {
                MessageBox.Show("Erreur lors de la migration.", "Erreur");
            }
        }

        private void AfficherResultat(string message, string couleur)
        {
            lblResultat.Text       = message;
            lblResultat.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(couleur));
        }
    }
}
