using System.Windows;
using System.Windows.Controls;

namespace HopitalApp.WPF
{
    public partial class ShellWindow : Window
    {
        private readonly string _role;

        public ShellWindow(string role, string login)
        {
            InitializeComponent();
            _role = role;
            lblUtilisateur.Text = $"{login}  ({role})";

            if (role == "Administration")
            {
                btnDashboard.Visibility    = Visibility.Visible;
                btnMedecins.Visibility     = Visibility.Visible;
                btnSpecialites.Visibility  = Visibility.Visible;
                btnUtilisateurs.Visibility = Visibility.Visible;
                NaviguerVers("Dashboard");
            }
            else
            {
                NaviguerVers("Patients");
            }
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string page)
                NaviguerVers(page);
        }

        private void NaviguerVers(string page)
        {
            MainContent.Content = page switch
            {
                "Dashboard"   => new DashboardPage(),
                "Patients"    => new PatientPage(),
                "Medecins"    => new MedecinPage(),
                "Specialites" => new SpecialitePage(),
                "RendezVous"    => new RendezVousPage(),
                "Utilisateurs"  => new UtilisateurPage(),
                _               => null
            };
        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log($"Déconnexion");
            new MainWindow().Show();
            this.Close();
        }
    }
}
