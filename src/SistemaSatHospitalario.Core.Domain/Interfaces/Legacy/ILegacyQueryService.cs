using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using SistemaSatHospitalario.Core.Domain.DTOs.Legacy;

namespace SistemaSatHospitalario.Core.Domain.Interfaces.Legacy
{
    public interface ILegacyQueryService
    {
        /// <summary>
        /// Obtiene la lista de análisis asociados a uno o más perfiles desde perfilesanalisis (Legacy).
        /// </summary>
        Task<IEnumerable<AnalysisMappingDto>> GetAnalysesForProfilesAsync(List<int> profileIds, CancellationToken cancellationToken);
        
        /// <summary>
        /// Obtiene el conteo de órdenes del día para calcular NumeroDia (Legacy).
        /// </summary>
        Task<int> GetCurrentDayOrderCountAsync(CancellationToken cancellationToken);
    }
}
