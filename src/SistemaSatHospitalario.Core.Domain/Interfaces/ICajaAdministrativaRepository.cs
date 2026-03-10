using System;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface ICajaAdministrativaRepository
    {
        Task<CajaDiaria?> ObtenerCajaAbiertaAsync(CancellationToken cancellationToken);
        Task AgregarCajaAsync(CajaDiaria caja, CancellationToken cancellationToken);
        Task GuardarCambiosAsync(CancellationToken cancellationToken);
        

        
        Task<CajaDiaria?> ObtenerCajaAbiertaConDetallesAsync(CancellationToken cancellationToken);
    }
}
