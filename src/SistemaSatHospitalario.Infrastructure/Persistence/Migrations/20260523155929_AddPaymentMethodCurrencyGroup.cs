using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodCurrencyGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TasaCambioAplicada",
                table: "DetallesPago",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "GrupoMoneda",
                table: "CatalogoMetodosPago",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Migración de datos históricos
            migrationBuilder.Sql("UPDATE CatalogoMetodosPago SET GrupoMoneda = 1 WHERE EsUSD = 1;");
            migrationBuilder.Sql("UPDATE CatalogoMetodosPago SET GrupoMoneda = 2 WHERE EsUSD = 0;");
            migrationBuilder.Sql("UPDATE DetallesPago dp INNER JOIN RecibosFacturas rf ON dp.ReciboFacturaId = rf.Id SET dp.TasaCambioAplicada = rf.TasaCambioDia;");
            migrationBuilder.Sql("UPDATE DetallesPago dp INNER JOIN CatalogoMetodosPago mp ON (dp.MetodoPago = mp.Valor OR dp.MetodoPago = mp.Nombre) SET dp.TasaCambioAplicada = 1.0 WHERE mp.GrupoMoneda = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TasaCambioAplicada",
                table: "DetallesPago");

            migrationBuilder.DropColumn(
                name: "GrupoMoneda",
                table: "CatalogoMetodosPago");
        }
    }
}
