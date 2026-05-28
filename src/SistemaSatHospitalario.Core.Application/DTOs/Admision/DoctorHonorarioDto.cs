using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class DoctorHonorarioDto
    {
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; } = string.Empty;
        public decimal Honorario { get; set; }
    }
}
