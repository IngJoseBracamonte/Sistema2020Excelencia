using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Diagnostics;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Common.Helpers;
using System.Runtime.InteropServices;

namespace SistemaSatHospitalario.WebAPI.Controllers
{
    /// <summary>
    /// [PHASE-6] Advanced Diagnostics for "Insight" analysis.
    /// Provides technical transparency to catch "Possible Bugs" in the infrastructure.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTimeProvider _dateTime;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiagnosticsController> _logger;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
        private static readonly DateTime _startTime = DateTime.UtcNow;

        public DiagnosticsController(
            IApplicationDbContext context, 
            IDateTimeProvider dateTime,
            IConfiguration configuration,
            ILogger<DiagnosticsController> logger,
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _context = context;
            _dateTime = dateTime;
            _configuration = configuration;
            _logger = logger;
            _env = env;
        }

        [AllowAnonymous]
        [HttpGet("environment-status")]
        public IActionResult GetEnvironmentStatus()
        {
            return Ok(new
            {
                EnvironmentName = _env.EnvironmentName,
                IsDevelopment = _env.IsDevelopment(),
                IsProduction = _env.IsProduction(),
                Runtime = RuntimeInformation.FrameworkDescription,
                OS = RuntimeInformation.OSDescription,
                ServerTime = DateTime.UtcNow
            });
        }

        [HttpGet("HealthInsight")]
        public async Task<ActionResult<TechnicalInsightDto>> GetHealthInsight()
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Analyze DB Health (Potential Bug: Connection Pool Exhaustion)
            bool isDbHealthy = false;
            try {
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                isDbHealthy = true;
            } catch { }
            stopwatch.Stop();

            var currentProcess = Process.GetCurrentProcess();

            var insight = new TechnicalInsightDto
            {
                ServerTime = _dateTime.UtcNow,
                HospitalTime = _dateTime.HospitalNow,
                Uptime = (DateTime.UtcNow - _startTime).ToString(@"dd\.hh\:mm\:ss"),
                MemoryUsageBytes = currentProcess.WorkingSet64,
                DatabaseLatencyMs = stopwatch.Elapsed.TotalMilliseconds,
                SystemStatus = isDbHealthy ? "Optimal" : "Degraded",
                
                // Possible Bug Discovery: Fetch recent ErrorTickets
                RecentAnomalies = await _context.ErrorTickets
                    .AsNoTracking()
                    .OrderByDescending(e => e.FechaCreacion)
                    .Take(5)
                    .Select(e => new ErrorSummaryDto {
                        Timestamp = e.FechaCreacion,
                        Message = e.MensajeExcepcion ?? "Unknown error",
                        Path = e.RequestPath ?? "N/A"
                    })
                    .ToListAsync()
            };

            return Ok(insight);
        }

        [HttpGet("legacy-status")]
        public async Task<IActionResult> GetLegacyStatus()
        {
            var rawConnStr = _configuration.GetConnectionString("LegacyConnection");

            if (string.IsNullOrEmpty(rawConnStr))
            {
                return Ok(new { Status = "NOT_CONFIGURED", Timestamp = DateTime.UtcNow });
            }

            try
            {
                // Senior Architecture: Normalize (preserve case for legacy) and Enhance for Cloud
                var conStr = ConnectionStringHelper.NormalizeMySqlConnectionString(rawConnStr, forceLowercase: false);
                conStr = ConnectionStringHelper.EnhanceForCloud(conStr);

                var builder = new MySqlConnector.MySqlConnectionStringBuilder(conStr);
                
                using var connection = new MySqlConnector.MySqlConnection(conStr);
                await connection.OpenAsync();

                var stats = new
                {
                    Pacientes = await GetTableCountAsync(connection, "datospersonales"),
                    Ordenes = await GetTableCountAsync(connection, "ordenes"),
                    Resultados = await GetTableCountAsync(connection, "resultadospaciente")
                };

                await connection.CloseAsync();

                return Ok(new
                {
                    Status = "CONNECTED",
                    Server = builder.Server,
                    Database = builder.Database,
                    TableCounts = stats,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = "ERROR", Message = ex.Message, Timestamp = DateTime.UtcNow });
            }
        }

        private static async Task<int> GetTableCountAsync(MySqlConnector.MySqlConnection connection, string tableName)
        {
            try
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM `{tableName}`";
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch { return -1; }
        }
    }
}
