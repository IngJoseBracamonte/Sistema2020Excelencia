using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class KardexResultDto
    {
        public decimal InitialBalance { get; set; }
        public decimal TotalEntradas { get; set; }
        public decimal TotalSalidas { get; set; }
        public decimal FinalBalance { get; set; }
        public List<KardexMovimientoDto> Movimientos { get; set; } = new();
    }

    public class KardexMovimientoDto
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public Guid InsumoId { get; set; }
        public string InsumoCodigo { get; set; } = string.Empty;
        public string InsumoNombre { get; set; } = string.Empty;
        public Guid SedeId { get; set; }
        public string SedeNombre { get; set; } = string.Empty;
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal CantidadBase { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public bool EsEntrada { get; set; }
    }
}
