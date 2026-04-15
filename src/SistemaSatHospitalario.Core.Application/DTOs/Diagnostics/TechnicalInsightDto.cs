using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Diagnostics
{
    public class TechnicalInsightDto
    {
        public string SystemStatus { get; set; } = "Operational";
        public DateTime ServerTime { get; set; }
        public DateTime HospitalTime { get; set; }
        public string Uptime { get; set; } = string.Empty;
        public long MemoryUsageBytes { get; set; }
        public double DatabaseLatencyMs { get; set; }
        public List<ErrorSummaryDto> RecentAnomalies { get; set; } = new();
    }

    public class ErrorSummaryDto
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
