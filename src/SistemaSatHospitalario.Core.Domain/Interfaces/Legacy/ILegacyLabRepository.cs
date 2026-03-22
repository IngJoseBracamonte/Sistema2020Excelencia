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

        // Métodos de Lectura con Dapper (Capa Anticorrupción)
        Task<DatosPersonalesLegacy?> GetPatientByCedulaAsync(string cedula, CancellationToken cancellationToken);
        Task<int> CreatePatientLegacyAsync(DatosPersonalesLegacy patient, CancellationToken cancellationToken);
        
        // Búsqueda aproximada limitada para autocompletado
        Task<List<DatosPersonalesLegacy>> SearchPatientsLimitedAsync(string term, CancellationToken cancellationToken);
        
        // Mantenimiento de Convenios Legacy
        Task<List<int>> GetLegacyAgreementsIdsAsync(CancellationToken cancellationToken);
        Task<List<PerfilLegacy>> GetAvailableProfilesAsync(CancellationToken cancellationToken);
    }
}
