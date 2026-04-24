using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTelefonoToMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Medicos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "CompromisoGenerado",
                table: "CuentasPorCobrar",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_PacienteId",
                table: "CuentasServicios",
                column: "PacienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasServicios_PacientesAdmision_PacienteId",
                table: "CuentasServicios",
                column: "PacienteId",
                principalTable: "PacientesAdmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentasServicios_PacientesAdmision_PacienteId",
                table: "CuentasServicios");

            migrationBuilder.DropIndex(
                name: "IX_CuentasServicios_PacienteId",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "CompromisoGenerado",
                table: "CuentasPorCobrar");
        }
    }
}
