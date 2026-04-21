using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class MedicoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Especialidad { get; set; }
        public Guid EspecialidadId { get; set; }
        public decimal HonorarioBase { get; set; }
        public bool Activo { get; set; }
        public string? Telefono { get; set; }
    }
}
