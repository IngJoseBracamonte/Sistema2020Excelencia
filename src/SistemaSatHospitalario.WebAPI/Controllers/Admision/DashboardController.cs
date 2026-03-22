using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Insights")]
        public async Task<ActionResult<BusinessInsightsDto>> GetInsights()
        {
            // Extraer el rol del token JWT
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            
            var results = await _mediator.Send(new GetBusinessInsightsQuery 
            { 
                UserRole = userRole 
            });
            
            return Ok(results);
        }
    }
}
