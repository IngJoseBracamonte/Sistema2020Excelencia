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
            // [Senior Pattern] El rol se extrae automáticamente dentro del Handler
            var results = await _mediator.Send(new GetBusinessInsightsQuery());
            
            return Ok(results);
        }
    }
}
