using System;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface IUserAuditLogger
    {
        Task LogActionAsync(string usuario, string accion, Guid cuentaId, string descripcion);
    }
}
