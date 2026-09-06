using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHonorarioConfigUsuarioFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioConfiguroId",
                table: "HonorariosConfig",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosConfig_UsuarioConfiguroId",
                table: "HonorariosConfig",
                column: "UsuarioConfiguroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HonorariosConfig_UsuarioConfiguroId",
                table: "HonorariosConfig");

            migrationBuilder.DropColumn(
                name: "UsuarioConfiguroId",
                table: "HonorariosConfig");
        }
    }
}
