using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHonorariumBaseToCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HonorarioBase",
                table: "ServiciosClinicos",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Honorario",
                table: "DetallesServicioCuenta",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HonorarioAnterior",
                table: "AuditLogsPrecios",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NuevoHonorario",
                table: "AuditLogsPrecios",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HonorarioBase",
                table: "ServiciosClinicos");

            migrationBuilder.DropColumn(
                name: "Honorario",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "HonorarioAnterior",
                table: "AuditLogsPrecios");

            migrationBuilder.DropColumn(
                name: "NuevoHonorario",
                table: "AuditLogsPrecios");
        }
    }
}
