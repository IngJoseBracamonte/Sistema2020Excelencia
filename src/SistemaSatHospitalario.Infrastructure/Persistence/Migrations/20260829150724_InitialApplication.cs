using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OldValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogsPrecios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DescripcionServicio = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecioOriginal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioModificado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HonorarioAnterior = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NuevoHonorario = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    UsuarioOperador = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AutorizadoPor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogsPrecios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BloqueosHorarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoraPautada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Motivo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloqueosHorarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CajasDiarias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaApertura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MontoInicialDivisa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoInicialBs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreUsuario = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeclaracionCierreJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalIngresado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalCobrado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Diferencia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajasDiarias", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CategoriasInsumo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasInsumo", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConfiguracionGeneral",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NombreEmpresa = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rif = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Iva = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ClaveSupervisor = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FacturarLaboratorio = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MostrarDetalleFacturacion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LogoBase64 = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionGeneral", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DocumentLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DocumentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ErrorTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RequestPath = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetodoHTTP = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MensajeExcepcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StackTrace = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioAsociado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Resuelto = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ComentariosResolucion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaResolucion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ResueltoPor = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorTickets", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistorialModificacionCuentas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Usuario = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PacienteAnteriorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PacienteAnteriorNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PacienteNuevoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PacienteNuevoNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoIngresoAnterior = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoIngresoNuevo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConvenioAnteriorId = table.Column<int>(type: "int", nullable: true),
                    ConvenioAnteriorNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConvenioNuevoId = table.Column<int>(type: "int", nullable: true),
                    ConvenioNuevoNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboTotalAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboTotalNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboVueltoAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboVueltoNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboPagadoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CxCSaldoAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CxCSaldoNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DetalleServiciosCambiosJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialModificacionCuentas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HonorariumMappingRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Pattern = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MatchType = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UsuarioCreo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HonorariumMappingRules", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "IncidenciasHorario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Inicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreadoPor = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenciasHorario", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Insumos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnidadMedidaBase = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostoUnitarioBaseUSD = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PermiteFraccionamiento = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    Categoria = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Medicamento")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    FechaInactivacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OcultoEnTraslados = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    ReactivosCombinados = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Indicaciones = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetUserId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetRole = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ActionUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrdenesCompraInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NumeroFactura = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProveedorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ProveedorNombre = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaEmision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MontoTotalUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoTotalBs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAbonadoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SaldoPendienteUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCompraInventario", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PacientesAdmision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IdPacienteLegacy = table.Column<int>(type: "int", nullable: true),
                    CedulaPasaporte = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreCorto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TelefonoContact = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Direccion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacientesAdmision", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PrincipiosActivos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipiosActivos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RIF = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RazonSocial = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RegistroAuditoriaIncidencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TurnoMedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IncidenciaIgnoradaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OperadorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaTraza = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Motivo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAuditoriaIncidencias", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RequisitosCirugia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsActivo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosCirugia", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReservasTemporales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoraPautada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comentario = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiracionUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasTemporales", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sedes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsPrincipal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SegurosConvenios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rtn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegurosConvenios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TasaCambio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasaCambio", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TiposServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposServicio", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TurnosMedicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaHoraToma = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IgnorandoIncidencia = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IncidenciaIgnoradaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnosMedicos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EspecialidadId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HonorarioBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IntervaloTurnoMinutos = table.Column<int>(type: "int", nullable: false),
                    Telefono = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicos_Especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "Especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogoMetodosPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Valor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsUSD = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EsVuelto = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    GrupoMoneda = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogoMetodosPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogoMetodosPago_Monedas_GrupoMoneda",
                        column: x => x.GrupoMoneda,
                        principalTable: "Monedas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PagosProveedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCompraId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaPago = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MontoAbonadoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TasaCambio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoAbonadoBs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MetodoPago = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Referencia = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosProveedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosProveedores_OrdenesCompraInventario_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalTable: "OrdenesCompraInventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrdenesDeServicio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NumeroLlegadaDiario = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NombrePaciente = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoIngreso = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoFacturacion = table.Column<int>(type: "int", nullable: false),
                    TotalCobrado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ConvenioId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstudioSolicitado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Procesada = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    FechaProcesada = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AsistenteRxId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeServicio_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InsumosPrincipiosActivos",
                columns: table => new
                {
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PrincipioActivoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Concentracion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsumosPrincipiosActivos", x => new { x.InsumoId, x.PrincipioActivoId });
                    table.ForeignKey(
                        name: "FK_InsumosPrincipiosActivos_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsumosPrincipiosActivos_PrincipiosActivos_PrincipioActivoId",
                        column: x => x.PrincipioActivoId,
                        principalTable: "PrincipiosActivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CierresInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaCierre = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Usuario = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierresInventario_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MovimientosInsumo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TipoMovimiento = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    CantidadBase = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnidadMedidaOriginal = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CantidadOriginal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Usuario = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInsumo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosInsumo_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInsumo_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PedidosInterSede",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Correlativo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SedeSolicitanteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeProveedoraId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaDespacho = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioCreador = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosInterSede", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosInterSede_Sedes_SedeProveedoraId",
                        column: x => x.SedeProveedoraId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidosInterSede_Sedes_SedeSolicitanteId",
                        column: x => x.SedeSolicitanteId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StocksSede",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StockActual = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    StockMaximo = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    RowVersion = table.Column<DateTime>(type: "datetime(6)", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StocksSede", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StocksSede_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StocksSede_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransferenciasReposicionStock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeOrigenId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeDestinoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Motivo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaTransferencia = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferenciasReposicionStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferenciasReposicionStock_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferenciasReposicionStock_Sedes_SedeDestinoId",
                        column: x => x.SedeDestinoId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferenciasReposicionStock_Sedes_SedeOrigenId",
                        column: x => x.SedeOrigenId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConvenioPerfilPrecios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SeguroConvenioId = table.Column<int>(type: "int", nullable: false),
                    PerfilId = table.Column<int>(type: "int", nullable: false),
                    PrecioHNL = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvenioPerfilPrecios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvenioPerfilPrecios_SegurosConvenios_SeguroConvenioId",
                        column: x => x.SeguroConvenioId,
                        principalTable: "SegurosConvenios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ServiciosClinicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Codigo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecioBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HonorarioBase = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TipoServicio = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoServicioId = table.Column<int>(type: "int", nullable: false),
                    LegacyMappingId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<int>(type: "int", nullable: false),
                    HonorariumCategory = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PermiteFraccionamiento = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequiereInventario = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EspecialidadId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ServicioInformeId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    EsServicioInforme = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DesactivadoPorUsuarioId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaDesactivacion = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosClinicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiciosClinicos_Especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "Especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiciosClinicos_ServiciosClinicos_ServicioInformeId",
                        column: x => x.ServicioInformeId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiciosClinicos_TiposServicio_TipoServicioId",
                        column: x => x.TipoServicioId,
                        principalTable: "TiposServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "OrdenesImagenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CuentaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PacienteNombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estudio = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoServicio = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ProcesadoPor = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaProcesado = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EsDirecta = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequiereValidacion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Validada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ValidadorPor = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaValidacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MedicoSolicitanteId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    MedicoSolicitanteNombre = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Informe = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkInforme = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObservacionesMedico = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoInterpreteId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    RequiereInforme = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesImagenes_Medicos_MedicoSolicitanteId",
                        column: x => x.MedicoSolicitanteId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdenesImagenes_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CierresInventarioDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CierreInventarioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StockTeoricoBase = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    StockRealBase = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CostoBaseUSD = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresInventarioDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierresInventarioDetalles_CierresInventario_CierreInventario~",
                        column: x => x.CierreInventarioId,
                        principalTable: "CierresInventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CierresInventarioDetalles_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PedidosInterSedeDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PedidoInterSedeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CantidadSolicitada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CantidadDespachada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CantidadRecibida = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ObservacionDespacho = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosInterSedeDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosInterSedeDetalles_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidosInterSedeDetalles_PedidosInterSede_PedidoInterSedeId",
                        column: x => x.PedidoInterSedeId,
                        principalTable: "PedidosInterSede",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AreasClinicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EsSubAreaAlmacenPrincipal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AreaPadreId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EsAreaAdmision = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ServicioTarifaBaseId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreasClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreasClinicas_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreasClinicas_ServiciosClinicos_ServicioTarifaBaseId",
                        column: x => x.ServicioTarifaBaseId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "PreciosServicioConvenio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioClinicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SeguroConvenioId = table.Column<int>(type: "int", nullable: false),
                    PrecioDiferencial = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreciosServicioConvenio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreciosServicioConvenio_SegurosConvenios_SeguroConvenioId",
                        column: x => x.SeguroConvenioId,
                        principalTable: "SegurosConvenios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreciosServicioConvenio_ServiciosClinicos_ServicioClinicoId",
                        column: x => x.ServicioClinicoId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ServiciosInsumoRecetas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioClinicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioCodigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnidadMedidaConsumo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosInsumoRecetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiciosInsumoRecetas_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiciosInsumoRecetas_ServiciosClinicos_ServicioClinicoId",
                        column: x => x.ServicioClinicoId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
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

            migrationBuilder.CreateTable(
                name: "CuentasServicios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaPrincipalId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioCarga = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCarga = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoIngreso = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConvenioId = table.Column<int>(type: "int", nullable: true),
                    LegacyOrderId = table.Column<int>(type: "int", nullable: true),
                    ProcesamientoEstado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AreaClinicaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SubAreaClinica = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CamaRetenidaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioValidacion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaValidacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioAuditoria = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaAuditoria = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DestinoPaciente = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PersonalRelevo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasServicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_AreasClinicas_AreaClinicaId",
                        column: x => x.AreaClinicaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_AreasClinicas_CamaRetenidaId",
                        column: x => x.CamaRetenidaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_CuentasServicios_CuentaPrincipalId",
                        column: x => x.CuentaPrincipalId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_SegurosConvenios_ConvenioId",
                        column: x => x.ConvenioId,
                        principalTable: "SegurosConvenios",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "CitasMedicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoraPautada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comentario = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AreaClinicaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitasMedicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitasMedicas_AreasClinicas_AreaClinicaId",
                        column: x => x.AreaClinicaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CitasMedicas_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitasMedicas_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CuentasPorCobrar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MontoTotalBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoPagadoBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAudited = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UsuarioAuditoria = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaAuditoria = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CompromisoGenerado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GarantiaGenerada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    QuienAutorizo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DoctorProcedimiento = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InformacionAdicional = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasPorCobrar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasPorCobrar_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DetallesServicioCuenta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Honorario = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TipoServicio = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoServicioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCarga = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioCargaId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCarga = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LegacyMappingId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoResponsableId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CategoriaHonorario = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AreaClinicaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DetallePadreId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IncluidoEnTarifaBase = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PrecioCatalogoHistorico = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Realizado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRealizacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioTecnico = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesServicioCuenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesServicioCuenta_AreasClinicas_AreaClinicaId",
                        column: x => x.AreaClinicaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DetallesServicioCuenta_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesServicioCuenta_DetallesServicioCuenta_DetallePadreId",
                        column: x => x.DetallePadreId,
                        principalTable: "DetallesServicioCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesServicioCuenta_Medicos_MedicoResponsableId",
                        column: x => x.MedicoResponsableId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DetallesServicioCuenta_TiposServicio_TipoServicioId",
                        column: x => x.TipoServicioId,
                        principalTable: "TiposServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrdenesCirugia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AreaClinicaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SedeQuirofanoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AreaClinicaOrigenId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SedeOrigenId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DescripcionCirugia = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecioBaseUsd = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioDerechoSalaUsd = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaHoraProgramada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MotivoCancelacion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioCreacion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SalaQuirofano = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModalidadAnestesia = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsAlquilado = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCirugia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_AreasClinicas_AreaClinicaId",
                        column: x => x.AreaClinicaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_AreasClinicas_AreaClinicaOrigenId",
                        column: x => x.AreaClinicaOrigenId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_Sedes_SedeOrigenId",
                        column: x => x.SedeOrigenId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_Sedes_SedeQuirofanoId",
                        column: x => x.SedeQuirofanoId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RecibosFacturas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CajaDiariaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    NroControlFiscal = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TasaCambioDia = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EstadoFiscal = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NumeroRecibo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroComprobante = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalFacturadoUSD = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MontoVueltoUSD = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioEmision = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecibosFacturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecibosFacturas_CajasDiarias_CajaDiariaId",
                        column: x => x.CajaDiariaId,
                        principalTable: "CajasDiarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RecibosFacturas_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionRapida = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionDetallada = table.Column<string>(type: "longtext", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "CompromisosPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaPorCobrarId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Omitido = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Observacion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioCreacion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompromisosPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompromisosPago_CuentasPorCobrar_CuentaPorCobrarId",
                        column: x => x.CuentaPorCobrarId,
                        principalTable: "CuentasPorCobrar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GarantiasItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaPorCobrarId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValorEstimado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarantiasItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarantiasItems_CuentasPorCobrar_CuentaPorCobrarId",
                        column: x => x.CuentaPorCobrarId,
                        principalTable: "CuentasPorCobrar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConsumosServiciosRealizados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioCuentaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CantidadConsumidaBase = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CostoTotalUSD = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FechaConsumo = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumosServiciosRealizados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumosServiciosRealizados_DetallesServicioCuenta_DetalleSe~",
                        column: x => x.DetalleServicioCuentaId,
                        principalTable: "DetallesServicioCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsumosServiciosRealizados_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DetallesServiciosMedicosResponsables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioCuentaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Rol = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontoHonorario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesServiciosMedicosResponsables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesServiciosMedicosResponsables_DetallesServicioCuenta_~",
                        column: x => x.DetalleServicioCuentaId,
                        principalTable: "DetallesServicioCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesServiciosMedicosResponsables_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CirugiaLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UsuarioId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Evento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Detalle = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CirugiaLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CirugiaLogs_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CirugiasMedicosHonorarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EspecialidadId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MontoHonorarioUsd = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EsCirujanoPrincipal = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CirugiasMedicosHonorarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CirugiasMedicosHonorarios_Especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "Especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CirugiasMedicosHonorarios_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CirugiasMedicosHonorarios_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CirugiasObservacionesHistorial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Observacion = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioRegistroId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CirugiasObservacionesHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CirugiasObservacionesHistorial_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InsumosCirugiasPacientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CantidadEntregada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CantidadDevuelta = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsumosCirugiasPacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsumosCirugiasPacientes_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsumosCirugiasPacientes_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsumosCirugiasPacientes_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrdenesCirugiaRequisitos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RequisitoCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Cumplido = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaVerificacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VerificadoPor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCirugiaRequisitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugiaRequisitos_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugiaRequisitos_RequisitosCirugia_RequisitoCirugiaId",
                        column: x => x.RequisitoCirugiaId,
                        principalTable: "RequisitosCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SolicitudesInsumosCirugia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CantidadSolicitada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AlmacenOrigenId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EstadoSolicitud = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioSolicitud = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaDespacho = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioDespacho = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesInsumosCirugia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesInsumosCirugia_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesInsumosCirugia_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudesInsumosCirugia_Sedes_AlmacenOrigenId",
                        column: x => x.AlmacenOrigenId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DetallesPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ReciboFacturaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MetodoPago = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetodoPagoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ReferenciaBancaria = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontoAbonadoMoneda = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EquivalenteAbonadoBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TasaCambioAplicada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioCarga = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioCargaId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesPago_CatalogoMetodosPago_MetodoPagoId",
                        column: x => x.MetodoPagoId,
                        principalTable: "CatalogoMetodosPago",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DetallesPago_RecibosFacturas_ReciboFacturaId",
                        column: x => x.ReciboFacturaId,
                        principalTable: "RecibosFacturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "CategoriasInsumo",
                columns: new[] { "Id", "Activo", "Codigo", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), true, "MED", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Medicamento" },
                    { new Guid("50000000-0000-0000-0000-000000000002"), true, "DESC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Descartable" },
                    { new Guid("50000000-0000-0000-0000-000000000003"), true, "MAT-MED", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Material Médico" },
                    { new Guid("50000000-0000-0000-0000-000000000004"), true, "REACT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reactivo" },
                    { new Guid("50000000-0000-0000-0000-000000000005"), true, "MAT-QX", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Material Quirúrgico" },
                    { new Guid("50000000-0000-0000-0000-000000000006"), true, "OTRO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Otro" }
                });

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

            migrationBuilder.InsertData(
                table: "RequisitosCirugia",
                columns: new[] { "Id", "Descripcion", "EsActivo", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), "Informe de cardiología y electrocardiograma vigente.", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Evaluación Cardiovascular / Riesgo Quirúrgico" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "Hematología completa, TP, TPT, Glucemia, Urea, Creatinina y VIH/VDRL.", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exámenes Preoperatorios (Laboratorio)" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "Firma del paciente o familiar responsable para procedimiento quirúrgico y anestesia.", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Consentimiento Informado Firmado" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "Verificación por enfermería de ayuno estricto.", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ayuno Verificado (Mínimo 8 Horas)" },
                    { new Guid("40000000-0000-0000-0000-000000000005"), "Aprobación formal firmada por el médico anestesiólogo.", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Valoración Anestésica" },
                    { new Guid("40000000-0000-0000-0000-000000000006"), "Disponibilidad confirmada con Banco de Sangre (cuando aplique).", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reserva de Sangre / Hemoderivados" },
                    { new Guid("40000000-0000-0000-0000-000000000007"), "Cama confirmada para el traslado post-quirúrgico.", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Disponibilidad de Cama Postoperatoria (UCI / Hosp)" }
                });

            migrationBuilder.InsertData(
                table: "Sedes",
                columns: new[] { "Id", "Activo", "Codigo", "EsPrincipal", "Nombre" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), true, "SEDE-PRINCIPAL", true, "Almacén Principal / Farmacia Central" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), true, "SEDE-EMG", false, "Depósito Emergencia" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), true, "SEDE-HOSP", false, "Depósito Hospitalización" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), true, "SEDE-UCI", false, "Depósito UCI" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), true, "SEDE-CIRUGIA", false, "Quirófano / Pabellón Central" }
                });

            migrationBuilder.InsertData(
                table: "TiposServicio",
                columns: new[] { "Id", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, "MEDICO", "Servicio Médico / Consulta" },
                    { 2, "LAB", "Examen de Laboratorio" },
                    { 3, "RX", "Rayos X / Imagenología" },
                    { 4, "TOMO", "Tomografía Axial" },
                    { 5, "INSUMO", "Insumo / Medicamento" },
                    { 6, "INFORME", "Informe / Lectura Médica" }
                });

            migrationBuilder.InsertData(
                table: "AreasClinicas",
                columns: new[] { "Id", "Activo", "AreaPadreId", "Codigo", "EsAreaAdmision", "EsSubAreaAlmacenPrincipal", "Estado", "Nombre", "SedeId", "ServicioTarifaBaseId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), true, null, "BOX-1", true, false, 1, "Box Emergencia 1", new Guid("10000000-0000-0000-0000-000000000002"), null },
                    { new Guid("30000000-0000-0000-0000-000000000002"), true, null, "HAB-101", false, false, 1, "Habitación 101", new Guid("10000000-0000-0000-0000-000000000003"), null },
                    { new Guid("30000000-0000-0000-0000-000000000003"), true, null, "UCI-1", false, false, 1, "Cama UCI 1", new Guid("10000000-0000-0000-0000-000000000004"), null },
                    { new Guid("30000000-0000-0000-0000-000000000004"), true, null, "FARMACIA", false, false, 1, "Farmacia Central", new Guid("10000000-0000-0000-0000-000000000001"), null },
                    { new Guid("30000000-0000-0000-0000-000000000005"), true, null, "LABORATORIO", false, false, 1, "Laboratorio Central", new Guid("10000000-0000-0000-0000-000000000001"), null },
                    { new Guid("30000000-0000-0000-0000-000000000006"), true, null, "QX-1", false, false, 1, "Quirófano 1 (Cirugía Mayor)", new Guid("10000000-0000-0000-0000-000000000005"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreasClinicas_SedeId_Codigo",
                table: "AreasClinicas",
                columns: new[] { "SedeId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AreasClinicas_ServicioTarifaBaseId",
                table: "AreasClinicas",
                column: "ServicioTarifaBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueosHorarios_MedicoId_HoraPautada",
                table: "BloqueosHorarios",
                columns: new[] { "MedicoId", "HoraPautada" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogoMetodosPago_GrupoMoneda",
                table: "CatalogoMetodosPago",
                column: "GrupoMoneda");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogoMetodosPago_Valor",
                table: "CatalogoMetodosPago",
                column: "Valor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasInsumo_Nombre",
                table: "CategoriasInsumo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventario_SedeId",
                table: "CierresInventario",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventarioDetalles_CierreInventarioId",
                table: "CierresInventarioDetalles",
                column: "CierreInventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventarioDetalles_InsumoId",
                table: "CierresInventarioDetalles",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaLogs_OrdenCirugiaId",
                table: "CirugiaLogs",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaLogs_Timestamp",
                table: "CirugiaLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiasMedicosHonorarios_EspecialidadId",
                table: "CirugiasMedicosHonorarios",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiasMedicosHonorarios_MedicoId",
                table: "CirugiasMedicosHonorarios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiasMedicosHonorarios_OrdenCirugiaId",
                table: "CirugiasMedicosHonorarios",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiasObservacionesHistorial_OrdenCirugiaId",
                table: "CirugiasObservacionesHistorial",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_AreaClinicaId",
                table: "CitasMedicas",
                column: "AreaClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_CuentaServicioId",
                table: "CitasMedicas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_HoraPautada",
                table: "CitasMedicas",
                column: "HoraPautada");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_MedicoId",
                table: "CitasMedicas",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPago_CuentaPorCobrarId",
                table: "CompromisosPago",
                column: "CuentaPorCobrarId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosServiciosRealizados_DetalleServicioCuentaId",
                table: "ConsumosServiciosRealizados",
                column: "DetalleServicioCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosServiciosRealizados_InsumoId",
                table: "ConsumosServiciosRealizados",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioPerfilPrecios_SeguroConvenioId_PerfilId",
                table: "ConvenioPerfilPrecios",
                columns: new[] { "SeguroConvenioId", "PerfilId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_CuentaServicioId",
                table: "CuentasPorCobrar",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_AreaClinicaId",
                table: "CuentasServicios",
                column: "AreaClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_CamaRetenidaId",
                table: "CuentasServicios",
                column: "CamaRetenidaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_ConvenioId",
                table: "CuentasServicios",
                column: "ConvenioId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_CuentaPrincipalId",
                table: "CuentasServicios",
                column: "CuentaPrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_FechaCarga",
                table: "CuentasServicios",
                column: "FechaCarga");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_MedicoId",
                table: "CuentasServicios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_PacienteId",
                table: "CuentasServicios",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPago_FechaPago",
                table: "DetallesPago",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPago_MetodoPagoId",
                table: "DetallesPago",
                column: "MetodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPago_ReciboFacturaId",
                table: "DetallesPago",
                column: "ReciboFacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_AreaClinicaId",
                table: "DetallesServicioCuenta",
                column: "AreaClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_CuentaServicioId",
                table: "DetallesServicioCuenta",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_DetallePadreId",
                table: "DetallesServicioCuenta",
                column: "DetallePadreId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_MedicoResponsableId",
                table: "DetallesServicioCuenta",
                column: "MedicoResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_TipoServicioId",
                table: "DetallesServicioCuenta",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServiciosMedicosResponsables_DetalleServicioCuentaId",
                table: "DetallesServiciosMedicosResponsables",
                column: "DetalleServicioCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServiciosMedicosResponsables_MedicoId",
                table: "DetallesServiciosMedicosResponsables",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLogs_ReferenceId",
                table: "DocumentLogs",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLogs_Timestamp",
                table: "DocumentLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_GarantiasItems_CuentaPorCobrarId",
                table: "GarantiasItems",
                column: "CuentaPorCobrarId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_CuentaServicioId",
                table: "HistorialModificacionCuentas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_FechaModificacion",
                table: "HistorialModificacionCuentas",
                column: "FechaModificacion");

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
                name: "IX_HonorariosMedicosServicios_MedicoId",
                table: "HonorariosMedicosServicios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_ServicioId",
                table: "HonorariosMedicosServicios",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariumMappingRules_IsActive",
                table: "HonorariumMappingRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariumMappingRules_Priority",
                table: "HonorariumMappingRules",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosAtencionMedicos_MedicoId",
                table: "HorariosAtencionMedicos",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_Codigo",
                table: "Insumos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsumosCirugiasPacientes_CuentaServicioId_InsumoId",
                table: "InsumosCirugiasPacientes",
                columns: new[] { "CuentaServicioId", "InsumoId" });

            migrationBuilder.CreateIndex(
                name: "IX_InsumosCirugiasPacientes_InsumoId",
                table: "InsumosCirugiasPacientes",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_InsumosCirugiasPacientes_OrdenCirugiaId",
                table: "InsumosCirugiasPacientes",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_InsumosPrincipiosActivos_PrincipioActivoId",
                table: "InsumosPrincipiosActivos",
                column: "PrincipioActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_DetalleServicioId",
                table: "LogsAsignacionHonorario",
                column: "DetalleServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_FechaAccion",
                table: "LogsAsignacionHonorario",
                column: "FechaAccion");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_EspecialidadId",
                table: "Medicos",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_InsumoId",
                table: "MovimientosInsumo",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_SedeId",
                table: "MovimientosInsumo",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetRole",
                table: "Notifications",
                column: "TargetRole");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetUserId",
                table: "Notifications",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Timestamp",
                table: "Notifications",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_AreaClinicaId",
                table: "OrdenesCirugia",
                column: "AreaClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_AreaClinicaOrigenId",
                table: "OrdenesCirugia",
                column: "AreaClinicaOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_CuentaServicioId",
                table: "OrdenesCirugia",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_Estado",
                table: "OrdenesCirugia",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_FechaHoraProgramada",
                table: "OrdenesCirugia",
                column: "FechaHoraProgramada");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_MedicoId",
                table: "OrdenesCirugia",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_PacienteId",
                table: "OrdenesCirugia",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_SedeOrigenId",
                table: "OrdenesCirugia",
                column: "SedeOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_SedeQuirofanoId",
                table: "OrdenesCirugia",
                column: "SedeQuirofanoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugiaRequisitos_OrdenCirugiaId",
                table: "OrdenesCirugiaRequisitos",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugiaRequisitos_RequisitoCirugiaId",
                table: "OrdenesCirugiaRequisitos",
                column: "RequisitoCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraInventario_Estado",
                table: "OrdenesCompraInventario",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraInventario_NumeroFactura",
                table: "OrdenesCompraInventario",
                column: "NumeroFactura");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraInventario_ProveedorNombre",
                table: "OrdenesCompraInventario",
                column: "ProveedorNombre");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_PacienteId",
                table: "OrdenesDeServicio",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_Estado",
                table: "OrdenesImagenes",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_MedicoSolicitanteId",
                table: "OrdenesImagenes",
                column: "MedicoSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_PacienteId",
                table: "OrdenesImagenes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_TipoServicio",
                table: "OrdenesImagenes",
                column: "TipoServicio");

            migrationBuilder.CreateIndex(
                name: "IX_PacientesAdmision_CedulaPasaporte",
                table: "PacientesAdmision",
                column: "CedulaPasaporte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PacientesAdmision_IdPacienteLegacy",
                table: "PacientesAdmision",
                column: "IdPacienteLegacy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedores_FechaPago",
                table: "PagosProveedores",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedores_OrdenCompraId",
                table: "PagosProveedores",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_Correlativo",
                table: "PedidosInterSede",
                column: "Correlativo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_SedeProveedoraId",
                table: "PedidosInterSede",
                column: "SedeProveedoraId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_SedeSolicitanteId",
                table: "PedidosInterSede",
                column: "SedeSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSedeDetalles_InsumoId",
                table: "PedidosInterSedeDetalles",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSedeDetalles_PedidoInterSedeId",
                table: "PedidosInterSedeDetalles",
                column: "PedidoInterSedeId");

            migrationBuilder.CreateIndex(
                name: "IX_PreciosServicioConvenio_SeguroConvenioId",
                table: "PreciosServicioConvenio",
                column: "SeguroConvenioId");

            migrationBuilder.CreateIndex(
                name: "IX_PreciosServicioConvenio_ServicioClinicoId",
                table: "PreciosServicioConvenio",
                column: "ServicioClinicoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrincipiosActivos_Nombre",
                table: "PrincipiosActivos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_RazonSocial",
                table: "Proveedores",
                column: "RazonSocial");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_RIF",
                table: "Proveedores",
                column: "RIF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_CajaDiariaId",
                table: "RecibosFacturas",
                column: "CajaDiariaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_CuentaServicioId",
                table: "RecibosFacturas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_MedicoId_HoraPautada",
                table: "ReservasTemporales",
                columns: new[] { "MedicoId", "HoraPautada" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Codigo",
                table: "Sedes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosClinicos_EspecialidadId",
                table: "ServiciosClinicos",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosClinicos_ServicioInformeId",
                table: "ServiciosClinicos",
                column: "ServicioInformeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosClinicos_TipoServicioId",
                table: "ServiciosClinicos",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosIncluidosAreas_ServicioClinicoId",
                table: "ServiciosIncluidosAreas",
                column: "ServicioClinicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosInsumoRecetas_InsumoId",
                table: "ServiciosInsumoRecetas",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosInsumoRecetas_ServicioClinicoId",
                table: "ServiciosInsumoRecetas",
                column: "ServicioClinicoId");

            migrationBuilder.CreateIndex(
                name: "IX_serviciossugerencias_ServicioOrigenId",
                table: "serviciossugerencias",
                column: "ServicioOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_serviciossugerencias_ServicioSugeridoId",
                table: "serviciossugerencias",
                column: "ServicioSugeridoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInsumosCirugia_AlmacenOrigenId",
                table: "SolicitudesInsumosCirugia",
                column: "AlmacenOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInsumosCirugia_EstadoSolicitud",
                table: "SolicitudesInsumosCirugia",
                column: "EstadoSolicitud");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInsumosCirugia_InsumoId",
                table: "SolicitudesInsumosCirugia",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInsumosCirugia_OrdenCirugiaId",
                table: "SolicitudesInsumosCirugia",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_StocksSede_InsumoId",
                table: "StocksSede",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_StocksSede_SedeId_InsumoId",
                table: "StocksSede",
                columns: new[] { "SedeId", "InsumoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciasReposicionStock_FechaTransferencia",
                table: "TransferenciasReposicionStock",
                column: "FechaTransferencia");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciasReposicionStock_InsumoId",
                table: "TransferenciasReposicionStock",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciasReposicionStock_SedeDestinoId",
                table: "TransferenciasReposicionStock",
                column: "SedeDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciasReposicionStock_SedeOrigenId",
                table: "TransferenciasReposicionStock",
                column: "SedeOrigenId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "AuditLogsPrecios");

            migrationBuilder.DropTable(
                name: "BloqueosHorarios");

            migrationBuilder.DropTable(
                name: "CategoriasInsumo");

            migrationBuilder.DropTable(
                name: "CierresInventarioDetalles");

            migrationBuilder.DropTable(
                name: "CirugiaLogs");

            migrationBuilder.DropTable(
                name: "CirugiasMedicosHonorarios");

            migrationBuilder.DropTable(
                name: "CirugiasObservacionesHistorial");

            migrationBuilder.DropTable(
                name: "CitasMedicas");

            migrationBuilder.DropTable(
                name: "CompromisosPago");

            migrationBuilder.DropTable(
                name: "ConfiguracionGeneral");

            migrationBuilder.DropTable(
                name: "ConsumosServiciosRealizados");

            migrationBuilder.DropTable(
                name: "ConvenioPerfilPrecios");

            migrationBuilder.DropTable(
                name: "DetallesPago");

            migrationBuilder.DropTable(
                name: "DetallesServiciosMedicosResponsables");

            migrationBuilder.DropTable(
                name: "DocumentLogs");

            migrationBuilder.DropTable(
                name: "ErrorTickets");

            migrationBuilder.DropTable(
                name: "GarantiasItems");

            migrationBuilder.DropTable(
                name: "HistorialModificacionCuentas");

            migrationBuilder.DropTable(
                name: "HonorariosConfig");

            migrationBuilder.DropTable(
                name: "HonorariosMedicosServicios");

            migrationBuilder.DropTable(
                name: "HonorariumMappingRules");

            migrationBuilder.DropTable(
                name: "HorariosAtencionMedicos");

            migrationBuilder.DropTable(
                name: "IncidenciasHorario");

            migrationBuilder.DropTable(
                name: "InsumosCirugiasPacientes");

            migrationBuilder.DropTable(
                name: "InsumosPrincipiosActivos");

            migrationBuilder.DropTable(
                name: "LogsAsignacionHonorario");

            migrationBuilder.DropTable(
                name: "MovimientosInsumo");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrdenesCirugiaRequisitos");

            migrationBuilder.DropTable(
                name: "OrdenesDeServicio");

            migrationBuilder.DropTable(
                name: "OrdenesImagenes");

            migrationBuilder.DropTable(
                name: "PagosProveedores");

            migrationBuilder.DropTable(
                name: "PedidosInterSedeDetalles");

            migrationBuilder.DropTable(
                name: "PreciosServicioConvenio");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "RegistroAuditoriaIncidencias");

            migrationBuilder.DropTable(
                name: "ReservasTemporales");

            migrationBuilder.DropTable(
                name: "ServiciosIncluidosAreas");

            migrationBuilder.DropTable(
                name: "ServiciosInsumoRecetas");

            migrationBuilder.DropTable(
                name: "serviciossugerencias");

            migrationBuilder.DropTable(
                name: "SolicitudesInsumosCirugia");

            migrationBuilder.DropTable(
                name: "StocksSede");

            migrationBuilder.DropTable(
                name: "TasaCambio");

            migrationBuilder.DropTable(
                name: "TransferenciasReposicionStock");

            migrationBuilder.DropTable(
                name: "TriagesEnfermeria");

            migrationBuilder.DropTable(
                name: "TurnosMedicos");

            migrationBuilder.DropTable(
                name: "ValoracionesFisicas");

            migrationBuilder.DropTable(
                name: "CierresInventario");

            migrationBuilder.DropTable(
                name: "CatalogoMetodosPago");

            migrationBuilder.DropTable(
                name: "RecibosFacturas");

            migrationBuilder.DropTable(
                name: "DetallesServicioCuenta");

            migrationBuilder.DropTable(
                name: "CuentasPorCobrar");

            migrationBuilder.DropTable(
                name: "PrincipiosActivos");

            migrationBuilder.DropTable(
                name: "RequisitosCirugia");

            migrationBuilder.DropTable(
                name: "OrdenesCompraInventario");

            migrationBuilder.DropTable(
                name: "PedidosInterSede");

            migrationBuilder.DropTable(
                name: "OrdenesCirugia");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "Monedas");

            migrationBuilder.DropTable(
                name: "CajasDiarias");

            migrationBuilder.DropTable(
                name: "CuentasServicios");

            migrationBuilder.DropTable(
                name: "AreasClinicas");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "PacientesAdmision");

            migrationBuilder.DropTable(
                name: "SegurosConvenios");

            migrationBuilder.DropTable(
                name: "Sedes");

            migrationBuilder.DropTable(
                name: "ServiciosClinicos");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.DropTable(
                name: "TiposServicio");
        }
    }
}
