using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.DTOs;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class LoginCommand : IRequest<JwtAuthResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
