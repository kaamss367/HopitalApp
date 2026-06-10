using System;
using System.Net.Http;

namespace HopitalApp.WPF.Services
{
    public static class ApiService
    {
        public static HttpClient Client { get; }

        static ApiService()
        {
            // ⚠️ DEV uniquement : ignore l'erreur de certificat localhost
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, err) => true
            };

            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7101/") // vérifie le port dans launchSettings.json
            };
        }
    }
}