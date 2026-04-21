using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiciosSugeridosAndLogoBase64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoBase64",
                table: "ConfiguracionGeneral",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "serviciossugerencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioOrigenId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioSugeridoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serviciossugerencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_serviciossugerencias_ServiciosClinicos_ServicioOrigenId",
                        column: x => x.ServicioOrigenId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_serviciossugerencias_ServiciosClinicos_ServicioSugeridoId",
                        column: x => x.ServicioSugeridoId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_serviciossugerencias_ServicioOrigenId",
                table: "serviciossugerencias",
                column: "ServicioOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_serviciossugerencias_ServicioSugeridoId",
                table: "serviciossugerencias",
                column: "ServicioSugeridoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "serviciossugerencias");

            migrationBuilder.DropColumn(
                name: "LogoBase64",
                table: "ConfiguracionGeneral");
        }
    }
}
