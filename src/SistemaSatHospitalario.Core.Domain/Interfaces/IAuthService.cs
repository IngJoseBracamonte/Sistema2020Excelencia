using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.DTOs;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<JwtAuthResult> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
    }
}
