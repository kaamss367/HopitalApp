using System.Windows;

namespace HopitalApp.WPF
{
    public partial class AccueilWindow : Window
    {
        public AccueilWindow()
        {
            InitializeComponent();
        }

        private void BtnPatients_Click(object sender, RoutedEventArgs e)
        {
            PatientWindow window = new PatientWindow();
            window.Show();
        }

        private void BtnRendezVous_Click(object sender, RoutedEventArgs e)
        {
            RendezVousWindow window = new RendezVousWindow();
            window.Show();
        }
    }
}