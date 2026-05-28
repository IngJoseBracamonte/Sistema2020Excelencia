using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHonorariosMedicosServicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HonorariosMedicosServicios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MontoHonorario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UsuarioModifico = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HonorariosMedicosServicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HonorariosMedicosServicios_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HonorariosMedicosServicios_ServiciosClinicos_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_MedicoId",
                table: "HonorariosMedicosServicios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_ServicioId",
                table: "HonorariosMedicosServicios",
                column: "ServicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HonorariosMedicosServicios");
        }
    }
}
