using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;

namespace SistemaSatHospitalario.Core.Domain.Interfaces.Legacy
{
    public interface ILegacyLabRepository
    {
        // El repositorio transaccional manejará internamente IDbContextTransaction
        Task<int> GenerarOrdenLaboratorioAsync(
            OrdenLegacy orden, 
            List<PerfilesFacturadosLegacy> perfilesAFacturar, 
            List<ResultadosPacienteLegacy> resultados,
            CancellationToken cancellationToken);
    }
}
