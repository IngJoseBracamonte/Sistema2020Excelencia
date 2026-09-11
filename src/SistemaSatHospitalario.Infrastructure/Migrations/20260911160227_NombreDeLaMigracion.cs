using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaSatHospitalario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NombreDeLaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
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
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasInsumo", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClasificacionesAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClasificacionesAreas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConfiguracionGeneral",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NombreEmpresa = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rif = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Iva = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ClaveSupervisor = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FacturarLaboratorio = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    MostrarDetalleFacturacion = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
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
                name: "Especialidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EstadosCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCaja", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EstadosCitaMedica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCitaMedica", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EstadosCuenta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCuenta", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EstadosFiscales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosFiscales", x => x.Id);
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
                    EsBaseUsd = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monedas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MotivosAutorizacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosAutorizacion", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PacientesAdmision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IdPacienteLegacy = table.Column<int>(type: "int", nullable: true),
                    CedulaPasaporte = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreCorto = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TelefonoContact = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Direccion = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
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
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
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
                    RIF = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RazonSocial = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RequisitosCirugia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsActivo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosCirugia", x => x.Id);
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
                    EsPrincipal = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
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
                name: "TasasCambio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasasCambio", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TiposIngreso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposIngreso", x => x.Id);
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
                name: "UnidadesMedida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Simbolo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsFraccionable = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UsuarioHospital",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NombreReal = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApellidoReal = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LegacyCajeroId = table.Column<int>(type: "int", nullable: true),
                    EsActivo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequirePasswordReset = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioHospital", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EspecialidadId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    HonorarioBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0.00m),
                    IntervaloTurnoMinutos = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    Telefono = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
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
                name: "CajasDiarias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaApertura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MontoInicialDivisa = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoInicialBs = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioIdentityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajasDiarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CajasDiarias_EstadosCaja_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "EstadosCaja",
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
                    EsUSD = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    EsVuelto = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    Orden = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
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
                name: "OrdenesDeServicio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NumeroLlegadaDiario = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TipoIngreso = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoFacturacion = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ConvenioId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PacienteAdmisionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    EstudioSolicitado = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Procesada = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    FechaProcesada = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AsistenteRxId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesDeServicio_PacientesAdmision_PacienteAdmisionId",
                        column: x => x.PacienteAdmisionId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrdenesDeServicio_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    FechaEmision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MontoTotalUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCompraInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesCompraInventario_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
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
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
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
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecioBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HonorarioBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TipoServicio = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoServicioId = table.Column<int>(type: "int", nullable: false),
                    LegacyMappingId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<int>(type: "int", nullable: false),
                    HonorariumCategory = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    UnidadMedida = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PermiteFraccionamiento = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    RequiereInventario = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    EspecialidadId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ServicioInformeId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    EsServicioInforme = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DesactivadoPorUsuarioId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
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
                name: "Insumos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnidadMedidaId = table.Column<int>(type: "int", nullable: false),
                    UnidadMedidaBase = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostoUnitarioBaseUSD = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PermiteFraccionamiento = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CategoriaInsumoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    FechaInactivacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OcultoEnTraslados = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    ReactivosCombinados = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Indicaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Insumos_CategoriasInsumo_CategoriaInsumoId",
                        column: x => x.CategoriaInsumoId,
                        principalTable: "CategoriasInsumo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Insumos_UnidadesMedida_UnidadMedidaId",
                        column: x => x.UnidadMedidaId,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UsuarioIdentityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ActionType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OldValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_UsuarioHospital_UsuarioIdentityId",
                        column: x => x.UsuarioIdentityId,
                        principalTable: "UsuarioHospital",
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
                    UsuarioId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Usuario = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
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
                    table.ForeignKey(
                        name: "FK_CierresInventario_UsuarioHospital_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    UsuarioIdentityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Details = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentLogs_UsuarioHospital_UsuarioIdentityId",
                        column: x => x.UsuarioIdentityId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    MensajeExcepcion = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StackTrace = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioAsociadoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioAsociado = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Resuelto = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    ComentariosResolucion = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaResolucion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ResueltoPorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ResueltoPor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErrorTickets_UsuarioHospital_ResueltoPorId",
                        column: x => x.ResueltoPorId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ErrorTickets_UsuarioHospital_UsuarioAsociadoId",
                        column: x => x.UsuarioAsociadoId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                    MappingRuleType = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    UsuarioCreoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioCreo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HonorariumMappingRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HonorariumMappingRules_UsuarioHospital_UsuarioCreoId",
                        column: x => x.UsuarioCreoId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetUserGuidId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TargetRole = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ActionUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_UsuarioHospital_TargetUserGuidId",
                        column: x => x.TargetUserGuidId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    UsuarioCreadorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioCreador = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
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
                    table.ForeignKey(
                        name: "FK_PedidosInterSede_UsuarioHospital_UsuarioCreadorId",
                        column: x => x.UsuarioCreadorId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BloqueosHorarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoraPautada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloqueosHorarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloqueosHorarios_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    UsuarioConfiguroId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaConfiguracion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NotasConfig = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
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
                name: "IncidenciasHorario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Inicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreadoPor = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenciasHorario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidenciasHorario_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidenciasHorario_UsuarioHospital_CreadoPor",
                        column: x => x.CreadoPor,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReservasTemporales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HoraPautada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioIdentityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Comentario = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiracionUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasTemporales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasTemporales_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservasTemporales_UsuarioHospital_UsuarioIdentityId",
                        column: x => x.UsuarioIdentityId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_TurnosMedicos_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnosMedicos_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CajaDeclaracionesMetodos",
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
                    table.PrimaryKey("PK_CajaDeclaracionesMetodos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CajaDeclaracionesMetodos_CajasDiarias_CajaDiariaId",
                        column: x => x.CajaDiariaId,
                        principalTable: "CajasDiarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CajaDeclaracionesMetodos_CatalogoMetodosPago_MetodoPagoId",
                        column: x => x.MetodoPagoId,
                        principalTable: "CatalogoMetodosPago",
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
                    TasaCambio = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MetodoPago = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Referencia = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioIdentityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    ServicioTarifaBaseId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ClasificacionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreasClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreasClinicas_ClasificacionesAreas_ClasificacionId",
                        column: x => x.ClasificacionId,
                        principalTable: "ClasificacionesAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AreasClinicas_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    UsuarioModificoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.ForeignKey(
                        name: "FK_HonorariosMedicosServicios_UsuarioHospital_UsuarioModificoId",
                        column: x => x.UsuarioModificoId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "ServiciosSugerencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioOrigenId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioSugeridoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosSugerencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiciosSugerencias_ServiciosClinicos_ServicioOrigenId",
                        column: x => x.ServicioOrigenId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiciosSugerencias_ServiciosClinicos_ServicioSugeridoId",
                        column: x => x.ServicioSugeridoId,
                        principalTable: "ServiciosClinicos",
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
                    Concentracion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, defaultValue: "")
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
                name: "MovimientosInsumo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TipoMovimiento = table.Column<int>(type: "int", nullable: false),
                    CantidadBase = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnidadMedidaOriginalId = table.Column<int>(type: "int", nullable: false),
                    CantidadOriginal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UsuarioIdentityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
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
                    table.ForeignKey(
                        name: "FK_MovimientosInsumo_UnidadesMedida_UnidadMedidaOriginalId",
                        column: x => x.UnidadMedidaOriginalId,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInsumo_UsuarioHospital_UsuarioIdentityId",
                        column: x => x.UsuarioIdentityId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ServiciosInsumoRecetas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioClinicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnidadMedidaConsumoId = table.Column<int>(type: "int", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiciosInsumoRecetas_UnidadesMedida_UnidadMedidaConsumoId",
                        column: x => x.UnidadMedidaConsumoId,
                        principalTable: "UnidadesMedida",
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
                    StockActual = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0.0000m),
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
                        onDelete: ReferentialAction.Restrict);
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
                    Motivo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaTransferencia = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioIdentityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
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
                    table.ForeignKey(
                        name: "FK_TransferenciasReposicionStock_UsuarioHospital_UsuarioIdentit~",
                        column: x => x.UsuarioIdentityId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    CantidadDespachada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0.0000m),
                    CantidadRecibida = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0.0000m),
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
                name: "RegistroAuditoriaIncidencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TurnoMedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IncidenciaIgnoradaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OperadorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaTraza = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Motivo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAuditoriaIncidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroAuditoriaIncidencias_IncidenciasHorario_IncidenciaIg~",
                        column: x => x.IncidenciaIgnoradaId,
                        principalTable: "IncidenciasHorario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroAuditoriaIncidencias_TurnosMedicos_TurnoMedicoId",
                        column: x => x.TurnoMedicoId,
                        principalTable: "TurnosMedicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistroAuditoriaIncidencias_UsuarioHospital_OperadorId",
                        column: x => x.OperadorId,
                        principalTable: "UsuarioHospital",
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
                    UsuarioCargaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCarga = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    TipoIngresoId = table.Column<int>(type: "int", nullable: false),
                    ConvenioId = table.Column<int>(type: "int", nullable: true),
                    LegacyOrderId = table.Column<int>(type: "int", nullable: true),
                    ProcesamientoEstado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AreaClinicaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SubAreaClinica = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CamaRetenidaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioValidacionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaValidacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioAuditoriaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaAuditoria = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DestinoPaciente = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PersonalRelevo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
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
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_AreasClinicas_CamaRetenidaId",
                        column: x => x.CamaRetenidaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_CuentasServicios_CuentaPrincipalId",
                        column: x => x.CuentaPrincipalId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_EstadosCuenta_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "EstadosCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_TiposIngreso_TipoIngresoId",
                        column: x => x.TipoIngresoId,
                        principalTable: "TiposIngreso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_UsuarioHospital_UsuarioAuditoriaId",
                        column: x => x.UsuarioAuditoriaId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_UsuarioHospital_UsuarioCargaId",
                        column: x => x.UsuarioCargaId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasServicios_UsuarioHospital_UsuarioValidacionId",
                        column: x => x.UsuarioValidacionId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ServiciosIncluidosArea",
                columns: table => new
                {
                    AreaClinicaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioClinicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosIncluidosArea", x => new { x.AreaClinicaId, x.ServicioClinicoId });
                    table.ForeignKey(
                        name: "FK_ServiciosIncluidosArea_AreasClinicas_AreaClinicaId",
                        column: x => x.AreaClinicaId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiciosIncluidosArea_ServiciosClinicos_ServicioClinicoId",
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
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
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
                        name: "FK_CitasMedicas_EstadosCitaMedica_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "EstadosCitaMedica",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitasMedicas_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitasMedicas_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAudited = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UsuarioAuditoriaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioAuditoria = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaAuditoria = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CompromisoGenerado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GarantiaGenerada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    QuienAutorizo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DoctorProcedimiento = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InformacionAdicional = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
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
                    table.ForeignKey(
                        name: "FK_CuentasPorCobrar_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasPorCobrar_UsuarioHospital_UsuarioAuditoriaId",
                        column: x => x.UsuarioAuditoriaId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DetallesServicioCuenta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Descripcion = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Honorario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TipoServicioId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCargaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCarga = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LegacyMappingId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoResponsableId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AreaClinicaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DetallePadreId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IncluidoEnTarifaBase = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PrecioCatalogoHistorico = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Realizado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRealizacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioTecnicoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
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
                name: "HistorialModificacionCuentas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PacienteAnteriorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PacienteNuevoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TipoIngresoAnterior = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoIngresoNuevo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConvenioAnteriorId = table.Column<int>(type: "int", nullable: true),
                    ConvenioNuevoId = table.Column<int>(type: "int", nullable: true),
                    TotalAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboTotalAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboTotalNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboVueltoAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboVueltoNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReciboPagadoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CxCSaldoAnteriorUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CxCSaldoNuevoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialModificacionCuentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialModificacionCuentas_CuentasServicios_CuentaServicio~",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialModificacionCuentas_PacientesAdmision_PacienteAnter~",
                        column: x => x.PacienteAnteriorId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HistorialModificacionCuentas_PacientesAdmision_PacienteNuevo~",
                        column: x => x.PacienteNuevoId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HistorialModificacionCuentas_SegurosConvenios_ConvenioAnteri~",
                        column: x => x.ConvenioAnteriorId,
                        principalTable: "SegurosConvenios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HistorialModificacionCuentas_SegurosConvenios_ConvenioNuevoId",
                        column: x => x.ConvenioNuevoId,
                        principalTable: "SegurosConvenios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HistorialModificacionCuentas_UsuarioHospital_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "UsuarioHospital",
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
                    DescripcionCirugia = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecioBaseUsd = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PrecioDerechoSalaUsd = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FechaHoraProgramada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MotivoCancelacion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioCreacionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    SalaQuirofano = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModalidadAnestesia = table.Column<string>(type: "longtext", nullable: false)
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_AreasClinicas_AreaClinicaOrigenId",
                        column: x => x.AreaClinicaOrigenId,
                        principalTable: "AreasClinicas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_PacientesAdmision_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "PacientesAdmision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_Sedes_SedeOrigenId",
                        column: x => x.SedeOrigenId,
                        principalTable: "Sedes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrdenesCirugia_Sedes_SedeQuirofanoId",
                        column: x => x.SedeQuirofanoId,
                        principalTable: "Sedes",
                        principalColumn: "Id");
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
                    Estudio = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoServicio = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ProcesadoPor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaProcesado = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EsDirecta = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    RequiereValidacion = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Validada = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    ValidadorPor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaValidacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MedicoSolicitanteId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Informe = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinkInforme = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObservacionesMedico = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoInterpreteId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    RequiereInforme = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesImagenes_CuentasServicios_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesImagenes_Medicos_MedicoInterpreteId",
                        column: x => x.MedicoInterpreteId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RecibosFacturas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CajaDiariaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    NroControlFiscal = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TasaCambioDia = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EstadoFiscalId = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NumeroRecibo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroComprobante = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalFacturadoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MontoVueltoUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioEmisionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
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
                    table.ForeignKey(
                        name: "FK_RecibosFacturas_EstadosFiscales_EstadoFiscalId",
                        column: x => x.EstadoFiscalId,
                        principalTable: "EstadosFiscales",
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
                    UsuarioRegistroId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DescripcionRapida = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionDetallada = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CuentaServicioId1 = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
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
                    table.ForeignKey(
                        name: "FK_TriagesEnfermeria_CuentasServicios_CuentaServicioId1",
                        column: x => x.CuentaServicioId1,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TriagesEnfermeria_UsuarioHospital_UsuarioRegistroId",
                        column: x => x.UsuarioRegistroId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    Alergias = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccesosVenosos = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pertenencias = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AntecedentesMedicos = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioRegistroId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioRegistro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
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
                    table.ForeignKey(
                        name: "FK_ValoracionesFisicas_UsuarioHospital_UsuarioRegistroId",
                        column: x => x.UsuarioRegistroId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CompromisosPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaPorCobrarId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Omitido = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Observacion = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioCreacionId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioCreacion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MotivoAutorizacionId = table.Column<int>(type: "int", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_CompromisosPago_MotivosAutorizacion_MotivoAutorizacionId",
                        column: x => x.MotivoAutorizacionId,
                        principalTable: "MotivosAutorizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompromisosPago_UsuarioHospital_UsuarioCreacionId",
                        column: x => x.UsuarioCreacionId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "AuditLogsPrecios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DescripcionServicio = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecioOriginal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioModificado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HonorarioAnterior = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NuevoHonorario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UsuarioOperadorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioOperador = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AutorizadoPorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    AutorizadoPor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogsPrecios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogsPrecios_DetallesServicioCuenta_DetalleServicioId",
                        column: x => x.DetalleServicioId,
                        principalTable: "DetallesServicioCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditLogsPrecios_UsuarioHospital_AutorizadoPorId",
                        column: x => x.AutorizadoPorId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLogsPrecios_UsuarioHospital_UsuarioOperadorId",
                        column: x => x.UsuarioOperadorId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    CostoTotalUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumosServiciosRealizados_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DetalleServiciosMedicosResponsables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioCuentaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Rol = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontoHonorario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0.00m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleServiciosMedicosResponsables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleServiciosMedicosResponsables_DetallesServicioCuenta_D~",
                        column: x => x.DetalleServicioCuentaId,
                        principalTable: "DetallesServicioCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleServiciosMedicosResponsables_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LogsAsignacionHonorario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NombreServicio = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoAccion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoAnteriorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    MedicoAnteriorNombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicoNuevoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    MedicoNuevoNombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioOperadorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaAccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsAsignacionHonorario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsAsignacionHonorario_DetallesServicioCuenta_DetalleServic~",
                        column: x => x.DetalleServicioId,
                        principalTable: "DetallesServicioCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogsAsignacionHonorario_Medicos_MedicoAnteriorId",
                        column: x => x.MedicoAnteriorId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LogsAsignacionHonorario_Medicos_MedicoNuevoId",
                        column: x => x.MedicoNuevoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistorialModificacionCuentaDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    HistorialModificacionCuentaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PrecioAnterior = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioNuevo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HonorarioAnterior = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HonorarioNuevo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CantidadAnterior = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CantidadNueva = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialModificacionCuentaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialModificacionCuentaDetalles_DetallesServicioCuenta_D~",
                        column: x => x.DetalleServicioId,
                        principalTable: "DetallesServicioCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCue~",
                        column: x => x.HistorialModificacionCuentaId,
                        principalTable: "HistorialModificacionCuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CirugiaLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UsuarioIdentityId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Evento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
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
                    table.ForeignKey(
                        name: "FK_CirugiaLogs_UsuarioHospital_UsuarioIdentityId",
                        column: x => x.UsuarioIdentityId,
                        principalTable: "UsuarioHospital",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CirugiaMedicosHonorarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EspecialidadId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MontoHonorarioUsd = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0.00m),
                    EsCirujanoPrincipal = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CirugiaMedicosHonorarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CirugiaMedicosHonorarios_Especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "Especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CirugiaMedicosHonorarios_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CirugiaMedicosHonorarios_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CirugiaObservacionesHistorial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Observacion = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioRegistroId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CirugiaObservacionesHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CirugiaObservacionesHistorial_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InsumosCirugiaPaciente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CuentaServicioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CantidadEntregada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CantidadDevuelta = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0.0000m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsumosCirugiaPaciente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsumosCirugiaPaciente_CuentasServicios_CuentaServicioId",
                        column: x => x.CuentaServicioId,
                        principalTable: "CuentasServicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsumosCirugiaPaciente_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InsumosCirugiaPaciente_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrdenCirugiaRequisitos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrdenCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RequisitoCirugiaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Cumplido = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    FechaVerificacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VerificadoPor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCirugiaRequisitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenCirugiaRequisitos_OrdenesCirugia_OrdenCirugiaId",
                        column: x => x.OrdenCirugiaId,
                        principalTable: "OrdenesCirugia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenCirugiaRequisitos_RequisitosCirugia_RequisitoCirugiaId",
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
                    UsuarioSolicitudId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaDespacho = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioDespachoId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Observaciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
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
                    ReferenciaBancaria = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontoAbonadoMoneda = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EquivalenteAbonadoBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TasaCambioAplicada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioCarga = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioCargaId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesPago_CatalogoMetodosPago_MetodoPagoId",
                        column: x => x.MetodoPagoId,
                        principalTable: "CatalogoMetodosPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                table: "ClasificacionesAreas",
                columns: new[] { "Id", "Codigo", "Descripcion" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), "CAMA", "Cama" },
                    { new Guid("60000000-0000-0000-0000-000000000002"), "QUIROFANO", "Quirófano" },
                    { new Guid("60000000-0000-0000-0000-000000000003"), "SALA_PARTO", "Sala de Parto" }
                });

            migrationBuilder.InsertData(
                table: "EstadosCaja",
                columns: new[] { "Id", "Activo", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "ABIERTA", "Abierta" },
                    { 2, true, "CERRADA_POR_ASISTENTE", "Cerrada por Asistente" },
                    { 3, true, "CERRADA", "Cerrada" }
                });

            migrationBuilder.InsertData(
                table: "EstadosCitaMedica",
                columns: new[] { "Id", "Activo", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "PENDIENTE", "Pendiente" },
                    { 2, true, "CONFIRMADA", "Confirmada" },
                    { 3, true, "ATENDIDA", "Atendida" },
                    { 4, true, "CANCELADA", "Cancelada" }
                });

            migrationBuilder.InsertData(
                table: "EstadosCuenta",
                columns: new[] { "Id", "Activo", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "ABIERTA", "Abierta" },
                    { 2, true, "FACTURADA", "Facturada" },
                    { 3, true, "ANULADA", "Anulada" },
                    { 4, true, "VALIDADA", "Validada" }
                });

            migrationBuilder.InsertData(
                table: "EstadosFiscales",
                columns: new[] { "Id", "Activo", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "BORRADOR", "Borrador" },
                    { 2, true, "EMITIDA", "Emitida" },
                    { 3, true, "ANULADA", "Anulada" }
                });

            migrationBuilder.InsertData(
                table: "Monedas",
                columns: new[] { "Id", "Codigo", "EsBaseUsd", "Nombre", "Simbolo" },
                values: new object[] { 1, "USD", true, "Dólar", "$" });

            migrationBuilder.InsertData(
                table: "Monedas",
                columns: new[] { "Id", "Codigo", "Nombre", "Simbolo" },
                values: new object[,]
                {
                    { 2, "VES", "Bolívar", "Bs." },
                    { 3, "EUR", "Euro", "€" },
                    { 4, "COP", "Peso Colombiano", "COP$" },
                    { 5, "ARS", "Peso Argentino", "ARS$" }
                });

            migrationBuilder.InsertData(
                table: "MotivosAutorizacion",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Autorizado por Dirección Médica" },
                    { 2, true, "Exoneración por Presidencia" },
                    { 3, true, "Convenio Institucional" }
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
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), true, "SEDE-PRINCIPAL", true, "Almacén Principal / Farmacia Central" });

            migrationBuilder.InsertData(
                table: "Sedes",
                columns: new[] { "Id", "Activo", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000002"), true, "SEDE-EMG", "Depósito Emergencia" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), true, "SEDE-HOSP", "Depósito Hospitalización" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), true, "SEDE-UCI", "Depósito UCI" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), true, "SEDE-CIRUGIA", "Quirófano / Pabellón Central" }
                });

            migrationBuilder.InsertData(
                table: "TiposIngreso",
                columns: new[] { "Id", "Activo", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "PARTICULAR", "Particular" },
                    { 2, true, "SEGURO", "Seguro" },
                    { 3, true, "HOSPITALIZACION", "Hospitalización" },
                    { 4, true, "EMERGENCIA", "Emergencia" },
                    { 5, true, "UCI", "UCI" }
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
                table: "UnidadesMedida",
                columns: new[] { "Id", "Activo", "Codigo", "EsFraccionable", "Nombre", "Simbolo" },
                values: new object[,]
                {
                    { 1, true, "UNIDAD", true, "Unidad", "UND" },
                    { 2, true, "KG", true, "Kilogramo", "kg" },
                    { 3, true, "G", true, "Gramo", "g" },
                    { 4, true, "DG", true, "Decigramo", "dg" },
                    { 5, true, "MG", true, "Miligramo", "mg" },
                    { 6, true, "L", true, "Litro", "L" },
                    { 7, true, "ML", true, "Mililitro", "mL" }
                });

            migrationBuilder.InsertData(
                table: "AreasClinicas",
                columns: new[] { "Id", "Activo", "AreaPadreId", "ClasificacionId", "Codigo", "EsAreaAdmision", "EsSubAreaAlmacenPrincipal", "Estado", "Nombre", "SedeId", "ServicioTarifaBaseId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), true, null, new Guid("60000000-0000-0000-0000-000000000001"), "BOX-1", true, false, 1, "Box Emergencia 1", new Guid("10000000-0000-0000-0000-000000000002"), null },
                    { new Guid("30000000-0000-0000-0000-000000000002"), true, null, new Guid("60000000-0000-0000-0000-000000000001"), "HAB-101", false, false, 1, "Habitación 101", new Guid("10000000-0000-0000-0000-000000000003"), null },
                    { new Guid("30000000-0000-0000-0000-000000000003"), true, null, new Guid("60000000-0000-0000-0000-000000000001"), "UCI-1", false, false, 1, "Cama UCI 1", new Guid("10000000-0000-0000-0000-000000000004"), null },
                    { new Guid("30000000-0000-0000-0000-000000000004"), true, null, new Guid("60000000-0000-0000-0000-000000000001"), "FARMACIA", false, false, 1, "Farmacia Central", new Guid("10000000-0000-0000-0000-000000000001"), null },
                    { new Guid("30000000-0000-0000-0000-000000000005"), true, null, new Guid("60000000-0000-0000-0000-000000000001"), "LABORATORIO", false, false, 1, "Laboratorio Central", new Guid("10000000-0000-0000-0000-000000000001"), null },
                    { new Guid("30000000-0000-0000-0000-000000000006"), true, null, new Guid("60000000-0000-0000-0000-000000000002"), "QX-1", false, false, 1, "Quirófano 1 (Cirugía Mayor)", new Guid("10000000-0000-0000-0000-000000000005"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreasClinicas_ClasificacionId",
                table: "AreasClinicas",
                column: "ClasificacionId");

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
                name: "IX_AuditLogs_ActionType",
                table: "AuditLogs",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActionType_Timestamp",
                table: "AuditLogs",
                columns: new[] { "ActionType", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UsuarioIdentityId",
                table: "AuditLogs",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsPrecios_AutorizadoPorId",
                table: "AuditLogsPrecios",
                column: "AutorizadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsPrecios_DetalleServicioId",
                table: "AuditLogsPrecios",
                column: "DetalleServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsPrecios_FechaModificacion",
                table: "AuditLogsPrecios",
                column: "FechaModificacion");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsPrecios_UsuarioOperadorId",
                table: "AuditLogsPrecios",
                column: "UsuarioOperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueosHorarios_FechaRegistro",
                table: "BloqueosHorarios",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueosHorarios_MedicoId",
                table: "BloqueosHorarios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_BloqueosHorarios_MedicoId_HoraPautada",
                table: "BloqueosHorarios",
                columns: new[] { "MedicoId", "HoraPautada" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CajaDeclaracionesMetodos_CajaDiariaId",
                table: "CajaDeclaracionesMetodos",
                column: "CajaDiariaId");

            migrationBuilder.CreateIndex(
                name: "IX_CajaDeclaracionesMetodos_MetodoPagoId",
                table: "CajaDeclaracionesMetodos",
                column: "MetodoPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_CajasDiarias_EstadoId",
                table: "CajasDiarias",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CajasDiarias_UsuarioIdentityId",
                table: "CajasDiarias",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogoMetodosPago_Activo",
                table: "CatalogoMetodosPago",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogoMetodosPago_Activo_Orden",
                table: "CatalogoMetodosPago",
                columns: new[] { "Activo", "Orden" });

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
                name: "IX_CategoriasInsumo_Activo",
                table: "CategoriasInsumo",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasInsumo_Codigo",
                table: "CategoriasInsumo",
                column: "Codigo");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasInsumo_Nombre",
                table: "CategoriasInsumo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventario_FechaCierre",
                table: "CierresInventario",
                column: "FechaCierre");

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventario_SedeId",
                table: "CierresInventario",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventario_SedeId_FechaCierre",
                table: "CierresInventario",
                columns: new[] { "SedeId", "FechaCierre" });

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventario_UsuarioId",
                table: "CierresInventario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventarioDetalles_CierreInventarioId",
                table: "CierresInventarioDetalles",
                column: "CierreInventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventarioDetalles_CierreInventarioId_InsumoId",
                table: "CierresInventarioDetalles",
                columns: new[] { "CierreInventarioId", "InsumoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventarioDetalles_InsumoId",
                table: "CierresInventarioDetalles",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaLogs_OrdenCirugiaId",
                table: "CirugiaLogs",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaLogs_OrdenCirugiaId_Timestamp",
                table: "CirugiaLogs",
                columns: new[] { "OrdenCirugiaId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaLogs_Timestamp",
                table: "CirugiaLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaLogs_UsuarioIdentityId",
                table: "CirugiaLogs",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaMedicosHonorarios_EspecialidadId",
                table: "CirugiaMedicosHonorarios",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaMedicosHonorarios_MedicoId",
                table: "CirugiaMedicosHonorarios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaMedicosHonorarios_MedicoId_MontoHonorarioUsd",
                table: "CirugiaMedicosHonorarios",
                columns: new[] { "MedicoId", "MontoHonorarioUsd" });

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaMedicosHonorarios_OrdenCirugiaId",
                table: "CirugiaMedicosHonorarios",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaMedicosHonorarios_OrdenCirugiaId_MedicoId",
                table: "CirugiaMedicosHonorarios",
                columns: new[] { "OrdenCirugiaId", "MedicoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaObservacionesHistorial_OrdenCirugiaId",
                table: "CirugiaObservacionesHistorial",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaObservacionesHistorial_UsuarioRegistroId",
                table: "CirugiaObservacionesHistorial",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_AreaClinicaId",
                table: "CitasMedicas",
                column: "AreaClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_CuentaServicioId",
                table: "CitasMedicas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_EstadoId",
                table: "CitasMedicas",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_FechaRegistro",
                table: "CitasMedicas",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_HoraPautada",
                table: "CitasMedicas",
                column: "HoraPautada");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_MedicoId",
                table: "CitasMedicas",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_MedicoId_HoraPautada",
                table: "CitasMedicas",
                columns: new[] { "MedicoId", "HoraPautada" });

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_PacienteId",
                table: "CitasMedicas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClasificacionesAreas_Codigo",
                table: "ClasificacionesAreas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClasificacionesAreas_Descripcion",
                table: "ClasificacionesAreas",
                column: "Descripcion");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPago_CuentaPorCobrarId",
                table: "CompromisosPago",
                column: "CuentaPorCobrarId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPago_FechaCreacion",
                table: "CompromisosPago",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPago_MotivoAutorizacionId",
                table: "CompromisosPago",
                column: "MotivoAutorizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPago_UsuarioCreacionId",
                table: "CompromisosPago",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosServiciosRealizados_DetalleServicioCuentaId",
                table: "ConsumosServiciosRealizados",
                column: "DetalleServicioCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosServiciosRealizados_FechaConsumo",
                table: "ConsumosServiciosRealizados",
                column: "FechaConsumo");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosServiciosRealizados_InsumoId",
                table: "ConsumosServiciosRealizados",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioPerfilPrecios_Activo",
                table: "ConvenioPerfilPrecios",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioPerfilPrecios_PerfilId",
                table: "ConvenioPerfilPrecios",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioPerfilPrecios_SeguroConvenioId",
                table: "ConvenioPerfilPrecios",
                column: "SeguroConvenioId");

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
                name: "IX_CuentasPorCobrar_Estado",
                table: "CuentasPorCobrar",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_FechaCreacion",
                table: "CuentasPorCobrar",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_PacienteId",
                table: "CuentasPorCobrar",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_UsuarioAuditoriaId",
                table: "CuentasPorCobrar",
                column: "UsuarioAuditoriaId");

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
                name: "IX_CuentasServicios_EstadoId",
                table: "CuentasServicios",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_FechaCarga",
                table: "CuentasServicios",
                column: "FechaCarga");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_FechaCierre",
                table: "CuentasServicios",
                column: "FechaCierre");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_LegacyOrderId",
                table: "CuentasServicios",
                column: "LegacyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_MedicoId",
                table: "CuentasServicios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_PacienteId",
                table: "CuentasServicios",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_TipoIngresoId",
                table: "CuentasServicios",
                column: "TipoIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_UsuarioAuditoriaId",
                table: "CuentasServicios",
                column: "UsuarioAuditoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_UsuarioCargaId",
                table: "CuentasServicios",
                column: "UsuarioCargaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_UsuarioValidacionId",
                table: "CuentasServicios",
                column: "UsuarioValidacionId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleServiciosMedicosResponsables_DetalleServicioCuentaId",
                table: "DetalleServiciosMedicosResponsables",
                column: "DetalleServicioCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleServiciosMedicosResponsables_MedicoId",
                table: "DetalleServiciosMedicosResponsables",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleServiciosMedicosResponsables_MedicoId_MontoHonorario",
                table: "DetalleServiciosMedicosResponsables",
                columns: new[] { "MedicoId", "MontoHonorario" });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleServiciosMedicosResponsables_Rol",
                table: "DetalleServiciosMedicosResponsables",
                column: "Rol");

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
                name: "IX_DetallesPago_UsuarioCargaId",
                table: "DetallesPago",
                column: "UsuarioCargaId");

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
                name: "IX_DetallesServicioCuenta_FechaCarga",
                table: "DetallesServicioCuenta",
                column: "FechaCarga");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_MedicoResponsableId",
                table: "DetallesServicioCuenta",
                column: "MedicoResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_ServicioId",
                table: "DetallesServicioCuenta",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_TipoServicioId",
                table: "DetallesServicioCuenta",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_UsuarioCargaId",
                table: "DetallesServicioCuenta",
                column: "UsuarioCargaId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLogs_DocumentType_ReferenceId",
                table: "DocumentLogs",
                columns: new[] { "DocumentType", "ReferenceId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLogs_ReferenceId",
                table: "DocumentLogs",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLogs_Timestamp",
                table: "DocumentLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLogs_UsuarioIdentityId",
                table: "DocumentLogs",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTickets_FechaCreacion",
                table: "ErrorTickets",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTickets_Resuelto",
                table: "ErrorTickets",
                column: "Resuelto");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTickets_ResueltoPorId",
                table: "ErrorTickets",
                column: "ResueltoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTickets_UsuarioAsociadoId",
                table: "ErrorTickets",
                column: "UsuarioAsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Especialidades_Activo",
                table: "Especialidades",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Especialidades_Nombre",
                table: "Especialidades",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCaja_Codigo",
                table: "EstadosCaja",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCitaMedica_Codigo",
                table: "EstadosCitaMedica",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCuenta_Codigo",
                table: "EstadosCuenta",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosFiscales_Codigo",
                table: "EstadosFiscales",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GarantiasItems_CuentaPorCobrarId",
                table: "GarantiasItems",
                column: "CuentaPorCobrarId");

            migrationBuilder.CreateIndex(
                name: "IX_GarantiasItems_FechaRegistro",
                table: "GarantiasItems",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentaDetalles_DetalleServicioId",
                table: "HistorialModificacionCuentaDetalles",
                column: "DetalleServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCue~",
                table: "HistorialModificacionCuentaDetalles",
                column: "HistorialModificacionCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_ConvenioAnteriorId",
                table: "HistorialModificacionCuentas",
                column: "ConvenioAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_ConvenioNuevoId",
                table: "HistorialModificacionCuentas",
                column: "ConvenioNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_CuentaServicioId",
                table: "HistorialModificacionCuentas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_FechaModificacion",
                table: "HistorialModificacionCuentas",
                column: "FechaModificacion");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_PacienteAnteriorId",
                table: "HistorialModificacionCuentas",
                column: "PacienteAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_PacienteNuevoId",
                table: "HistorialModificacionCuentas",
                column: "PacienteNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialModificacionCuentas_UsuarioId",
                table: "HistorialModificacionCuentas",
                column: "UsuarioId");

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
                name: "IX_HonorariosConfig_UsuarioConfiguroId",
                table: "HonorariosConfig",
                column: "UsuarioConfiguroId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_FechaModificacion",
                table: "HonorariosMedicosServicios",
                column: "FechaModificacion");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_MedicoId",
                table: "HonorariosMedicosServicios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_ServicioId",
                table: "HonorariosMedicosServicios",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_ServicioId_MedicoId",
                table: "HonorariosMedicosServicios",
                columns: new[] { "ServicioId", "MedicoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_UsuarioModificoId",
                table: "HonorariosMedicosServicios",
                column: "UsuarioModificoId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariumMappingRules_Category",
                table: "HonorariumMappingRules",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariumMappingRules_IsActive_Priority",
                table: "HonorariumMappingRules",
                columns: new[] { "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_HonorariumMappingRules_UsuarioCreoId",
                table: "HonorariumMappingRules",
                column: "UsuarioCreoId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosAtencionMedicos_MedicoId",
                table: "HorariosAtencionMedicos",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosAtencionMedicos_MedicoId_DiaSemana",
                table: "HorariosAtencionMedicos",
                columns: new[] { "MedicoId", "DiaSemana" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasHorario_CreadoPor",
                table: "IncidenciasHorario",
                column: "CreadoPor");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasHorario_MedicoId",
                table: "IncidenciasHorario",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasHorario_MedicoId_Inicio_Fin",
                table: "IncidenciasHorario",
                columns: new[] { "MedicoId", "Inicio", "Fin" });

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_CategoriaInsumoId",
                table: "Insumos",
                column: "CategoriaInsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_Codigo",
                table: "Insumos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_FechaVencimiento",
                table: "Insumos",
                column: "FechaVencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_IsDeleted",
                table: "Insumos",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_Nombre",
                table: "Insumos",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_OcultoEnTraslados",
                table: "Insumos",
                column: "OcultoEnTraslados");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_UnidadMedidaId",
                table: "Insumos",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_InsumosCirugiaPaciente_CuentaServicioId",
                table: "InsumosCirugiaPaciente",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_InsumosCirugiaPaciente_CuentaServicioId_InsumoId",
                table: "InsumosCirugiaPaciente",
                columns: new[] { "CuentaServicioId", "InsumoId" });

            migrationBuilder.CreateIndex(
                name: "IX_InsumosCirugiaPaciente_InsumoId",
                table: "InsumosCirugiaPaciente",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_InsumosCirugiaPaciente_OrdenCirugiaId",
                table: "InsumosCirugiaPaciente",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_InsumosPrincipiosActivos_InsumoId",
                table: "InsumosPrincipiosActivos",
                column: "InsumoId");

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
                name: "IX_LogsAsignacionHonorario_MedicoAnteriorId",
                table: "LogsAsignacionHonorario",
                column: "MedicoAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_MedicoNuevoId",
                table: "LogsAsignacionHonorario",
                column: "MedicoNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_UsuarioOperadorId",
                table: "LogsAsignacionHonorario",
                column: "UsuarioOperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_Activo",
                table: "Medicos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_EspecialidadId",
                table: "Medicos",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_Nombre",
                table: "Medicos",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Monedas_Codigo",
                table: "Monedas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotivosAutorizacion_Nombre",
                table: "MotivosAutorizacion",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_Fecha",
                table: "MovimientosInsumo",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_InsumoId",
                table: "MovimientosInsumo",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_InsumoId_SedeId_Fecha",
                table: "MovimientosInsumo",
                columns: new[] { "InsumoId", "SedeId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_SedeId",
                table: "MovimientosInsumo",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_TipoMovimiento",
                table: "MovimientosInsumo",
                column: "TipoMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_UnidadMedidaOriginalId",
                table: "MovimientosInsumo",
                column: "UnidadMedidaOriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_UsuarioIdentityId",
                table: "MovimientosInsumo",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetRole",
                table: "Notifications",
                column: "TargetRole");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetUserGuidId",
                table: "Notifications",
                column: "TargetUserGuidId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetUserGuidId_IsRead",
                table: "Notifications",
                columns: new[] { "TargetUserGuidId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Timestamp",
                table: "Notifications",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCirugiaRequisitos_Cumplido",
                table: "OrdenCirugiaRequisitos",
                column: "Cumplido");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCirugiaRequisitos_OrdenCirugiaId",
                table: "OrdenCirugiaRequisitos",
                column: "OrdenCirugiaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCirugiaRequisitos_OrdenCirugiaId_RequisitoCirugiaId",
                table: "OrdenCirugiaRequisitos",
                columns: new[] { "OrdenCirugiaId", "RequisitoCirugiaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCirugiaRequisitos_RequisitoCirugiaId",
                table: "OrdenCirugiaRequisitos",
                column: "RequisitoCirugiaId");

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
                name: "IX_OrdenesCompraInventario_Estado",
                table: "OrdenesCompraInventario",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraInventario_NumeroFactura",
                table: "OrdenesCompraInventario",
                column: "NumeroFactura");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraInventario_ProveedorId",
                table: "OrdenesCompraInventario",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_AsistenteRxId",
                table: "OrdenesDeServicio",
                column: "AsistenteRxId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_ConvenioId",
                table: "OrdenesDeServicio",
                column: "ConvenioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_FechaCreacion",
                table: "OrdenesDeServicio",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_FechaCreacion_NumeroLlegadaDiario",
                table: "OrdenesDeServicio",
                columns: new[] { "FechaCreacion", "NumeroLlegadaDiario" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_FechaProcesada",
                table: "OrdenesDeServicio",
                column: "FechaProcesada");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_PacienteAdmisionId",
                table: "OrdenesDeServicio",
                column: "PacienteAdmisionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_PacienteId",
                table: "OrdenesDeServicio",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeServicio_Procesada",
                table: "OrdenesDeServicio",
                column: "Procesada");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_CuentaId",
                table: "OrdenesImagenes",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_FechaCreacion",
                table: "OrdenesImagenes",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_MedicoInterpreteId",
                table: "OrdenesImagenes",
                column: "MedicoInterpreteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_MedicoSolicitanteId",
                table: "OrdenesImagenes",
                column: "MedicoSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_PacienteId",
                table: "OrdenesImagenes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesImagenes_TipoServicio_Estado",
                table: "OrdenesImagenes",
                columns: new[] { "TipoServicio", "Estado" });

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
                name: "IX_PacientesAdmision_NombreCorto",
                table: "PacientesAdmision",
                column: "NombreCorto");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedores_FechaPago",
                table: "PagosProveedores",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedores_OrdenCompraId",
                table: "PagosProveedores",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedores_UsuarioIdentityId",
                table: "PagosProveedores",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_Correlativo",
                table: "PedidosInterSede",
                column: "Correlativo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_Estado",
                table: "PedidosInterSede",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_FechaCreacion",
                table: "PedidosInterSede",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_SedeProveedoraId",
                table: "PedidosInterSede",
                column: "SedeProveedoraId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_SedeProveedoraId_Estado",
                table: "PedidosInterSede",
                columns: new[] { "SedeProveedoraId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_SedeSolicitanteId",
                table: "PedidosInterSede",
                column: "SedeSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_SedeSolicitanteId_Estado",
                table: "PedidosInterSede",
                columns: new[] { "SedeSolicitanteId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_UsuarioCreadorId",
                table: "PedidosInterSede",
                column: "UsuarioCreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSedeDetalles_InsumoId",
                table: "PedidosInterSedeDetalles",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSedeDetalles_PedidoInterSedeId",
                table: "PedidosInterSedeDetalles",
                column: "PedidoInterSedeId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSedeDetalles_PedidoInterSedeId_InsumoId",
                table: "PedidosInterSedeDetalles",
                columns: new[] { "PedidoInterSedeId", "InsumoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreciosServicioConvenio_SeguroConvenioId",
                table: "PreciosServicioConvenio",
                column: "SeguroConvenioId");

            migrationBuilder.CreateIndex(
                name: "IX_PreciosServicioConvenio_ServicioClinicoId",
                table: "PreciosServicioConvenio",
                column: "ServicioClinicoId");

            migrationBuilder.CreateIndex(
                name: "IX_PreciosServicioConvenio_ServicioClinicoId_SeguroConvenioId",
                table: "PreciosServicioConvenio",
                columns: new[] { "ServicioClinicoId", "SeguroConvenioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrincipiosActivos_Activo",
                table: "PrincipiosActivos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_PrincipiosActivos_Nombre",
                table: "PrincipiosActivos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_Activo",
                table: "Proveedores",
                column: "Activo");

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
                name: "IX_RecibosFacturas_EstadoFiscalId",
                table: "RecibosFacturas",
                column: "EstadoFiscalId");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_NumeroRecibo",
                table: "RecibosFacturas",
                column: "NumeroRecibo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_PacienteId",
                table: "RecibosFacturas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_UsuarioEmisionId",
                table: "RecibosFacturas",
                column: "UsuarioEmisionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAuditoriaIncidencias_FechaTraza",
                table: "RegistroAuditoriaIncidencias",
                column: "FechaTraza");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAuditoriaIncidencias_IncidenciaIgnoradaId",
                table: "RegistroAuditoriaIncidencias",
                column: "IncidenciaIgnoradaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAuditoriaIncidencias_OperadorId",
                table: "RegistroAuditoriaIncidencias",
                column: "OperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAuditoriaIncidencias_TurnoMedicoId",
                table: "RegistroAuditoriaIncidencias",
                column: "TurnoMedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosCirugia_EsActivo",
                table: "RequisitosCirugia",
                column: "EsActivo");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosCirugia_Nombre",
                table: "RequisitosCirugia",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_ExpiracionUtc",
                table: "ReservasTemporales",
                column: "ExpiracionUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_MedicoId_HoraPautada",
                table: "ReservasTemporales",
                columns: new[] { "MedicoId", "HoraPautada" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_UsuarioIdentityId",
                table: "ReservasTemporales",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Activo",
                table: "Sedes",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Codigo",
                table: "Sedes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_EsPrincipal",
                table: "Sedes",
                column: "EsPrincipal");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Nombre",
                table: "Sedes",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosClinicos_Activo",
                table: "ServiciosClinicos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosClinicos_Codigo",
                table: "ServiciosClinicos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosClinicos_Descripcion",
                table: "ServiciosClinicos",
                column: "Descripcion");

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
                name: "IX_ServiciosIncluidosArea_AreaClinicaId",
                table: "ServiciosIncluidosArea",
                column: "AreaClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosIncluidosArea_AreaClinicaId_Activo",
                table: "ServiciosIncluidosArea",
                columns: new[] { "AreaClinicaId", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosIncluidosArea_ServicioClinicoId",
                table: "ServiciosIncluidosArea",
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
                name: "IX_ServiciosInsumoRecetas_ServicioClinicoId_InsumoId",
                table: "ServiciosInsumoRecetas",
                columns: new[] { "ServicioClinicoId", "InsumoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosInsumoRecetas_UnidadMedidaConsumoId",
                table: "ServiciosInsumoRecetas",
                column: "UnidadMedidaConsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosSugerencias_ServicioOrigenId",
                table: "ServiciosSugerencias",
                column: "ServicioOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosSugerencias_ServicioOrigenId_ServicioSugeridoId",
                table: "ServiciosSugerencias",
                columns: new[] { "ServicioOrigenId", "ServicioSugeridoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosSugerencias_ServicioSugeridoId",
                table: "ServiciosSugerencias",
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
                name: "IX_StocksSede_InsumoId_SedeId",
                table: "StocksSede",
                columns: new[] { "InsumoId", "SedeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StocksSede_SedeId",
                table: "StocksSede",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_StocksSede_SedeId_StockActual",
                table: "StocksSede",
                columns: new[] { "SedeId", "StockActual" });

            migrationBuilder.CreateIndex(
                name: "IX_TasasCambio_Activo",
                table: "TasasCambio",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_TasasCambio_Activo_Fecha",
                table: "TasasCambio",
                columns: new[] { "Activo", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_TasasCambio_Fecha",
                table: "TasasCambio",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_TiposIngreso_Codigo",
                table: "TiposIngreso",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposServicio_Codigo",
                table: "TiposServicio",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposServicio_Nombre",
                table: "TiposServicio",
                column: "Nombre");

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
                name: "IX_TransferenciasReposicionStock_SedeOrigenId_SedeDestinoId_Fec~",
                table: "TransferenciasReposicionStock",
                columns: new[] { "SedeOrigenId", "SedeDestinoId", "FechaTransferencia" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciasReposicionStock_UsuarioIdentityId",
                table: "TransferenciasReposicionStock",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_TriagesEnfermeria_CuentaServicioId",
                table: "TriagesEnfermeria",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_TriagesEnfermeria_CuentaServicioId1",
                table: "TriagesEnfermeria",
                column: "CuentaServicioId1");

            migrationBuilder.CreateIndex(
                name: "IX_TriagesEnfermeria_FechaRegistro",
                table: "TriagesEnfermeria",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_TriagesEnfermeria_UsuarioRegistroId",
                table: "TriagesEnfermeria",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosMedicos_FechaHoraToma",
                table: "TurnosMedicos",
                column: "FechaHoraToma");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosMedicos_IncidenciaIgnoradaId",
                table: "TurnosMedicos",
                column: "IncidenciaIgnoradaId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosMedicos_MedicoId",
                table: "TurnosMedicos",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnosMedicos_MedicoId_FechaHoraToma",
                table: "TurnosMedicos",
                columns: new[] { "MedicoId", "FechaHoraToma" });

            migrationBuilder.CreateIndex(
                name: "IX_TurnosMedicos_PacienteId",
                table: "TurnosMedicos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_Activo",
                table: "UnidadesMedida",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_Codigo",
                table: "UnidadesMedida",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionesFisicas_CuentaServicioId",
                table: "ValoracionesFisicas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionesFisicas_FechaRegistro",
                table: "ValoracionesFisicas",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionesFisicas_UsuarioRegistroId",
                table: "ValoracionesFisicas",
                column: "UsuarioRegistroId");
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
                name: "CajaDeclaracionesMetodos");

            migrationBuilder.DropTable(
                name: "CierresInventarioDetalles");

            migrationBuilder.DropTable(
                name: "CirugiaLogs");

            migrationBuilder.DropTable(
                name: "CirugiaMedicosHonorarios");

            migrationBuilder.DropTable(
                name: "CirugiaObservacionesHistorial");

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
                name: "DetalleServiciosMedicosResponsables");

            migrationBuilder.DropTable(
                name: "DetallesPago");

            migrationBuilder.DropTable(
                name: "DocumentLogs");

            migrationBuilder.DropTable(
                name: "ErrorTickets");

            migrationBuilder.DropTable(
                name: "GarantiasItems");

            migrationBuilder.DropTable(
                name: "HistorialModificacionCuentaDetalles");

            migrationBuilder.DropTable(
                name: "HonorariosConfig");

            migrationBuilder.DropTable(
                name: "HonorariosMedicosServicios");

            migrationBuilder.DropTable(
                name: "HonorariumMappingRules");

            migrationBuilder.DropTable(
                name: "HorariosAtencionMedicos");

            migrationBuilder.DropTable(
                name: "InsumosCirugiaPaciente");

            migrationBuilder.DropTable(
                name: "InsumosPrincipiosActivos");

            migrationBuilder.DropTable(
                name: "LogsAsignacionHonorario");

            migrationBuilder.DropTable(
                name: "MovimientosInsumo");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrdenCirugiaRequisitos");

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
                name: "RegistroAuditoriaIncidencias");

            migrationBuilder.DropTable(
                name: "ReservasTemporales");

            migrationBuilder.DropTable(
                name: "ServiciosIncluidosArea");

            migrationBuilder.DropTable(
                name: "ServiciosInsumoRecetas");

            migrationBuilder.DropTable(
                name: "ServiciosSugerencias");

            migrationBuilder.DropTable(
                name: "SolicitudesInsumosCirugia");

            migrationBuilder.DropTable(
                name: "StocksSede");

            migrationBuilder.DropTable(
                name: "TasasCambio");

            migrationBuilder.DropTable(
                name: "TransferenciasReposicionStock");

            migrationBuilder.DropTable(
                name: "TriagesEnfermeria");

            migrationBuilder.DropTable(
                name: "ValoracionesFisicas");

            migrationBuilder.DropTable(
                name: "CierresInventario");

            migrationBuilder.DropTable(
                name: "EstadosCitaMedica");

            migrationBuilder.DropTable(
                name: "MotivosAutorizacion");

            migrationBuilder.DropTable(
                name: "CatalogoMetodosPago");

            migrationBuilder.DropTable(
                name: "RecibosFacturas");

            migrationBuilder.DropTable(
                name: "CuentasPorCobrar");

            migrationBuilder.DropTable(
                name: "HistorialModificacionCuentas");

            migrationBuilder.DropTable(
                name: "PrincipiosActivos");

            migrationBuilder.DropTable(
                name: "DetallesServicioCuenta");

            migrationBuilder.DropTable(
                name: "RequisitosCirugia");

            migrationBuilder.DropTable(
                name: "OrdenesCompraInventario");

            migrationBuilder.DropTable(
                name: "PedidosInterSede");

            migrationBuilder.DropTable(
                name: "IncidenciasHorario");

            migrationBuilder.DropTable(
                name: "TurnosMedicos");

            migrationBuilder.DropTable(
                name: "OrdenesCirugia");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "Monedas");

            migrationBuilder.DropTable(
                name: "CajasDiarias");

            migrationBuilder.DropTable(
                name: "EstadosFiscales");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "CuentasServicios");

            migrationBuilder.DropTable(
                name: "CategoriasInsumo");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");

            migrationBuilder.DropTable(
                name: "EstadosCaja");

            migrationBuilder.DropTable(
                name: "AreasClinicas");

            migrationBuilder.DropTable(
                name: "EstadosCuenta");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "PacientesAdmision");

            migrationBuilder.DropTable(
                name: "SegurosConvenios");

            migrationBuilder.DropTable(
                name: "TiposIngreso");

            migrationBuilder.DropTable(
                name: "UsuarioHospital");

            migrationBuilder.DropTable(
                name: "ClasificacionesAreas");

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
