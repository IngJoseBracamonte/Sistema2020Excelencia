using System;
using Microsoft.AspNetCore.Identity;

namespace SistemaSatHospitalario.Infrastructure.Identity.Models
{
    public class UsuarioHospital : IdentityUser<Guid>
    {
        public string NombreReal { get; set; }
        public string ApellidoReal { get; set; }
        
        // Relación opcional con CajeroId en la base de datos Legacy
        public int? LegacyCajeroId { get; set; }

        public bool EsActivo { get; set; } = true;
        public bool RequirePasswordReset { get; set; } = false;
    }
}
