using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface ICajaAdministrativaRepository
    {
        Task<CajaDiaria?> ObtenerCajaAbiertaAsync(CancellationToken cancellationToken);
        Task<CajaDiaria?> ObtenerCajaAbiertaPorUsuarioAsync(string usuarioId, CancellationToken cancellationToken);
        Task AgregarCajaAsync(CajaDiaria caja, CancellationToken cancellationToken);
        Task GuardarCambiosAsync(CancellationToken cancellationToken);
        
        Task<CajaDiaria?> ObtenerCajaAbiertaConDetallesAsync(CancellationToken cancellationToken);
        
        // Métricas para Admin (Micro-Ciclo 28)
        Task<IEnumerable<CajaDiaria>> ObtenerHistorialCierresAsync(DateTime desde, DateTime hasta, string? usuarioId, CancellationToken cancellationToken);
    }
}
