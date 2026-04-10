using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class DoctorHonorariumSummaryDto
    {
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; }
        public int CantidadServicios { get; set; }
        public decimal TotalHonorarios { get; set; }
    }
}
