using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDetalleServicioCuentaUserFks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioCargaId",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioTecnicoId",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_UsuarioCargaId",
                table: "DetallesServicioCuenta",
                column: "UsuarioCargaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_UsuarioTecnicoId",
                table: "DetallesServicioCuenta",
                column: "UsuarioTecnicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DetallesServicioCuenta_UsuarioCargaId",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropIndex(
                name: "IX_DetallesServicioCuenta_UsuarioTecnicoId",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "UsuarioTecnicoId",
                table: "DetallesServicioCuenta");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCargaId",
                table: "DetallesServicioCuenta",
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
