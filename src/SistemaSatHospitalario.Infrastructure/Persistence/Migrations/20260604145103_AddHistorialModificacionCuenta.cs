using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHistorialModificacionCuenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumeroComprobante",
                table: "RecibosFacturas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistorialModificacionCuentas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Usuario = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PacienteAnteriorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PacienteAnteriorNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PacienteNuevoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PacienteNuevoNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoIngresoAnterior = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoIngresoNuevo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConvenioAnteriorId = table.Column<int>(type: "int", nullable: true),
                    ConvenioAnteriorNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConvenioNuevoId = table.Column<int>(type: "int", nullable: true),
                    ConvenioNuevoNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboTotalAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboTotalNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboVueltoAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboVueltoNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboPagadoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CxCSaldoAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CxCSaldoNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DetalleServiciosCambiosJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialModificacionCuentas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_CuentaServicioId",
                table: "HistorialModificacionCuentas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_FechaModificacion",
                table: "HistorialModificacionCuentas",
                column: "FechaModificacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialModificacionCuentas");

            migrationBuilder.DropColumn(
                name: "NumeroComprobante",
                table: "RecibosFacturas");
        }
    }
}
