using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using Xunit;

namespace SistemaSatHospitalario.Tests.Unit.Application
{
    public class ComprasRecepcionTests
    {
        private readonly DbContextOptions<SatHospitalarioDbContext> _options;

        public ComprasRecepcionTests()
        {
            _options = new DbContextOptionsBuilder<SatHospitalarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task RecordPurchase_ShouldPersistPresentacionCompraAndExactKardexQuantityWithoutCalculations()
        {
            // Arrange
            using var context = new SatHospitalarioDbContext(_options);

            var principalSede = new Sede("PRINCIPAL", "Almacén Central", true);
            var propertyInfo = typeof(Sede).GetProperty(nameof(Sede.Id));
            propertyInfo?.SetValue(principalSede, SeedConstants.SedeId_Principal);
            context.Sedes.Add(principalSede);

            var insumo = new Insumo("INS-001", "Vitamina C Ampollas", 0m, UnidadMedida.UNIDAD, 2.50m);
            context.Insumos.Add(insumo);
            await context.SaveChangesAsync();

            // Simular recepción de compra con presentación descriptiva (string) y cantidad Kárdex directa (decimal plano)
            const decimal cantidadKardexDirecta = 10000m;
            const decimal precioCostoUnitarioUSD = 0.25m;
            const string presentacionCompraDescriptiva = "10 Cajas x 1000 Ampollas";
            const string proveedorNombre = "Droguería Central";
            const string numeroFactura = "FAC-2026-001";

            var stockSede = await context.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Principal);
            if (stockSede == null)
            {
                stockSede = new StockSede(insumo.Id, SeedConstants.SedeId_Principal, 0);
                context.StocksSedes.Add(stockSede);
            }

            // Suma directa al Kárdex
            stockSede.RegistrarMovimientoStock(cantidadKardexDirecta, insumo.PermiteFraccionamiento);

            var descPres = !string.IsNullOrWhiteSpace(presentacionCompraDescriptiva) ? $" [Pres: {presentacionCompraDescriptiva.Trim()}]" : "";
            var mov = new MovimientoInsumo(
                insumo.Id,
                SeedConstants.SedeId_Principal,
                "Ingreso",
                cantidadKardexDirecta,
                insumo.UnidadMedidaBase,
                cantidadKardexDirecta,
                "Farmaceutico",
                $"Compra de insumos registrada a costo unitario ${precioCostoUnitarioUSD} USD.{descPres} Prov: {proveedorNombre}"
            );
            context.MovimientosInsumo.Add(mov);

            var ordenCompra = new OrdenCompraInventario(
                numeroFactura,
                proveedorNombre,
                DateTime.UtcNow,
                cantidadKardexDirecta * precioCostoUnitarioUSD,
                50.00m
            );
            context.OrdenesCompraInventario.Add(ordenCompra);

            await context.SaveChangesAsync();

            // Assert
            using var assertContext = new SatHospitalarioDbContext(_options);
            var savedStock = await assertContext.StocksSedes
                .FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SeedConstants.SedeId_Principal);

            savedStock.Should().NotBeNull();
            savedStock!.StockActual.Should().Be(10000m, "la cantidad de stock en Kárdex debe sumarse de forma directa sin multiplicadores ni fórmulas");

            var savedMovimiento = await assertContext.MovimientosInsumo
                .FirstOrDefaultAsync(m => m.InsumoId == insumo.Id);

            savedMovimiento.Should().NotBeNull();
            savedMovimiento!.CantidadBase.Should().Be(10000m);
            savedMovimiento.Motivo.Should().Contain("[Pres: 10 Cajas x 1000 Ampollas]");
            savedMovimiento.Motivo.Should().Contain("Droguería Central");

            var savedOrden = await assertContext.OrdenesCompraInventario
                .FirstOrDefaultAsync(o => o.NumeroFactura == "FAC-2026-001");

            savedOrden.Should().NotBeNull();
            savedOrden!.MontoTotalUSD.Should().Be(2500.00m);
            savedOrden.SaldoPendienteUSD.Should().Be(2500.00m);
        }
    }
}
