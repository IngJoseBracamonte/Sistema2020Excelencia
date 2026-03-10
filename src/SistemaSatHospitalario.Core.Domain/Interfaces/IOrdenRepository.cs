using System;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IOrdenRepository
    {
        Task<OrdenDeServicio?> ObtenerDetalleOrdenAsync(Guid ordenId, CancellationToken cancellationToken);
        Task ActualizarOrdenAsync(OrdenDeServicio orden, CancellationToken cancellationToken);
    }
}
