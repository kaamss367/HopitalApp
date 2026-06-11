using System;
using System.IO;

namespace HopitalApp.WPF
{
    public static class Logger
    {
        private static readonly string LogFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.txt");

        public static void Log(string message)
        {
            string ligne =
                $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {message}";

            File.AppendAllText(LogFile, ligne + Environment.NewLine);
        }
    }
}