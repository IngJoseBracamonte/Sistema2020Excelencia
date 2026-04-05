using System;
using System.IO;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Infrastructure.Services
{
    public class LegacyErrorReportingService : ILegacyErrorReportingService
    {
        private readonly string _logPath;

        public LegacyErrorReportingService()
        {
            // Ubicación en la raíz del proyecto para fácil acceso del usuario (V12.3)
            _logPath = @"c:\Src\src\Sistema2020Excelencia\legacy_sync_log.txt";
        }

        public void LogTrace(string message)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [TRACE] {message}{Environment.NewLine}";
                File.AppendAllText(_logPath, logEntry);
            }
            catch { /* Evitamos que el log bloquee el flujo principal */ }
        }

        public void LogError(string message, Exception? ex = null)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {message}";
                if (ex != null)
                {
                    logEntry += $" | Exception: {ex.Message} | StackTrace: {ex.StackTrace}";
                }
                logEntry += Environment.NewLine;
                File.AppendAllText(_logPath, logEntry);
            }
            catch { /* Evitamos que el log bloquee el flujo principal */ }
        }
    }
}
