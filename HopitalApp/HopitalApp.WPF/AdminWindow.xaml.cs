using System.Windows;

namespace HopitalApp.WPF
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void BtnPatients_Click(object sender, RoutedEventArgs e)
        {
            PatientWindow patientWindow = new PatientWindow();
            patientWindow.Show();
        }

        private void BtnMedecins_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fenêtre Médecins à venir");
        }

        private void BtnSpecialites_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fenêtre Spécialités à venir");
        }

        private void BtnRendezVous_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fenêtre Rendez-vous à venir");
        }
    }
}