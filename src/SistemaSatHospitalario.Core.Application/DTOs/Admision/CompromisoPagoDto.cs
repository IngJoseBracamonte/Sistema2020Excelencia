using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CompromisoPagoDto
    {
        public string NombreResponsable { get; set; }
        public string RelacionResponsable { get; set; }
        public string CedulaResponsable { get; set; }
        public string DireccionResponsable { get; set; }
        public string TelefonoResponsable { get; set; }
        public Guid? CuentaPorCobrarId { get; set; }
        
        public string Conceptos { get; set; } // Ej: HC, Rx Torax
        
        public string NombrePaciente { get; set; }
        public int EdadPaciente { get; set; }
        public string CedulaPaciente { get; set; }
        
        public decimal MontoTotal { get; set; }
        public int DiasLiquidar { get; set; }
        public int Cuotas { get; set; }
        public decimal MontoGarantia { get; set; }
        public string? DescripcionGarantia { get; set; }
        public DateTime FechaCompromiso { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}
