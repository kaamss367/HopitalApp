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