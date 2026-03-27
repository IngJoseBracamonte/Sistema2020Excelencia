using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface ITasaNotificationService
    {
        Task NotifyTasaUpdatedAsync(decimal nuevaTasa, CancellationToken cancellationToken = default);
    }
}
