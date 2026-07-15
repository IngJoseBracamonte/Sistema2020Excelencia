using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SistemaSatHospitalario.Infrastructure.Services;

namespace SistemaSatHospitalario.UnitTests.Application
{
    public class UserAuditLoggerTests : IDisposable
    {
        private readonly string _logsDirectory;

        public UserAuditLoggerTests()
        {
            _logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "UserAudits");
            CleanupLogs();
        }

        private void CleanupLogs()
        {
            if (Directory.Exists(_logsDirectory))
            {
                try
                {
                    Directory.Delete(_logsDirectory, true);
                }
                catch
                {
                    // Ignorar errores de borrado en pruebas
                }
            }
        }

        [Fact]
        public async Task LogActionAsync_ShouldCreateLogFileAndWriteEntry()
        {
            // Arrange
            var logger = new UserAuditLogger();
            var username = "Dr_Bracamonte";
            var action = "ANULACION_SERVICIO";
            var cuentaId = Guid.NewGuid();
            var details = "Servicio 'LAB-Perfil Hepatico' anulado administrativamente.";

            // Act
            await logger.LogActionAsync(username, action, cuentaId, details);

            // Assert
            var expectedFileName = $"audit_{username.ToLowerInvariant()}.log";
            var expectedFilePath = Path.Combine(_logsDirectory, expectedFileName);

            Assert.True(File.Exists(expectedFilePath), "El archivo de log individual del usuario no fue creado.");

            var content = await File.ReadAllTextAsync(expectedFilePath);
            Assert.Contains($"Accion: {action}", content);
            Assert.Contains($"Cuenta: {cuentaId}", content);
            Assert.Contains(details, content);
        }

        [Fact]
        public async Task LogActionAsync_ShouldHandleConcurrencySafely()
        {
            // Arrange
            var logger = new UserAuditLogger();
            var username = "Concurrent_User";
            var cuentaId = Guid.NewGuid();
            int taskCount = 20;

            // Act & Assert
            var tasks = Enumerable.Range(1, taskCount).Select(i => 
                logger.LogActionAsync(username, "TEST_CONCURRENCY", cuentaId, $"Escritura concurrente {i}")
            );

            // No debe lanzar excepciones de E/S de archivos
            await Task.WhenAll(tasks);

            var expectedFileName = $"audit_{username.ToLowerInvariant()}.log";
            var expectedFilePath = Path.Combine(_logsDirectory, expectedFileName);

            Assert.True(File.Exists(expectedFilePath));
            var lines = await File.ReadAllLinesAsync(expectedFilePath);
            Assert.Equal(taskCount, lines.Length);
        }

        public void Dispose()
        {
            CleanupLogs();
        }
    }
}
