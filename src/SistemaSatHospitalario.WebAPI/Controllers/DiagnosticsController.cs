using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.DTOs.Diagnostics;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

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
        private static readonly DateTime _startTime = DateTime.UtcNow;

        public DiagnosticsController(IApplicationDbContext context, IDateTimeProvider dateTime)
        {
            _context = context;
            _dateTime = dateTime;
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
    }
}
