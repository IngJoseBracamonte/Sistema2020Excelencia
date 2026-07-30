using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IOrdenExternaService
    {
        Task EnviarOrdenRXAsync(Guid cuentaId, Guid pacienteId, string estudio, string paciente, CancellationToken cancellationToken, bool requiereInforme = false, Guid? medicoInterpreteId = null);
        Task EnviarOrdenTomoAsync(Guid cuentaId, Guid pacienteId, string estudio, string paciente, CancellationToken cancellationToken, bool requiereInforme = false, Guid? medicoInterpreteId = null);
        Task EnviarOrdenLegacyAsync(decimal monto, int idPersona, CancellationToken cancellationToken);
    }
}
