using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands;
using SistemaSatHospitalario.Infrastructure.Identity.Models;

    using Microsoft.Extensions.Hosting;

    namespace SistemaSatHospitalario.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AngularPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<UsuarioHospital> _userManager;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public AuthController(IMediator mediator, UserManager<UsuarioHospital> userManager, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _mediator = mediator;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("debug-token")]
        [AllowAnonymous]
        public IActionResult DebugToken([FromBody] ValidateTokenRequest req)
        {
            var handler = new global::System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecret = _configuration["JwtConfig:Secret"] ?? "DefaultSecretKey_MustBeChangedInProduction_1234567890123456";
            var key = global::System.Text.Encoding.ASCII.GetBytes(jwtSecret);

            try
            {
                handler.ValidateToken(req.Token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtConfig:Issuer"] ?? "SistemaSatHospitalarioAPI",
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtConfig:Audience"] ?? "SistemaSatHospitalario_PWA",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                return Ok(new { valid = true, claims = ((global::System.IdentityModel.Tokens.Jwt.JwtSecurityToken)validatedToken).Claims.Select(c => new { c.Type, c.Value }) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { valid = false, error = ex.Message, stack = ex.StackTrace, secretUsed = jwtSecret.Substring(0, 5) + "..." });
            }
        }
        
        public class ValidateTokenRequest { public string Token { get; set; } = string.Empty; }

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

        // ===== TEMPORARY DEBUG ENDPOINT - REMOVE AFTER FIX =====
        [HttpGet("debug-users")]
        [AllowAnonymous]
        public async Task<IActionResult> DebugUsers()
        {
            var users = _userManager.Users.Select(u => new 
            {
                u.Id,
                u.UserName,
                u.NormalizedUserName,
                u.Email,
                u.EsActivo,
                u.LockoutEnd,
                u.AccessFailedCount,
                u.LockoutEnabled,
                HasPasswordHash = !string.IsNullOrEmpty(u.PasswordHash)
            }).ToList();

            // Also test password for admin
            var adminUser = await _userManager.FindByNameAsync("admin");
            bool? passwordValid = null;
            string[] roles = Array.Empty<string>();
            if (adminUser != null)
            {
                passwordValid = await _userManager.CheckPasswordAsync(adminUser, "Admin123*!");
                roles = (await _userManager.GetRolesAsync(adminUser)).ToArray();
            }

            return Ok(new 
            { 
                userCount = users.Count, 
                users,
                adminPasswordTest = passwordValid,
                adminRoles = roles
            });
        }
        // ===== END TEMPORARY DEBUG =====
    }
}
