using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHonorarioConfigYAsignacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoriaHonorario",
                table: "DetallesServicioCuenta",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "CuentaServicioId1",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "MedicoResponsableId",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "HonorariosConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CategoriaServicio = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoDefaultId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioConfiguro = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaConfiguracion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NotasConfig = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HonorariosConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HonorariosConfig_Medicos_MedicoDefaultId",
                        column: x => x.MedicoDefaultId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LogsAsignacionHonorario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NombreServicio = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoAccion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoAnteriorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    MedicoAnteriorNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoNuevoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    MedicoNuevoNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioOperador = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaAccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAsignacionHonorario", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_CuentaServicioId1",
                table: "DetallesServicioCuenta",
                column: "CuentaServicioId1");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_MedicoResponsableId",
                table: "DetallesServicioCuenta",
                column: "MedicoResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosConfig_CategoriaServicio",
                table: "HonorariosConfig",
                column: "CategoriaServicio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosConfig_MedicoDefaultId",
                table: "HonorariosConfig",
                column: "MedicoDefaultId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_DetalleServicioId",
                table: "LogsAsignacionHonorario",
                column: "DetalleServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_FechaAccion",
                table: "LogsAsignacionHonorario",
                column: "FechaAccion");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesServicioCuenta_CuentasServicios_CuentaServicioId1",
                table: "DetallesServicioCuenta",
                column: "CuentaServicioId1",
                principalTable: "CuentasServicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesServicioCuenta_Medicos_MedicoResponsableId",
                table: "DetallesServicioCuenta",
                column: "MedicoResponsableId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesServicioCuenta_CuentasServicios_CuentaServicioId1",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesServicioCuenta_Medicos_MedicoResponsableId",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropTable(
                name: "HonorariosConfig");

            migrationBuilder.DropTable(
                name: "LogsAsignacionHonorario");

            migrationBuilder.DropIndex(
                name: "IX_DetallesServicioCuenta_CuentaServicioId1",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropIndex(
                name: "IX_DetallesServicioCuenta_MedicoResponsableId",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "CategoriaHonorario",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "CuentaServicioId1",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "MedicoResponsableId",
                table: "DetallesServicioCuenta");
        }
    }
}
