using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class DoctorHonorariumSummaryDto
    {
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; }
        public int CantidadServicios { get; set; }
        public decimal TotalHonorarios { get; set; }
        public List<HonorarioDesgloseCategoriaDto> Desglose { get; set; } = new();
    }

    public class HonorarioDesgloseCategoriaDto
    {
        public string Categoria { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
