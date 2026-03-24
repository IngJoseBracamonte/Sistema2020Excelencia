using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands;

    using Microsoft.Extensions.Hosting;

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
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity("LoginProcess");
            activity?.SetTag("user.username", command.Username);
            
            DiagnosticsConfig.LoginCounter.Add(1, new TagList { { "user.username", command.Username } });

            var result = await _mediator.Send(command, cancellationToken);
            if (result == null)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Invalid credentials");
                return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo." });
            }

            activity?.SetStatus(ActivityStatusCode.Ok);
            return Ok(result);
        }
    }
}
