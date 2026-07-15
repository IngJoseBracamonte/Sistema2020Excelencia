using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHygieneMonitorAndSpecialistValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MedicoId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "HistorialesLimpiezasCamas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CamaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaInicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioInicio = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioFin = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesLimpiezasCamas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesLimpiezasCamas_AreasClinicas_CamaId",
                        column: x => x.CamaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_MedicoId",
                table: "CuentasServicios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesLimpiezasCamas_CamaId_FechaFin",
                table: "HistorialesLimpiezasCamas",
                columns: new[] { "CamaId", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesLimpiezasCamas_FechaFin",
                table: "HistorialesLimpiezasCamas",
                column: "FechaFin");

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasServicios_Medicos_MedicoId",
                table: "CuentasServicios",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentasServicios_Medicos_MedicoId",
                table: "CuentasServicios");

            migrationBuilder.DropTable(
                name: "HistorialesLimpiezasCamas");

            migrationBuilder.DropIndex(
                name: "IX_CuentasServicios_MedicoId",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "CuentasServicios");
        }
    }
}
