using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admision
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles + "," + AuthorizationConstants.Medico + "," + AuthorizationConstants.AsistenteHospitalario + "," + AuthorizationConstants.AsistenteEmergencia + "," + AuthorizationConstants.Cajero + "," + AuthorizationConstants.Supervisor)]
    [ApiController]
    [Route("api/[controller]")]
    public class TriageController : ControllerBase
    {
        [HttpGet("niveles")]
        public IActionResult GetNiveles()
        {
            var niveles = new[]
            {
                new { Id = "1", Codigo = "I", Nombre = "Nivel I (Rojo) - Reanimación", ColorHex = "#EF4444", TiempoAtencionMinutos = 0 },
                new { Id = "2", Codigo = "II", Nombre = "Nivel II (Naranja) - Emergencia", ColorHex = "#F97316", TiempoAtencionMinutos = 15 },
                new { Id = "3", Codigo = "III", Nombre = "Nivel III (Amarillo) - Urgencia", ColorHex = "#EAB308", TiempoAtencionMinutos = 30 },
                new { Id = "4", Codigo = "IV", Nombre = "Nivel IV (Verde) - Menor", ColorHex = "#22C55E", TiempoAtencionMinutos = 60 }
            };
            return Ok(niveles);
        }
    }
}
