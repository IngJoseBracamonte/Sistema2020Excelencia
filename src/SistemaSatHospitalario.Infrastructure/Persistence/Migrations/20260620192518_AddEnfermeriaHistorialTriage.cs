using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnfermeriaHistorialTriage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {




            migrationBuilder.AlterColumn<decimal>(
                name: "Cantidad",
                table: "DetallesServicioCuenta",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "CuentaPrincipalId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "TriagesEnfermeria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MotivoConsulta = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TensionArterial = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FrecuenciaCardiaca = table.Column<int>(type: "int", nullable: false),
                    FrecuenciaRespiratoria = table.Column<int>(type: "int", nullable: false),
                    Temperatura = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    SaturacionO2 = table.Column<int>(type: "int", nullable: false),
                    GlicemiaCapilar = table.Column<int>(type: "int", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriagesEnfermeria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TriagesEnfermeria_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ValoracionesFisicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EstadoConciencia = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GlasgowOcular = table.Column<int>(type: "int", nullable: false),
                    GlasgowVerbal = table.Column<int>(type: "int", nullable: false),
                    GlasgowMotor = table.Column<int>(type: "int", nullable: false),
                    GlasgowTotal = table.Column<int>(type: "int", nullable: false),
                    ViaAerea = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ventilacion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pulso = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PielMucosas = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LlenadoCapilar = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pupilas = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Alergias = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccesosVenosos = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pertenencias = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AntecedentesMedicos = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValoracionesFisicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValoracionesFisicas_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_CuentaPrincipalId",
                table: "CuentasServicios",
                column: "CuentaPrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_TriagesEnfermeria_CuentaServicioId",
                table: "TriagesEnfermeria",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_TriagesEnfermeria_FechaRegistro",
                table: "TriagesEnfermeria",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionesFisicas_CuentaServicioId",
                table: "ValoracionesFisicas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionesFisicas_FechaRegistro",
                table: "ValoracionesFisicas",
                column: "FechaRegistro");

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasServicios_CuentasServicios_CuentaPrincipalId",
                table: "CuentasServicios",
                column: "CuentaPrincipalId",
                principalTable: "CuentasServicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentasServicios_CuentasServicios_CuentaPrincipalId",
                table: "CuentasServicios");

            migrationBuilder.DropTable(
                name: "TriagesEnfermeria");

            migrationBuilder.DropTable(
                name: "ValoracionesFisicas");

            migrationBuilder.DropIndex(
                name: "IX_CuentasServicios_CuentaPrincipalId",
                table: "CuentasServicios");





            migrationBuilder.DropColumn(
                name: "CuentaPrincipalId",
                table: "CuentasServicios");

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "DetallesServicioCuenta",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);
        }
    }
}
