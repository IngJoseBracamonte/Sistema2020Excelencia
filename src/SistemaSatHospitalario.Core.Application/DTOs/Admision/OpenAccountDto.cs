using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class OpenAccountDto
    {
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public string TipoIngreso { get; set; } = string.Empty;
        public int? ConvenioId { get; set; }
        public List<OpenAccountDetailDto> Detalles { get; set; } = new();
    }

    public class OpenAccountDetailDto
    {
        public Guid Id { get; set; }
        public Guid ServicioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Honorario { get; set; }
        public int Cantidad { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public DateTime FechaCarga { get; set; }
        public string? LegacyMappingId { get; set; }
        public Guid? MedicoResponsableId { get; set; }
        public string? MedicoNombre { get; set; }
    }
}
