using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class PatientHistoryDto
    {
        public Guid CuentaId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string Estado { get; set; }
        public string TipoIngreso { get; set; }
        public decimal Total { get; set; }
        public List<HistoryServiceDetailDto> Servicios { get; set; } = new();
    }

    public class HistoryServiceDetailDto
    {
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string TipoServicio { get; set; }
    }
}
