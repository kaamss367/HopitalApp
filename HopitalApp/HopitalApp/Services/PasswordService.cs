namespace HopitalApp.Services
{
    public class PasswordService
    {
        // Hache un mot de passe en clair → retourne le hash BCrypt à stocker en BDD
        public string Hacher(string motDePasseClair)
            => BCrypt.Net.BCrypt.HashPassword(motDePasseClair, workFactor: 12);

        // Vérifie qu'un mot de passe en clair correspond au hash stocké en BDD
        public bool Verifier(string motDePasseClair, string hash)
            => BCrypt.Net.BCrypt.Verify(motDePasseClair, hash);

        // Détecte si une valeur est déjà un hash BCrypt (commence par $2a$ ou $2b$)
        public bool EstDejaHache(string valeur)
            => valeur.StartsWith("$2a$") || valeur.StartsWith("$2b$");
    }
}
