using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAdmisiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Admision");

            migrationBuilder.CreateTable(
                name: "CajasDiarias",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MontoInicialDivisa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoInicialBs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajasDiarias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncidenciasHorario",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreadoPor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenciasHorario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PacientesAdmision",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CedulaPasaporte = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NombreCorto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonoContact = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacientesAdmision", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistroAuditoriaIncidencias",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnoMedicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidenciaIgnoradaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaTraza = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAuditoriaIncidencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SegurosConvenios",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeCobertura = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegurosConvenios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TurnosMedicos",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaHoraToma = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IgnorandoIncidencia = table.Column<bool>(type: "bit", nullable: false),
                    IncidenciaIgnoradaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnosMedicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TurnosCajero",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CajaDiariaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CajeroUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraFin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnosCajero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnosCajero_CajasDiarias_CajaDiariaId",
                        column: x => x.CajaDiariaId,
                        principalSchema: "Admision",
                        principalTable: "CajasDiarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesDeServicio",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroLlegadaDiario = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombrePaciente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoIngreso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoFacturacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalCobrado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConvenioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    EstudioSolicitado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Procesada = table.Column<bool>(type: "bit", nullable: true),
                    FechaProcesada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AsistenteRxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeServicio_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalSchema: "Admision",
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecibosFacturas",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdenServicioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnoCajeroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NroControlFiscal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TasaCambioDia = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EstadoFiscal = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecibosFacturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecibosFacturas_OrdenesDeServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalSchema: "Admision",
                        principalTable: "OrdenesDeServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecibosFacturas_TurnosCajero_TurnoCajeroId",
                        column: x => x.TurnoCajeroId,
                        principalSchema: "Admision",
                        principalTable: "TurnosCajero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DetallesPago",
                schema: "Admision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReciboFacturaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenciaBancaria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MontoAbonadoMoneda = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EquivalenteAbonadoBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesPago_RecibosFacturas_ReciboFacturaId",
                        column: x => x.ReciboFacturaId,
                        principalSchema: "Admision",
                        principalTable: "RecibosFacturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPago_ReciboFacturaId",
                schema: "Admision",
                table: "DetallesPago",
                column: "ReciboFacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_PacienteId",
                schema: "Admision",
                table: "OrdenesDeServicio",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PacientesAdmision_CedulaPasaporte",
                schema: "Admision",
                table: "PacientesAdmision",
                column: "CedulaPasaporte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_OrdenServicioId",
                schema: "Admision",
                table: "RecibosFacturas",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_TurnoCajeroId",
                schema: "Admision",
                table: "RecibosFacturas",
                column: "TurnoCajeroId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosCajero_CajaDiariaId",
                schema: "Admision",
                table: "TurnosCajero",
                column: "CajaDiariaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesPago",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "IncidenciasHorario",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "RegistroAuditoriaIncidencias",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "SegurosConvenios",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "TurnosMedicos",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "RecibosFacturas",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "OrdenesDeServicio",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "TurnosCajero",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "PacientesAdmision",
                schema: "Admision");

            migrationBuilder.DropTable(
                name: "CajasDiarias",
                schema: "Admision");
        }
    }
}
