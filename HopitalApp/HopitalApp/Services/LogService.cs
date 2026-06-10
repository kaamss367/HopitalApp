using System;
using System.IO;

namespace HopitalApp.Services
{
    public class LogService
    {
        private readonly string _logFile = "logs.txt";

        public void EcrireLog(string message)
        {
            string ligne = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            File.AppendAllText(_logFile, ligne + Environment.NewLine);
        }
    }
}