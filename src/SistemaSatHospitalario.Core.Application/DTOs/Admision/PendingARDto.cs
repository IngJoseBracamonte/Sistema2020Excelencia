using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class PendingARDto
    {
        public Guid Id { get; set; }
        public Guid CuentaId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteCedula { get; set; }
        public string TipoIngreso { get; set; }
        public string SeguroNombre { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; }
    }
}
