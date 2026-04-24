using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditAndValidationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioEmision",
                table: "RecibosFacturas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCarga",
                table: "DetallesPago",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAuditoria",
                table: "CuentasServicios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaValidacion",
                table: "CuentasServicios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioAuditoria",
                table: "CuentasServicios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioValidacion",
                table: "CuentasServicios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAuditoria",
                table: "CuentasPorCobrar",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioAuditoria",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioEmision",
                table: "RecibosFacturas");

            migrationBuilder.DropColumn(
                name: "UsuarioCarga",
                table: "DetallesPago");

            migrationBuilder.DropColumn(
                name: "FechaAuditoria",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "FechaValidacion",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "UsuarioAuditoria",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "UsuarioValidacion",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "FechaAuditoria",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "UsuarioAuditoria",
                table: "CuentasPorCobrar");
        }
    }
}
