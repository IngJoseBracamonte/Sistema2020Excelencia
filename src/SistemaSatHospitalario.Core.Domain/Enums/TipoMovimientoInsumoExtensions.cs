using System;

namespace SistemaSatHospitalario.Core.Domain.Enums
{
    public static class TipoMovimientoInsumoExtensions
    {
        public static TipoMovimientoInsumo? Parse(string? value)
        {
            return value?.Trim().ToUpperInvariant() switch
            {
                "ENV" => TipoMovimientoInsumo.Consumo,
                "INGRESO" => TipoMovimientoInsumo.Ingreso,
                "DESCARTE" => TipoMovimientoInsumo.Descarte,
                "CONSUMO" => TipoMovimientoInsumo.Consumo,
                "AJUSTECIERRE" => TipoMovimientoInsumo.AjusteCierre,
                "TRANSFERENCIAENTRADA" => TipoMovimientoInsumo.TransferenciaEntrada,
                "TRANSFERENCIASALIDA" => TipoMovimientoInsumo.TransferenciaSalida,
                "DEVOLUCION" => TipoMovimientoInsumo.Devolucion,
                "ENVIOINTERNO" => TipoMovimientoInsumo.EnvioInterno,
                _ => null
            };
        }
    }
}