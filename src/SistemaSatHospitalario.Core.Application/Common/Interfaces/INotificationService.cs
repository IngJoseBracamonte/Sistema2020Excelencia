using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendValidationAlertAsync(string title, string message, string category, object? metadata = null, CancellationToken ct = default);
        Task SendNotificationToGroupAsync(string groupName, string title, string message, string category = "General", object? metadata = null, CancellationToken ct = default);
        Task CreatePersistentNotificationAsync(string title, string message, string type, string? targetUserId = null, string? targetRole = null, string? actionUrl = null, CancellationToken ct = default);

    }
}
