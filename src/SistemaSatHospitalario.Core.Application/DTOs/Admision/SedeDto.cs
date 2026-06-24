using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class SedeDto
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool EsPrincipal { get; set; }
        public bool Activo { get; set; }
    }
}
