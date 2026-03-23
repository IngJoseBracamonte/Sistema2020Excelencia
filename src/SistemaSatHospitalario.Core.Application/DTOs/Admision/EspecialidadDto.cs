using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class EspecialidadDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
