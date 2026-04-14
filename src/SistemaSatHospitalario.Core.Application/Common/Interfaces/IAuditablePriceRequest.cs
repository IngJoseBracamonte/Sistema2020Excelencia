using System;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    /// <summary>
    /// [PHASE-3] Marker interface for commands that require price auditing.
    /// Used by AuditBehavior to isolate cross-cutting concerns.
    /// </summary>
    public interface IAuditablePriceRequest
    {
        string ServicioId { get; }
        string Descripcion { get; }
        decimal Precio { get; }
        decimal Honorario { get; }
        string UsuarioCarga { get; }
        string? SupervisorKey { get; }
    }
}
