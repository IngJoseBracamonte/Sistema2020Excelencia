using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeCajaClosingDeclarations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CajasDeclaracionesMetodos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CajaDiariaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MetodoPagoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MontoIngresado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoVueltos = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoEsperadoIngreso = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoEsperadoVueltos = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiferenciaOriginal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiferenciaBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajasDeclaracionesMetodos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CajasDeclaracionesMetodos_CajasDiarias_CajaDiariaId",
                        column: x => x.CajaDiariaId,
                        principalTable: "CajasDiarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CajasDeclaracionesMetodos_CatalogoMetodosPago_MetodoPagoId",
                        column: x => x.MetodoPagoId,
                        principalTable: "CatalogoMetodosPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CajasDeclaracionesMetodos_CajaDiariaId_MetodoPagoId",
                table: "CajasDeclaracionesMetodos",
                columns: new[] { "CajaDiariaId", "MetodoPagoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CajasDeclaracionesMetodos_MetodoPagoId",
                table: "CajasDeclaracionesMetodos",
                column: "MetodoPagoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CajasDeclaracionesMetodos");
        }
    }
}
