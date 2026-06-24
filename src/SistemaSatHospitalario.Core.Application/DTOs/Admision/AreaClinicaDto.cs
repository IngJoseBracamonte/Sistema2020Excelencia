using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class AreaClinicaDto
    {
        public Guid Id { get; set; }
        public Guid SedeId { get; set; }
        public string SedeNombre { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
