using System.Windows;

namespace HopitalApp.WPF
{
    public partial class PatientWindow : Window
    {
        public PatientWindow()
        {
            InitializeComponent();
        }

        private void dgPatients_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ajout patient à implémenter");
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modification patient à implémenter");
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Suppression patient à implémenter");
        }

        private void BtnVider_Click(object sender, RoutedEventArgs e)
        {
            txtNom.Clear();
            txtPrenom.Clear();
            txtTelephone.Clear();
            txtEmail.Clear();
        }
    }
}