using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Application.UnitTests.Admision
{
    public class FinancialLogicTests
    {
        [Fact]
        public void SplitPayment_WithDifferentRates_ShouldResultInZeroUSDBalance()
        {
            // Scenario: 60$ total. 30$ paid at rate 50. 30$ paid at rate 60.
            // Requirement: Total USD Balance must be 0 regardless of rates.

            // 1. Setup Account
            var cuenta = new CuentaServicios(1, "Particular");
            cuenta.AgregarServicio(Guid.NewGuid(), "Consulta Médica", 60.00m, 1, "MEDICO", "test-user");

            decimal totalCuentaUSD = cuenta.CalcularTotal();
            Assert.Equal(60.00m, totalCuentaUSD);

            // 2. First Payment (30$) - Rate 50
            decimal tasaDia1 = 50.00m;
            decimal montoBs1 = 1500.00m; // 30$ * 50
            decimal equivalenteUSD1 = montoBs1 / tasaDia1;

            var recibo1 = new ReciboFactura(cuenta.Id, 1, Guid.NewGuid(), tasaDia1);
            recibo1.AgregarDetallePago("Punto Venta Bs", "REF123", montoBs1, equivalenteUSD1);

            // 3. Second Payment (30$) - Rate 60
            decimal tasaDia2 = 60.00m;
            decimal montoBs2 = 1800.00m; // 30$ * 60
            decimal equivalenteUSD2 = montoBs2 / tasaDia2;

            var recibo2 = new ReciboFactura(cuenta.Id, 1, Guid.NewGuid(), tasaDia2);
            recibo2.AgregarDetallePago("Transferencia Bs", "REF456", montoBs2, equivalenteUSD2);

            // 4. Verification
            decimal totalPagadoUSD = recibo1.ObtenerTotalPagadoBase() + recibo2.ObtenerTotalPagadoBase();
            decimal saldoRestanteUSD = totalCuentaUSD - totalPagadoUSD;

            Assert.Equal(30.00m, equivalenteUSD1);
            Assert.Equal(30.00m, equivalenteUSD2);
            Assert.Equal(60.00m, totalPagadoUSD);
            Assert.Equal(0.00m, saldoRestanteUSD);
        }

        [Fact]
        public void MixedPayment_DivisasAndBs_ShouldSyncCorrectly()
        {
            // Scenario: 100$ total. 50$ cash. Remaining in Bs at rate 50.
            var cuenta = new CuentaServicios(1, "Particular");
            cuenta.AgregarServicio(Guid.NewGuid(), "Cirugía Menor", 100.00m, 1, "MEDICO", "test-user");

            decimal tasa = 50.00m;
            var recibo = new ReciboFactura(cuenta.Id, 1, Guid.NewGuid(), tasa);

            // Payment 1: 50$ Divisas
            recibo.AgregarDetallePago("Efectivo Divisas", "NA", 50.00m, 50.00m);

            // Payment 2: 2500 Bs (50$ * 50)
            recibo.AgregarDetallePago("Punto Venta Bs", "REF789", 2500.00m, 2500.00m / tasa);

            Assert.Equal(100.00m, recibo.ObtenerTotalPagadoBase());
            Assert.Equal(0.00m, cuenta.CalcularTotal() - recibo.ObtenerTotalPagadoBase());
        }
    }
}
