using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixTasaCambioMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TasaCambio",
                table: "TasaCambio");

            migrationBuilder.RenameTable(
                name: "TasaCambio",
                newName: "tasacambio");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tasacambio",
                table: "tasacambio",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tasacambio",
                table: "tasacambio");

            migrationBuilder.RenameTable(
                name: "tasacambio",
                newName: "TasaCambio");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TasaCambio",
                table: "TasaCambio",
                column: "Id");
        }
    }
}
