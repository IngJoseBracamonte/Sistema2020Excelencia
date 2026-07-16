using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CamaMonitoreoDto
    {
        public Guid CamaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string SedeNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty; // "Disponible" u "Ocupada"
        public bool EsAreaAdmision { get; set; }
        public Guid? PacienteId { get; set; }
        public string? PacienteNombre { get; set; }
        public string? PacienteCedula { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public Guid? CuentaId { get; set; }
        public decimal TotalFacturado { get; set; }
        public List<CamaMonitoreoDetalleCargoDto> DetallesCargos { get; set; } = new();
        public List<CamaMonitoreoTriageDto> HistorialTriage { get; set; } = new();
    }
}
