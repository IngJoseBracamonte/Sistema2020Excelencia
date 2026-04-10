using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Queries.Admision;

using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public DashboardController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
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
