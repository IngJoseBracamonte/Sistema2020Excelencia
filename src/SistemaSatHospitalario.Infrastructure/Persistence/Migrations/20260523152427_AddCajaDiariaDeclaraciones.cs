using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCajaDiariaDeclaraciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeclaracionCierreJson",
                table: "CajasDiarias",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Diferencia",
                table: "CajasDiarias",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCobrado",
                table: "CajasDiarias",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIngresado",
                table: "CajasDiarias",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeclaracionCierreJson",
                table: "CajasDiarias");

            migrationBuilder.DropColumn(
                name: "Diferencia",
                table: "CajasDiarias");

            migrationBuilder.DropColumn(
                name: "TotalCobrado",
                table: "CajasDiarias");

            migrationBuilder.DropColumn(
                name: "TotalIngresado",
                table: "CajasDiarias");
        }
    }
}
