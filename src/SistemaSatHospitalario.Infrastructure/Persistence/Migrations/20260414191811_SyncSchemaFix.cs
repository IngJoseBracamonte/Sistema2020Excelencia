using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchemaFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EspecialidadId",
                table: "ServiciosClinicos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "PacientesAdmision",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntervaloTurnoMinutos",
                table: "Medicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRealizacion",
                table: "DetallesServicioCuenta",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Realizado",
                table: "DetallesServicioCuenta",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioTecnico",
                table: "DetallesServicioCuenta",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "FacturarLaboratorio",
                table: "ConfiguracionGeneral",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarDetalleFacturacion",
                table: "ConfiguracionGeneral",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "HorariosAtencionMedicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosAtencionMedicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosAtencionMedicos_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosClinicos_EspecialidadId",
                table: "ServiciosClinicos",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosAtencionMedicos_MedicoId",
                table: "HorariosAtencionMedicos",
                column: "MedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosClinicos_Especialidades_EspecialidadId",
                table: "ServiciosClinicos",
                column: "EspecialidadId",
                principalTable: "Especialidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosClinicos_Especialidades_EspecialidadId",
                table: "ServiciosClinicos");

            migrationBuilder.DropTable(
                name: "HorariosAtencionMedicos");

            migrationBuilder.DropIndex(
                name: "IX_ServiciosClinicos_EspecialidadId",
                table: "ServiciosClinicos");

            migrationBuilder.DropColumn(
                name: "EspecialidadId",
                table: "ServiciosClinicos");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "PacientesAdmision");

            migrationBuilder.DropColumn(
                name: "IntervaloTurnoMinutos",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "FechaRealizacion",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "Realizado",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "UsuarioTecnico",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "FacturarLaboratorio",
                table: "ConfiguracionGeneral");

            migrationBuilder.DropColumn(
                name: "MostrarDetalleFacturacion",
                table: "ConfiguracionGeneral");
        }
    }
}
