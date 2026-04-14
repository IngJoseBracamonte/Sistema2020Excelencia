using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendValidationAlertAsync(string title, string message, string category, object? metadata = null, CancellationToken ct = default);
    }
}
