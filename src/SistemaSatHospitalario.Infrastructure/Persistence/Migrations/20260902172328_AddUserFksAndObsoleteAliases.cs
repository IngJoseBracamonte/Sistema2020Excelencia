using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFksAndObsoleteAliases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioEmisionId",
                table: "RecibosFacturas",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioCargaId",
                table: "DetallesPago",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioAuditoriaId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCargaId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioValidacionId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioIdentityId",
                table: "CajasDiarias",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_UsuarioEmisionId",
                table: "RecibosFacturas",
                column: "UsuarioEmisionId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPago_UsuarioCargaId",
                table: "DetallesPago",
                column: "UsuarioCargaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_UsuarioAuditoriaId",
                table: "CuentasServicios",
                column: "UsuarioAuditoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_UsuarioCargaId",
                table: "CuentasServicios",
                column: "UsuarioCargaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_UsuarioValidacionId",
                table: "CuentasServicios",
                column: "UsuarioValidacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CajasDiarias_UsuarioIdentityId",
                table: "CajasDiarias",
                column: "UsuarioIdentityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecibosFacturas_UsuarioEmisionId",
                table: "RecibosFacturas");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPago_UsuarioCargaId",
                table: "DetallesPago");

            migrationBuilder.DropIndex(
                name: "IX_CuentasServicios_UsuarioAuditoriaId",
                table: "CuentasServicios");

            migrationBuilder.DropIndex(
                name: "IX_CuentasServicios_UsuarioCargaId",
                table: "CuentasServicios");

            migrationBuilder.DropIndex(
                name: "IX_CuentasServicios_UsuarioValidacionId",
                table: "CuentasServicios");

            migrationBuilder.DropIndex(
                name: "IX_CajasDiarias_UsuarioIdentityId",
                table: "CajasDiarias");

            migrationBuilder.DropColumn(
                name: "UsuarioEmisionId",
                table: "RecibosFacturas");

            migrationBuilder.DropColumn(
                name: "UsuarioAuditoriaId",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "UsuarioCargaId",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "UsuarioValidacionId",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "UsuarioIdentityId",
                table: "CajasDiarias");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCargaId",
                table: "DetallesPago",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
