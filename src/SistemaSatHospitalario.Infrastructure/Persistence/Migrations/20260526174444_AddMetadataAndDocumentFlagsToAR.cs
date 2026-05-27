using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMetadataAndDocumentFlagsToAR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoctorProcedimiento",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InformacionAdicional",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "QuienAutorizo",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorProcedimiento",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "InformacionAdicional",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "QuienAutorizo",
                table: "CuentasPorCobrar");
        }
    }
}
