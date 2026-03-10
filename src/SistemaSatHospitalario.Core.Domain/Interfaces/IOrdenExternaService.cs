using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IOrdenExternaService
    {
        Task EnviarOrdenRXAsync(string estudio, string paciente, CancellationToken cancellationToken);
        Task EnviarOrdenLegacyAsync(decimal monto, int idPersona, CancellationToken cancellationToken);
    }
}
