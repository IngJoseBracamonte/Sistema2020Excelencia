using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class PaymentHistoryDto
    {
        public DateTime Fecha { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public decimal MontoBase { get; set; }
        public decimal MontoCambiario { get; set; }
    }

    public class PendingARDto
    {
        public Guid Id { get; set; }
        public Guid CuentaId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCedula { get; set; } = string.Empty;
        public string TipoIngreso { get; set; } = string.Empty;
        public string SeguroNombre { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool IsAudited { get; set; }
        public List<ConceptoFacturadoDto> Conceptos { get; set; } = new();
        public List<PaymentHistoryDto> Pagos { get; set; } = new();
    }
}
