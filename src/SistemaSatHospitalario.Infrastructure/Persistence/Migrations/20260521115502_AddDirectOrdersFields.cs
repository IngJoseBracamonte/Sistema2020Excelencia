using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectOrdersFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsDirecta",
                table: "OrdenesImagenes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaValidacion",
                table: "OrdenesImagenes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MedicoSolicitanteId",
                table: "OrdenesImagenes",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "MedicoSolicitanteNombre",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "RequiereValidacion",
                table: "OrdenesImagenes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Validada",
                table: "OrdenesImagenes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ValidadorPor",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsDirecta",
                table: "OrdenesImagenes");

            migrationBuilder.DropColumn(
                name: "FechaValidacion",
                table: "OrdenesImagenes");

            migrationBuilder.DropColumn(
                name: "MedicoSolicitanteId",
                table: "OrdenesImagenes");

            migrationBuilder.DropColumn(
                name: "MedicoSolicitanteNombre",
                table: "OrdenesImagenes");

            migrationBuilder.DropColumn(
                name: "RequiereValidacion",
                table: "OrdenesImagenes");

            migrationBuilder.DropColumn(
                name: "Validada",
                table: "OrdenesImagenes");

            migrationBuilder.DropColumn(
                name: "ValidadorPor",
                table: "OrdenesImagenes");
        }
    }
}
