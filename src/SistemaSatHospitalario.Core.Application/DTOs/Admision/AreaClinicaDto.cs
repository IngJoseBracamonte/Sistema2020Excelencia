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
        public Guid? ServicioTarifaBaseId { get; set; }
        public bool EsSubAreaAlmacenPrincipal { get; set; }
        public Guid? AreaPadreId { get; set; }
        public decimal TarifaBasePrecioUsd { get; set; }
        public decimal TarifaBaseHonorarioUsd { get; set; }
    }
}
