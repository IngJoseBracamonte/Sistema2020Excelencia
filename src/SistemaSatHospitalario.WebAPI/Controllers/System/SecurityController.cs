using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.WebAPI.Controllers.System
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;

        public SecurityController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// (Senior Diagnostic Mode) Retorna el contexto de identidad del usuario actual 
        /// y sus claims para depuración técnica sin herramientas externas.
        /// </summary>
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            var claims = User.Claims.Select(c => new 
            { 
                Type = c.Type, 
                Value = c.Value 
            }).ToList();

            return Ok(new
            {
                Context = new
                {
                    _currentUserService.UserId,
                    _currentUserService.UserName,
                    _currentUserService.Role,
                    _currentUserService.IsAuthenticated,
                    IsAdmin = _currentUserService.IsAdmin()
                },
                AllClaims = claims
            });
        }
    }
}
