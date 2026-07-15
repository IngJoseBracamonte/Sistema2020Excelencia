using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Infrastructure.Services
{
    public class UserAuditLogger : IUserAuditLogger
    {
        private readonly string _logsDirectory;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _userSemaphores = new();

        public UserAuditLogger()
        {
            // Directorio de logs en la carpeta base de ejecución
            _logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "UserAudits");
        }

        public async Task LogActionAsync(string usuario, string accion, Guid cuentaId, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                usuario = "sistema";
            }

            var cleanUsername = usuario.Trim().Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
            var fileName = $"audit_{cleanUsername.ToLowerInvariant()}.log";
            var filePath = Path.Combine(_logsDirectory, fileName);

            var semaphore = _userSemaphores.GetOrAdd(cleanUsername.ToLowerInvariant(), _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();
            try
            {
                if (!Directory.Exists(_logsDirectory))
                {
                    Directory.CreateDirectory(_logsDirectory);
                }

                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"[{timestamp}] Accion: {accion.ToUpperInvariant()} | Cuenta: {cuentaId} | Detalles: {descripcion}{Environment.NewLine}";

                await File.AppendAllTextAsync(filePath, logEntry);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
