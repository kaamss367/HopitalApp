using System.Windows;

namespace HopitalApp.WPF
{
    public partial class RendezVousDetailWindow : Window
    {
        public RendezVousDetailWindow(RendezVous rdv)
        {
            InitializeComponent();

            lblPatient.Text = rdv.Patient?.NomComplet ?? $"ID {rdv.PatientId}";
            lblMedecin.Text = rdv.Medecin?.NomComplet ?? $"ID {rdv.MedecinId}";
            lblDebut.Text   = rdv.DateDebut.ToString("dd/MM/yyyy HH:mm");
            lblFin.Text     = rdv.DateFin.ToString("dd/MM/yyyy HH:mm");
            txtInfos.Text   = string.IsNullOrWhiteSpace(rdv.InformationsComplementaires)
                                ? "Aucune information complémentaire."
                                : rdv.InformationsComplementaires;
        }

        private void BtnFermer_Click(object sender, RoutedEventArgs e) => Close();
    }
}
