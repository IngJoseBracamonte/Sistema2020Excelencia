using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMonedaTableAndRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Monedas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Simbolo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsBaseUsd = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monedas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Monedas",
                columns: new[] { "Id", "Codigo", "EsBaseUsd", "Nombre", "Simbolo" },
                values: new object[,]
                {
                    { 1, "USD", true, "Dólar", "$" },
                    { 2, "VES", false, "Bolívar", "Bs." },
                    { 3, "EUR", false, "Euro", "€" },
                    { 4, "COP", false, "Peso Colombiano", "COP$" },
                    { 5, "ARS", false, "Peso Argentino", "ARS$" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogoMetodosPago_GrupoMoneda",
                table: "CatalogoMetodosPago",
                column: "GrupoMoneda");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogoMetodosPago_Monedas_GrupoMoneda",
                table: "CatalogoMetodosPago",
                column: "GrupoMoneda",
                principalTable: "Monedas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogoMetodosPago_Monedas_GrupoMoneda",
                table: "CatalogoMetodosPago");

            migrationBuilder.DropTable(
                name: "Monedas");

            migrationBuilder.DropIndex(
                name: "IX_CatalogoMetodosPago_GrupoMoneda",
                table: "CatalogoMetodosPago");
        }
    }
}
