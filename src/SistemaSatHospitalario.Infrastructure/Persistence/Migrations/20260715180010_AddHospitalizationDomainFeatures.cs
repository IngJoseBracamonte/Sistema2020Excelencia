using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalizationDomainFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RowVersion",
                table: "StocksSede",
                type: "datetime(6)",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IncluidoEnTarifaBase",
                table: "DetallesServicioCuenta",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCatalogoHistorico",
                table: "DetallesServicioCuenta",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "EsAreaAdmision",
                table: "AreasClinicas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "AreasClinicas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ServicioTarifaBaseId",
                table: "AreasClinicas",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "ServiciosIncluidosAreas",
                columns: table => new
                {
                    AreaClinicaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioClinicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosIncluidosAreas", x => new { x.AreaClinicaId, x.ServicioClinicoId });
                    table.ForeignKey(
                        name: "FK_ServiciosIncluidosAreas_AreasClinicas_AreaClinicaId",
                        column: x => x.AreaClinicaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiciosIncluidosAreas_ServiciosClinicos_ServicioClinicoId",
                        column: x => x.ServicioClinicoId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AreasClinicas_ServicioTarifaBaseId",
                table: "AreasClinicas",
                column: "ServicioTarifaBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosIncluidosAreas_ServicioClinicoId",
                table: "ServiciosIncluidosAreas",
                column: "ServicioClinicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AreasClinicas_ServiciosClinicos_ServicioTarifaBaseId",
                table: "AreasClinicas",
                column: "ServicioTarifaBaseId",
                principalTable: "ServiciosClinicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreasClinicas_ServiciosClinicos_ServicioTarifaBaseId",
                table: "AreasClinicas");

            migrationBuilder.DropTable(
                name: "ServiciosIncluidosAreas");

            migrationBuilder.DropIndex(
                name: "IX_AreasClinicas_ServicioTarifaBaseId",
                table: "AreasClinicas");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "StocksSede");

            migrationBuilder.DropColumn(
                name: "IncluidoEnTarifaBase",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "PrecioCatalogoHistorico",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "EsAreaAdmision",
                table: "AreasClinicas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "AreasClinicas");

            migrationBuilder.DropColumn(
                name: "ServicioTarifaBaseId",
                table: "AreasClinicas");
        }
    }
}
