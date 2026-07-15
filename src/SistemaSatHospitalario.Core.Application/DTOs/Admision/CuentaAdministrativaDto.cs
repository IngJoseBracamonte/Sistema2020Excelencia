using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CuentaAdministrativaDto
    {
        public Guid CuentaId { get; set; }
        public Guid PacienteId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCedula { get; set; } = string.Empty;
        public DateTime FechaCarga { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string TipoIngreso { get; set; } = string.Empty;
        public int? ConvenioId { get; set; }
        public string? SeguroNombre { get; set; }
        public decimal Total { get; set; }
        public Guid? ReciboId { get; set; }
        public string? NumeroRecibo { get; set; }
        
        public List<CuentaAdministrativaDetailDto> Detalles { get; set; } = new();
    }

    public class CuentaAdministrativaDetailDto
    {
        public Guid Id { get; set; }
        public Guid ServicioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Honorario { get; set; }
        public decimal Cantidad { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public DateTime FechaCarga { get; set; }
        public string? LegacyMappingId { get; set; }
        public bool IncluidoEnTarifaBase { get; set; }
    }
}
