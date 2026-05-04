using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationshipAndRemoveShadowProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesServicioCuenta_CuentasServicios_CuentaServicioId1",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropIndex(
                name: "IX_DetallesServicioCuenta_CuentaServicioId1",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "CuentaServicioId1",
                table: "DetallesServicioCuenta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CuentaServicioId1",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_CuentaServicioId1",
                table: "DetallesServicioCuenta",
                column: "CuentaServicioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesServicioCuenta_CuentasServicios_CuentaServicioId1",
                table: "DetallesServicioCuenta",
                column: "CuentaServicioId1",
                principalTable: "CuentasServicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
