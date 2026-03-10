using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands;

namespace SistemaSatHospitalario.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AngularPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo." });
            }

            return Ok(result);
        }
    }
}
