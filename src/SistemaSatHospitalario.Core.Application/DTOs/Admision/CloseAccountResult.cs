using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CloseAccountResult
    {
        public Guid ReciboId { get; set; }
        public Guid CuentaId { get; set; }
        public Guid? CuentaPorCobrarId { get; set; } // V12.1: Para gestión inmediata de compromisos
        public decimal TotalUsd { get; set; }
        public bool SincronizacionLegacyExitosa { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
