using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class DetallePagoDto
    {
        public string MetodoPago { get; set; } = string.Empty;
        public string ReferenciaBancaria { get; set; } = string.Empty;
        public decimal MontoAbonadoMoneda { get; set; }
        public decimal EquivalenteAbonadoBase { get; set; }
    }
}
