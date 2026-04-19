using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CloseAccountResult
    {
        public Guid ReciboId { get; set; }
        public Guid CuentaId { get; set; }
        public decimal TotalUsd { get; set; }
        public bool SincronizacionLegacyExitosa { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
