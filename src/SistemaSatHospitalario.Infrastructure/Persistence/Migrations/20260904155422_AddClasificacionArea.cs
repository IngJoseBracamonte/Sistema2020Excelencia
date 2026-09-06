using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClasificacionArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CierresInventarioDetalles_CierresInventario_CierreInventario~",
                table: "CierresInventarioDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumosServiciosRealizados_DetallesServicioCuenta_DetalleSe~",
                table: "ConsumosServiciosRealizados");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesServiciosMedicosResponsables_DetallesServicioCuenta_~",
                table: "DetallesServiciosMedicosResponsables");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_DetallesServicioCuenta_D~",
                table: "HistorialModificacionCuentaDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCue~",
                table: "HistorialModificacionCuentaDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCu~1",
                table: "HistorialModificacionCuentaDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsAsignacionHonorario_DetallesServicioCuenta_DetalleServic~",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropIndex(
                name: "IX_PacientesAdmision_IdPacienteLegacy",
                table: "PacientesAdmision");

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: "30000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: "30000000-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: "30000000-0000-0000-0000-000000000003");

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: "30000000-0000-0000-0000-000000000004");

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: "30000000-0000-0000-0000-000000000005");

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: "30000000-0000-0000-0000-000000000006");

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: "50000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: "50000000-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: "50000000-0000-0000-0000-000000000003");

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: "50000000-0000-0000-0000-000000000004");

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: "50000000-0000-0000-0000-000000000005");

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: "50000000-0000-0000-0000-000000000006");

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: "40000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: "40000000-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: "40000000-0000-0000-0000-000000000003");

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: "40000000-0000-0000-0000-000000000004");

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: "40000000-0000-0000-0000-000000000005");

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: "40000000-0000-0000-0000-000000000006");

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: "40000000-0000-0000-0000-000000000007");

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: "10000000-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: "10000000-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: "10000000-0000-0000-0000-000000000003");

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: "10000000-0000-0000-0000-000000000004");

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: "10000000-0000-0000-0000-000000000005");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCue~",
                table: "HistorialModificacionCuentaDetalles",
                newName: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCuentaId");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCu~1",
                table: "HistorialModificacionCuentaDetalles",
                newName: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCuentaId1");

            migrationBuilder.AlterColumn<string>(
                name: "ViaAerea",
                table: "ValoracionesFisicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Ventilacion",
                table: "ValoracionesFisicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioRegistro",
                table: "ValoracionesFisicas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Pupilas",
                table: "ValoracionesFisicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Pulso",
                table: "ValoracionesFisicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PielMucosas",
                table: "ValoracionesFisicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Pertenencias",
                table: "ValoracionesFisicas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "LlenadoCapilar",
                table: "ValoracionesFisicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "ValoracionesFisicas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoConciencia",
                table: "ValoracionesFisicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "ValoracionesFisicas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "AntecedentesMedicos",
                table: "ValoracionesFisicas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Alergias",
                table: "ValoracionesFisicas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "AccesosVenosos",
                table: "ValoracionesFisicas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ValoracionesFisicas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Simbolo",
                table: "UnidadesMedida",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "UnidadesMedida",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "EsFraccionable",
                table: "UnidadesMedida",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "UnidadesMedida",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "UnidadesMedida",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "TurnosMedicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "TurnosMedicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "IncidenciaIgnoradaId",
                table: "TurnosMedicos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IgnorandoIncidencia",
                table: "TurnosMedicos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaHoraToma",
                table: "TurnosMedicos",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TurnosMedicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioRegistro",
                table: "TriagesEnfermeria",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TensionArterial",
                table: "TriagesEnfermeria",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "MotivoConsulta",
                table: "TriagesEnfermeria",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "TriagesEnfermeria",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionRapida",
                table: "TriagesEnfermeria",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionDetallada",
                table: "TriagesEnfermeria",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "TriagesEnfermeria",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TriagesEnfermeria",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "TransferenciasReposicionStock",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeOrigenId",
                table: "TransferenciasReposicionStock",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeDestinoId",
                table: "TransferenciasReposicionStock",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "TransferenciasReposicionStock",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "TransferenciasReposicionStock",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "TransferenciasReposicionStock",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaTransferencia",
                table: "TransferenciasReposicionStock",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TransferenciasReposicionStock",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "TiposServicio",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "TiposServicio",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "TiposIngreso",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "TiposIngreso",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "TiposIngreso",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "TasaCambio",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "TasaCambio",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TasaCambio",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeId",
                table: "StocksSede",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "StocksSede",
                type: "datetime2",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldRowVersion: true,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "StocksSede",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "StocksSede",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioSolicitud",
                table: "SolicitudesInsumosCirugia",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioDespacho",
                table: "SolicitudesInsumosCirugia",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenCirugiaId",
                table: "SolicitudesInsumosCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "SolicitudesInsumosCirugia",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "SolicitudesInsumosCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaSolicitud",
                table: "SolicitudesInsumosCirugia",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDespacho",
                table: "SolicitudesInsumosCirugia",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EstadoSolicitud",
                table: "SolicitudesInsumosCirugia",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "AlmacenOrigenId",
                table: "SolicitudesInsumosCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "SolicitudesInsumosCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioSugeridoId",
                table: "serviciossugerencias",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioOrigenId",
                table: "serviciossugerencias",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "serviciossugerencias",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UnidadMedidaConsumo",
                table: "ServiciosInsumoRecetas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioClinicoId",
                table: "ServiciosInsumoRecetas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "ServiciosInsumoRecetas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ServiciosInsumoRecetas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "ServiciosIncluidosAreas",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioClinicoId",
                table: "ServiciosIncluidosAreas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaClinicaId",
                table: "ServiciosIncluidosAreas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UnidadMedida",
                table: "ServiciosClinicos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoServicio",
                table: "ServiciosClinicos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioInformeId",
                table: "ServiciosClinicos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "RequiereInventario",
                table: "ServiciosClinicos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "PermiteFraccionamiento",
                table: "ServiciosClinicos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "LegacyMappingId",
                table: "ServiciosClinicos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HonorariumCategory",
                table: "ServiciosClinicos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "HonorarioBase",
                table: "ServiciosClinicos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDesactivacion",
                table: "ServiciosClinicos",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EspecialidadId",
                table: "ServiciosClinicos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EsServicioInforme",
                table: "ServiciosClinicos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "ServiciosClinicos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "DesactivadoPorUsuarioId",
                table: "ServiciosClinicos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "ServiciosClinicos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "ServiciosClinicos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ServiciosClinicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "SegurosConvenios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Rtn",
                table: "SegurosConvenios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "SegurosConvenios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SegurosConvenios",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "SegurosConvenios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "SegurosConvenios",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SegurosConvenios",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Sedes",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "EsPrincipal",
                table: "Sedes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Sedes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Sedes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Sedes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "ReservasTemporales",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "ReservasTemporales",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraPautada",
                table: "ReservasTemporales",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiracionUtc",
                table: "ReservasTemporales",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Comentario",
                table: "ReservasTemporales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ReservasTemporales",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "RequisitosCirugia",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "RequisitosCirugia",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<bool>(
                name: "EsActivo",
                table: "RequisitosCirugia",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "RequisitosCirugia",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "RequisitosCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "TurnoMedicoId",
                table: "RegistroAuditoriaIncidencias",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OperadorId",
                table: "RegistroAuditoriaIncidencias",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "RegistroAuditoriaIncidencias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "IncidenciaIgnoradaId",
                table: "RegistroAuditoriaIncidencias",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaTraza",
                table: "RegistroAuditoriaIncidencias",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "RegistroAuditoriaIncidencias",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioEmision",
                table: "RecibosFacturas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFacturadoUSD",
                table: "RecibosFacturas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "RecibosFacturas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroRecibo",
                table: "RecibosFacturas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroComprobante",
                table: "RecibosFacturas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NroControlFiscal",
                table: "RecibosFacturas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoVueltoUSD",
                table: "RecibosFacturas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaEmision",
                table: "RecibosFacturas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoFiscal",
                table: "RecibosFacturas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "RecibosFacturas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CajaDiariaId",
                table: "RecibosFacturas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "RecibosFacturas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Proveedores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RazonSocial",
                table: "Proveedores",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "RIF",
                table: "Proveedores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "Proveedores",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "Proveedores",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Proveedores",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Proveedores",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PrincipiosActivos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "PrincipiosActivos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PrincipiosActivos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioClinicoId",
                table: "PreciosServicioConvenio",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PreciosServicioConvenio",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PedidoInterSedeId",
                table: "PedidosInterSedeDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "ObservacionDespacho",
                table: "PedidosInterSedeDetalles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "PedidosInterSedeDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PedidosInterSedeDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreador",
                table: "PedidosInterSede",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeSolicitanteId",
                table: "PedidosInterSede",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeProveedoraId",
                table: "PedidosInterSede",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "PedidosInterSede",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRecepcion",
                table: "PedidosInterSede",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDespacho",
                table: "PedidosInterSede",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "PedidosInterSede",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Correlativo",
                table: "PedidosInterSede",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PedidosInterSede",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "PagosProveedores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Referencia",
                table: "PagosProveedores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenCompraId",
                table: "PagosProveedores",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "PagosProveedores",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPago",
                table: "PagosProveedores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaPago",
                table: "PagosProveedores",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PagosProveedores",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "TelefonoContact",
                table: "PacientesAdmision",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreCorto",
                table: "PacientesAdmision",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaNacimiento",
                table: "PacientesAdmision",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "PacientesAdmision",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CedulaPasaporte",
                table: "PacientesAdmision",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PacientesAdmision",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "ValidadorPor",
                table: "OrdenesImagenes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Validada",
                table: "OrdenesImagenes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "TipoServicio",
                table: "OrdenesImagenes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiereValidacion",
                table: "OrdenesImagenes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiereInforme",
                table: "OrdenesImagenes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "ProcesadoPor",
                table: "OrdenesImagenes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteNombre",
                table: "OrdenesImagenes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "OrdenesImagenes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "ObservacionesMedico",
                table: "OrdenesImagenes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoSolicitanteNombre",
                table: "OrdenesImagenes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoSolicitanteId",
                table: "OrdenesImagenes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoInterpreteId",
                table: "OrdenesImagenes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LinkInforme",
                table: "OrdenesImagenes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Informe",
                table: "OrdenesImagenes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaValidacion",
                table: "OrdenesImagenes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaProcesado",
                table: "OrdenesImagenes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "OrdenesImagenes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Estudio",
                table: "OrdenesImagenes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<bool>(
                name: "EsDirecta",
                table: "OrdenesImagenes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaId",
                table: "OrdenesImagenes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OrdenesImagenes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "TipoIngreso",
                table: "OrdenesDeServicio",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<bool>(
                name: "Procesada",
                table: "OrdenesDeServicio",
                type: "bit",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "OrdenesDeServicio",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "NombrePaciente",
                table: "OrdenesDeServicio",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaProcesada",
                table: "OrdenesDeServicio",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "OrdenesDeServicio",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "EstudioSolicitado",
                table: "OrdenesDeServicio",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "OrdenesDeServicio",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(21)",
                oldMaxLength: 21);

            migrationBuilder.AlterColumn<Guid>(
                name: "AsistenteRxId",
                table: "OrdenesDeServicio",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "OrdenesDeServicio",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "ProveedorNombre",
                table: "OrdenesCompraInventario",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProveedorId",
                table: "OrdenesCompraInventario",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "OrdenesCompraInventario",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroFactura",
                table: "OrdenesCompraInventario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaEmision",
                table: "OrdenesCompraInventario",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "OrdenesCompraInventario",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "OrdenesCompraInventario",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "VerificadoPor",
                table: "OrdenesCirugiaRequisitos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RequisitoCirugiaId",
                table: "OrdenesCirugiaRequisitos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenCirugiaId",
                table: "OrdenesCirugiaRequisitos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVerificacion",
                table: "OrdenesCirugiaRequisitos",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Cumplido",
                table: "OrdenesCirugiaRequisitos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "OrdenesCirugiaRequisitos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreacion",
                table: "OrdenesCirugia",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeQuirofanoId",
                table: "OrdenesCirugia",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeOrigenId",
                table: "OrdenesCirugia",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SalaQuirofano",
                table: "OrdenesCirugia",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "OrdenesCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "MotivoCancelacion",
                table: "OrdenesCirugia",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModalidadAnestesia",
                table: "OrdenesCirugia",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "OrdenesCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaHoraProgramada",
                table: "OrdenesCirugia",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "OrdenesCirugia",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "OrdenesCirugia",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "EsAlquilado",
                table: "OrdenesCirugia",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionCirugia",
                table: "OrdenesCirugia",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "OrdenesCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaClinicaOrigenId",
                table: "OrdenesCirugia",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaClinicaId",
                table: "OrdenesCirugia",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "OrdenesCirugia",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUserId",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TargetRole",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "ActionUrl",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "MovimientosInsumo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "MovimientosInsumo",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UnidadMedidaOriginal",
                table: "MovimientosInsumo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeId",
                table: "MovimientosInsumo",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "MovimientosInsumo",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "MovimientosInsumo",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "MovimientosInsumo",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "MovimientosInsumo",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "MotivosAutorizacion",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "MotivosAutorizacion",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MotivosAutorizacion",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Simbolo",
                table: "Monedas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Monedas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "EsBaseUsd",
                table: "Monedas",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Monedas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Medicos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Medicos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "EspecialidadId",
                table: "Medicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Medicos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Medicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioOperador",
                table: "LogsAsignacionHonorario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "TipoAccion",
                table: "LogsAsignacionHonorario",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "LogsAsignacionHonorario",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreServicio",
                table: "LogsAsignacionHonorario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoNuevoNombre",
                table: "LogsAsignacionHonorario",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoNuevoId",
                table: "LogsAsignacionHonorario",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoAnteriorNombre",
                table: "LogsAsignacionHonorario",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoAnteriorId",
                table: "LogsAsignacionHonorario",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAccion",
                table: "LogsAsignacionHonorario",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DetalleServicioId",
                table: "LogsAsignacionHonorario",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "LogsAsignacionHonorario",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Concentracion",
                table: "InsumosPrincipiosActivos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "PrincipioActivoId",
                table: "InsumosPrincipiosActivos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "InsumosPrincipiosActivos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenCirugiaId",
                table: "InsumosCirugiasPacientes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "InsumosCirugiasPacientes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "InsumosCirugiasPacientes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "InsumosCirugiasPacientes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UnidadMedidaBase",
                table: "Insumos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ReactivosCombinados",
                table: "Insumos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PermiteFraccionamiento",
                table: "Insumos",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)",
                oldDefaultValue: (byte)1);

            migrationBuilder.AlterColumn<bool>(
                name: "OcultoEnTraslados",
                table: "Insumos",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Insumos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Insumos",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<string>(
                name: "Indicaciones",
                table: "Insumos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Insumos",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInactivacion",
                table: "Insumos",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Insumos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoriaInsumoId",
                table: "Insumos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Insumos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Medicamento",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Medicamento");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Insumos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "IncidenciasHorario",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Inicio",
                table: "IncidenciasHorario",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fin",
                table: "IncidenciasHorario",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "IncidenciasHorario",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "IncidenciasHorario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreadoPor",
                table: "IncidenciasHorario",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "IncidenciasHorario",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "HorariosAtencionMedicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HoraInicio",
                table: "HorariosAtencionMedicos",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HoraFin",
                table: "HorariosAtencionMedicos",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "HorariosAtencionMedicos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreo",
                table: "HonorariumMappingRules",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "HonorariumMappingRules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "HonorariumMappingRules",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "HonorariumMappingRules",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "HonorariumMappingRules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "HonorariumMappingRules",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioModifico",
                table: "HonorariosMedicosServicios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioId",
                table: "HonorariosMedicosServicios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "HonorariosMedicosServicios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "HonorariosMedicosServicios",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "HonorariosMedicosServicios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioConfiguro",
                table: "HonorariosConfig",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "NotasConfig",
                table: "HonorariosConfig",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoDefaultId",
                table: "HonorariosConfig",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaConfiguracion",
                table: "HonorariosConfig",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoriaServicio",
                table: "HonorariosConfig",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "HonorariosConfig",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "HistorialModificacionCuentas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TipoIngresoNuevo",
                table: "HistorialModificacionCuentas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoIngresoAnterior",
                table: "HistorialModificacionCuentas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteNuevoNombre",
                table: "HistorialModificacionCuentas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteNuevoId",
                table: "HistorialModificacionCuentas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteAnteriorNombre",
                table: "HistorialModificacionCuentas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteAnteriorId",
                table: "HistorialModificacionCuentas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "HistorialModificacionCuentas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "HistorialModificacionCuentas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "ConvenioNuevoNombre",
                table: "HistorialModificacionCuentas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConvenioAnteriorNombre",
                table: "HistorialModificacionCuentas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "HistorialModificacionCuentas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "HistorialModificacionCuentaId1",
                table: "HistorialModificacionCuentaDetalles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "HistorialModificacionCuentaId",
                table: "HistorialModificacionCuentaDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DetalleServicioId",
                table: "HistorialModificacionCuentaDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "HistorialModificacionCuentaDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "GarantiasItems",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "GarantiasItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaPorCobrarId",
                table: "GarantiasItems",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GarantiasItems",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosFiscales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "EstadosFiscales",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "EstadosFiscales",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosCuenta",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "EstadosCuenta",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "EstadosCuenta",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosCitaMedica",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "EstadosCitaMedica",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "EstadosCitaMedica",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosCaja",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "EstadosCaja",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "EstadosCaja",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Especialidades",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Especialidades",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Especialidades",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioAsociado",
                table: "ErrorTickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StackTrace",
                table: "ErrorTickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "ResueltoPor",
                table: "ErrorTickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Resuelto",
                table: "ErrorTickets",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "RequestPath",
                table: "ErrorTickets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "MetodoHTTP",
                table: "ErrorTickets",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "MensajeExcepcion",
                table: "ErrorTickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaResolucion",
                table: "ErrorTickets",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "ErrorTickets",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "ComentariosResolucion",
                table: "ErrorTickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ErrorTickets",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "DocumentLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DocumentLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "DocumentLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceId",
                table: "DocumentLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "DocumentLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "DocumentLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "DocumentLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DocumentLogs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "DetallesServiciosMedicosResponsables",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "DetallesServiciosMedicosResponsables",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DetalleServicioCuentaId",
                table: "DetallesServiciosMedicosResponsables",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DetallesServiciosMedicosResponsables",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioTecnico",
                table: "DetallesServicioCuenta",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCarga",
                table: "DetallesServicioCuenta",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "TipoServicio",
                table: "DetallesServicioCuenta",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioId",
                table: "DetallesServicioCuenta",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<bool>(
                name: "Realizado",
                table: "DetallesServicioCuenta",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoResponsableId",
                table: "DetallesServicioCuenta",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LegacyMappingId",
                table: "DetallesServicioCuenta",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IncluidoEnTarifaBase",
                table: "DetallesServicioCuenta",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Honorario",
                table: "DetallesServicioCuenta",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRealizacion",
                table: "DetallesServicioCuenta",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCarga",
                table: "DetallesServicioCuenta",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DetallePadreId",
                table: "DetallesServicioCuenta",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "DetallesServicioCuenta",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "DetallesServicioCuenta",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoriaHonorario",
                table: "DetallesServicioCuenta",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaClinicaId",
                table: "DetallesServicioCuenta",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DetallesServicioCuenta",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCarga",
                table: "DetallesPago",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenciaBancaria",
                table: "DetallesPago",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReciboFacturaId",
                table: "DetallesPago",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MetodoPagoId",
                table: "DetallesPago",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPago",
                table: "DetallesPago",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaPago",
                table: "DetallesPago",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DetallesPago",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioValidacion",
                table: "CuentasServicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCarga",
                table: "CuentasServicios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioAuditoria",
                table: "CuentasServicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoIngreso",
                table: "CuentasServicios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "SubAreaClinica",
                table: "CuentasServicios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProcesamientoEstado",
                table: "CuentasServicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PersonalRelevo",
                table: "CuentasServicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "CuentasServicios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "CuentasServicios",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaValidacion",
                table: "CuentasServicios",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCierre",
                table: "CuentasServicios",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCarga",
                table: "CuentasServicios",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAuditoria",
                table: "CuentasServicios",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "CuentasServicios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "DestinoPaciente",
                table: "CuentasServicios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaPrincipalId",
                table: "CuentasServicios",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CamaRetenidaId",
                table: "CuentasServicios",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaClinicaId",
                table: "CuentasServicios",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CuentasServicios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioAuditoria",
                table: "CuentasPorCobrar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuienAutorizo",
                table: "CuentasPorCobrar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "CuentasPorCobrar",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAudited",
                table: "CuentasPorCobrar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "InformacionAdicional",
                table: "CuentasPorCobrar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "GarantiaGenerada",
                table: "CuentasPorCobrar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "CuentasPorCobrar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAuditoria",
                table: "CuentasPorCobrar",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "CuentasPorCobrar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorProcedimiento",
                table: "CuentasPorCobrar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "CuentasPorCobrar",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<bool>(
                name: "CompromisoGenerado",
                table: "CuentasPorCobrar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CuentasPorCobrar",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UltimaActualizacion",
                table: "ConvenioPerfilPrecios",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "ConvenioPerfilPrecios",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ConvenioPerfilPrecios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "ConsumosServiciosRealizados",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaConsumo",
                table: "ConsumosServiciosRealizados",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DetalleServicioCuentaId",
                table: "ConsumosServiciosRealizados",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ConsumosServiciosRealizados",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UltimaActualizacion",
                table: "ConfiguracionGeneral",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Rif",
                table: "ConfiguracionGeneral",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "NombreEmpresa",
                table: "ConfiguracionGeneral",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<bool>(
                name: "MostrarDetalleFacturacion",
                table: "ConfiguracionGeneral",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "FacturarLaboratorio",
                table: "ConfiguracionGeneral",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "ClaveSupervisor",
                table: "ConfiguracionGeneral",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ConfiguracionGeneral",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreacion",
                table: "CompromisosPago",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<bool>(
                name: "Omitido",
                table: "CompromisosPago",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "CompromisosPago",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "CompromisosPago",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaPorCobrarId",
                table: "CompromisosPago",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CompromisosPago",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "CitasMedicas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "CitasMedicas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraPautada",
                table: "CitasMedicas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "CitasMedicas",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "CitasMedicas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaServicioId",
                table: "CitasMedicas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Comentario",
                table: "CitasMedicas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaClinicaId",
                table: "CitasMedicas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CitasMedicas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioRegistro",
                table: "CirugiasObservacionesHistorial",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenCirugiaId",
                table: "CirugiasObservacionesHistorial",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "CirugiasObservacionesHistorial",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "CirugiasObservacionesHistorial",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CirugiasObservacionesHistorial",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenCirugiaId",
                table: "CirugiasMedicosHonorarios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "CirugiasMedicosHonorarios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "EspecialidadId",
                table: "CirugiasMedicosHonorarios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<bool>(
                name: "EsCirujanoPrincipal",
                table: "CirugiasMedicosHonorarios",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CirugiasMedicosHonorarios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "CirugiaLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "CirugiaLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenCirugiaId",
                table: "CirugiaLogs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Evento",
                table: "CirugiaLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Detalle",
                table: "CirugiaLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CirugiaLogs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "InsumoId",
                table: "CierresInventarioDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CierreInventarioId",
                table: "CierresInventarioDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CierresInventarioDetalles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "CierresInventario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeId",
                table: "CierresInventario",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "CierresInventario",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCierre",
                table: "CierresInventario",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CierresInventario",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CategoriasInsumo",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "CategoriasInsumo",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "CategoriasInsumo",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "CategoriasInsumo",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CategoriasInsumo",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "CatalogoMetodosPago",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CatalogoMetodosPago",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "EsVuelto",
                table: "CatalogoMetodosPago",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "EsUSD",
                table: "CatalogoMetodosPago",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "CatalogoMetodosPago",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CatalogoMetodosPago",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "CajasDiarias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "CajasDiarias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCierre",
                table: "CajasDiarias",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaApertura",
                table: "CajasDiarias",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "CajasDiarias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CajasDiarias",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "MetodoPagoId",
                table: "CajasDeclaracionesMetodos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CajaDiariaId1",
                table: "CajasDeclaracionesMetodos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CajaDiariaId",
                table: "CajasDeclaracionesMetodos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CajasDeclaracionesMetodos",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "BloqueosHorarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "MedicoId",
                table: "BloqueosHorarios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraPautada",
                table: "BloqueosHorarios",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "BloqueosHorarios",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BloqueosHorarios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioOperador",
                table: "AuditLogsPrecios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<decimal>(
                name: "NuevoHonorario",
                table: "AuditLogsPrecios",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "HonorarioAnterior",
                table: "AuditLogsPrecios",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "AuditLogsPrecios",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DetalleServicioId",
                table: "AuditLogsPrecios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionServicio",
                table: "AuditLogsPrecios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "AutorizadoPor",
                table: "AuditLogsPrecios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AuditLogsPrecios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "AuditLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "OldValue",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValue",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServicioTarifaBaseId",
                table: "AreasClinicas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SedeId",
                table: "AreasClinicas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "AreasClinicas",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "EsSubAreaAlmacenPrincipal",
                table: "AreasClinicas",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "EsAreaAdmision",
                table: "AreasClinicas",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "AreasClinicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaPadreId",
                table: "AreasClinicas",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "AreasClinicas",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AreasClinicas",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)");

            migrationBuilder.AddColumn<Guid>(
                name: "ClasificacionId",
                table: "AreasClinicas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ClasificacionesAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClasificacionesAreas", x => x.Id);
                });

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

            migrationBuilder.UpdateData(
                table: "EstadosCaja",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCaja",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCaja",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCitaMedica",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCitaMedica",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCitaMedica",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCitaMedica",
                keyColumn: "Id",
                keyValue: 4,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCuenta",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCuenta",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCuenta",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosCuenta",
                keyColumn: "Id",
                keyValue: 4,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosFiscales",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosFiscales",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "EstadosFiscales",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 1,
                column: "EsBaseUsd",
                value: true);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 2,
                column: "EsBaseUsd",
                value: false);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 3,
                column: "EsBaseUsd",
                value: false);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 4,
                column: "EsBaseUsd",
                value: false);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 5,
                column: "EsBaseUsd",
                value: false);

            migrationBuilder.UpdateData(
                table: "MotivosAutorizacion",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "MotivosAutorizacion",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "MotivosAutorizacion",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: true);

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

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 4,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 5,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { true, true });

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
                name: "IX_PacientesAdmision_IdPacienteLegacy",
                table: "PacientesAdmision",
                column: "IdPacienteLegacy",
                unique: true,
                filter: "[IdPacienteLegacy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AreasClinicas_ClasificacionId",
                table: "AreasClinicas",
                column: "ClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClasificacionesAreas_Codigo",
                table: "ClasificacionesAreas",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AreasClinicas_ClasificacionesAreas_ClasificacionId",
                table: "AreasClinicas",
                column: "ClasificacionId",
                principalTable: "ClasificacionesAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CierresInventarioDetalles_CierresInventario_CierreInventarioId",
                table: "CierresInventarioDetalles",
                column: "CierreInventarioId",
                principalTable: "CierresInventario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumosServiciosRealizados_DetallesServicioCuenta_DetalleServicioCuentaId",
                table: "ConsumosServiciosRealizados",
                column: "DetalleServicioCuentaId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesServiciosMedicosResponsables_DetallesServicioCuenta_DetalleServicioCuentaId",
                table: "DetallesServiciosMedicosResponsables",
                column: "DetalleServicioCuentaId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_DetallesServicioCuenta_DetalleServicioId",
                table: "HistorialModificacionCuentaDetalles",
                column: "DetalleServicioId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCuentas_HistorialModificacionCuentaId",
                table: "HistorialModificacionCuentaDetalles",
                column: "HistorialModificacionCuentaId",
                principalTable: "HistorialModificacionCuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCuentas_HistorialModificacionCuentaId1",
                table: "HistorialModificacionCuentaDetalles",
                column: "HistorialModificacionCuentaId1",
                principalTable: "HistorialModificacionCuentas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogsAsignacionHonorario_DetallesServicioCuenta_DetalleServicioId",
                table: "LogsAsignacionHonorario",
                column: "DetalleServicioId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreasClinicas_ClasificacionesAreas_ClasificacionId",
                table: "AreasClinicas");

            migrationBuilder.DropForeignKey(
                name: "FK_CierresInventarioDetalles_CierresInventario_CierreInventarioId",
                table: "CierresInventarioDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumosServiciosRealizados_DetallesServicioCuenta_DetalleServicioCuentaId",
                table: "ConsumosServiciosRealizados");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesServiciosMedicosResponsables_DetallesServicioCuenta_DetalleServicioCuentaId",
                table: "DetallesServiciosMedicosResponsables");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_DetallesServicioCuenta_DetalleServicioId",
                table: "HistorialModificacionCuentaDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCuentas_HistorialModificacionCuentaId",
                table: "HistorialModificacionCuentaDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCuentas_HistorialModificacionCuentaId1",
                table: "HistorialModificacionCuentaDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsAsignacionHonorario_DetallesServicioCuenta_DetalleServicioId",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropTable(
                name: "ClasificacionesAreas");

            migrationBuilder.DropIndex(
                name: "IX_PacientesAdmision_IdPacienteLegacy",
                table: "PacientesAdmision");

            migrationBuilder.DropIndex(
                name: "IX_AreasClinicas_ClasificacionId",
                table: "AreasClinicas");

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "AreasClinicas",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CategoriasInsumo",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "RequisitosCirugia",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Sedes",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DropColumn(
                name: "ClasificacionId",
                table: "AreasClinicas");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCuentaId1",
                table: "HistorialModificacionCuentaDetalles",
                newName: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCu~1");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCuentaId",
                table: "HistorialModificacionCuentaDetalles",
                newName: "IX_HistorialModificacionCuentaDetalles_HistorialModificacionCue~");

            migrationBuilder.AlterColumn<string>(
                name: "ViaAerea",
                table: "ValoracionesFisicas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Ventilacion",
                table: "ValoracionesFisicas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioRegistro",
                table: "ValoracionesFisicas",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Pupilas",
                table: "ValoracionesFisicas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Pulso",
                table: "ValoracionesFisicas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PielMucosas",
                table: "ValoracionesFisicas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Pertenencias",
                table: "ValoracionesFisicas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LlenadoCapilar",
                table: "ValoracionesFisicas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "ValoracionesFisicas",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoConciencia",
                table: "ValoracionesFisicas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "ValoracionesFisicas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "AntecedentesMedicos",
                table: "ValoracionesFisicas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Alergias",
                table: "ValoracionesFisicas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AccesosVenosos",
                table: "ValoracionesFisicas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ValoracionesFisicas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Simbolo",
                table: "UnidadesMedida",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "UnidadesMedida",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<byte>(
                name: "EsFraccionable",
                table: "UnidadesMedida",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "UnidadesMedida",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "UnidadesMedida",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "PacienteId",
                table: "TurnosMedicos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "TurnosMedicos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "IncidenciaIgnoradaId",
                table: "TurnosMedicos",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "IgnorandoIncidencia",
                table: "TurnosMedicos",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaHoraToma",
                table: "TurnosMedicos",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TurnosMedicos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioRegistro",
                table: "TriagesEnfermeria",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TensionArterial",
                table: "TriagesEnfermeria",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "MotivoConsulta",
                table: "TriagesEnfermeria",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "TriagesEnfermeria",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionRapida",
                table: "TriagesEnfermeria",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionDetallada",
                table: "TriagesEnfermeria",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "TriagesEnfermeria",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TriagesEnfermeria",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "TransferenciasReposicionStock",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SedeOrigenId",
                table: "TransferenciasReposicionStock",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "SedeDestinoId",
                table: "TransferenciasReposicionStock",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "TransferenciasReposicionStock",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "TransferenciasReposicionStock",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "TransferenciasReposicionStock",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaTransferencia",
                table: "TransferenciasReposicionStock",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TransferenciasReposicionStock",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "TiposServicio",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "TiposServicio",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "TiposIngreso",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "TiposIngreso",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "TiposIngreso",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "TasaCambio",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "TasaCambio",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TasaCambio",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "SedeId",
                table: "StocksSede",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RowVersion",
                table: "StocksSede",
                type: "datetime(6)",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldRowVersion: true,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "StocksSede",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StocksSede",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioSolicitud",
                table: "SolicitudesInsumosCirugia",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioDespacho",
                table: "SolicitudesInsumosCirugia",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrdenCirugiaId",
                table: "SolicitudesInsumosCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "SolicitudesInsumosCirugia",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "SolicitudesInsumosCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaSolicitud",
                table: "SolicitudesInsumosCirugia",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDespacho",
                table: "SolicitudesInsumosCirugia",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EstadoSolicitud",
                table: "SolicitudesInsumosCirugia",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AlmacenOrigenId",
                table: "SolicitudesInsumosCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "SolicitudesInsumosCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ServicioSugeridoId",
                table: "serviciossugerencias",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ServicioOrigenId",
                table: "serviciossugerencias",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "serviciossugerencias",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UnidadMedidaConsumo",
                table: "ServiciosInsumoRecetas",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ServicioClinicoId",
                table: "ServiciosInsumoRecetas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "ServiciosInsumoRecetas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ServiciosInsumoRecetas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "ServiciosIncluidosAreas",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ServicioClinicoId",
                table: "ServiciosIncluidosAreas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "AreaClinicaId",
                table: "ServiciosIncluidosAreas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UnidadMedida",
                table: "ServiciosClinicos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoServicio",
                table: "ServiciosClinicos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ServicioInformeId",
                table: "ServiciosClinicos",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "RequiereInventario",
                table: "ServiciosClinicos",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte>(
                name: "PermiteFraccionamiento",
                table: "ServiciosClinicos",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "LegacyMappingId",
                table: "ServiciosClinicos",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HonorariumCategory",
                table: "ServiciosClinicos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "HonorarioBase",
                table: "ServiciosClinicos",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDesactivacion",
                table: "ServiciosClinicos",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EspecialidadId",
                table: "ServiciosClinicos",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "EsServicioInforme",
                table: "ServiciosClinicos",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "ServiciosClinicos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DesactivadoPorUsuarioId",
                table: "ServiciosClinicos",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "ServiciosClinicos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "ServiciosClinicos",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ServiciosClinicos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "SegurosConvenios",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Rtn",
                table: "SegurosConvenios",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "SegurosConvenios",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SegurosConvenios",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "SegurosConvenios",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "SegurosConvenios",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SegurosConvenios",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Sedes",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<byte>(
                name: "EsPrincipal",
                table: "Sedes",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Sedes",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "Sedes",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Sedes",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "ReservasTemporales",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "ReservasTemporales",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraPautada",
                table: "ReservasTemporales",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiracionUtc",
                table: "ReservasTemporales",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Comentario",
                table: "ReservasTemporales",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ReservasTemporales",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "RequisitosCirugia",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "RequisitosCirugia",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<byte>(
                name: "EsActivo",
                table: "RequisitosCirugia",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "RequisitosCirugia",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "RequisitosCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "TurnoMedicoId",
                table: "RegistroAuditoriaIncidencias",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OperadorId",
                table: "RegistroAuditoriaIncidencias",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "RegistroAuditoriaIncidencias",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IncidenciaIgnoradaId",
                table: "RegistroAuditoriaIncidencias",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaTraza",
                table: "RegistroAuditoriaIncidencias",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "RegistroAuditoriaIncidencias",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioEmision",
                table: "RecibosFacturas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFacturadoUSD",
                table: "RecibosFacturas",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "PacienteId",
                table: "RecibosFacturas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroRecibo",
                table: "RecibosFacturas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroComprobante",
                table: "RecibosFacturas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NroControlFiscal",
                table: "RecibosFacturas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MontoVueltoUSD",
                table: "RecibosFacturas",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaEmision",
                table: "RecibosFacturas",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoFiscal",
                table: "RecibosFacturas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "RecibosFacturas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CajaDiariaId",
                table: "RecibosFacturas",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "RecibosFacturas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Proveedores",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RazonSocial",
                table: "Proveedores",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "RIF",
                table: "Proveedores",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "Proveedores",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "Proveedores",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "Proveedores",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Proveedores",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PrincipiosActivos",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "PrincipiosActivos",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PrincipiosActivos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ServicioClinicoId",
                table: "PreciosServicioConvenio",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PreciosServicioConvenio",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "PedidoInterSedeId",
                table: "PedidosInterSedeDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ObservacionDespacho",
                table: "PedidosInterSedeDetalles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "PedidosInterSedeDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PedidosInterSedeDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreador",
                table: "PedidosInterSede",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SedeSolicitanteId",
                table: "PedidosInterSede",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "SedeProveedoraId",
                table: "PedidosInterSede",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "PedidosInterSede",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRecepcion",
                table: "PedidosInterSede",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDespacho",
                table: "PedidosInterSede",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "PedidosInterSede",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Correlativo",
                table: "PedidosInterSede",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PedidosInterSede",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "PagosProveedores",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Referencia",
                table: "PagosProveedores",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "OrdenCompraId",
                table: "PagosProveedores",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "PagosProveedores",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPago",
                table: "PagosProveedores",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaPago",
                table: "PagosProveedores",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PagosProveedores",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "TelefonoContact",
                table: "PacientesAdmision",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreCorto",
                table: "PacientesAdmision",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaNacimiento",
                table: "PacientesAdmision",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "PacientesAdmision",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CedulaPasaporte",
                table: "PacientesAdmision",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PacientesAdmision",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ValidadorPor",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Validada",
                table: "OrdenesImagenes",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "TipoServicio",
                table: "OrdenesImagenes",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<byte>(
                name: "RequiereValidacion",
                table: "OrdenesImagenes",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte>(
                name: "RequiereInforme",
                table: "OrdenesImagenes",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ProcesadoPor",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteNombre",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PacienteId",
                table: "OrdenesImagenes",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ObservacionesMedico",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoSolicitanteNombre",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoSolicitanteId",
                table: "OrdenesImagenes",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoInterpreteId",
                table: "OrdenesImagenes",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LinkInforme",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Informe",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaValidacion",
                table: "OrdenesImagenes",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaProcesado",
                table: "OrdenesImagenes",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "OrdenesImagenes",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Estudio",
                table: "OrdenesImagenes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "EsDirecta",
                table: "OrdenesImagenes",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CuentaId",
                table: "OrdenesImagenes",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OrdenesImagenes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "TipoIngreso",
                table: "OrdenesDeServicio",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "Procesada",
                table: "OrdenesDeServicio",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteId",
                table: "OrdenesDeServicio",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "NombrePaciente",
                table: "OrdenesDeServicio",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaProcesada",
                table: "OrdenesDeServicio",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "OrdenesDeServicio",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "EstudioSolicitado",
                table: "OrdenesDeServicio",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "OrdenesDeServicio",
                type: "varchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);

            migrationBuilder.AlterColumn<string>(
                name: "AsistenteRxId",
                table: "OrdenesDeServicio",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OrdenesDeServicio",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ProveedorNombre",
                table: "OrdenesCompraInventario",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "ProveedorId",
                table: "OrdenesCompraInventario",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "OrdenesCompraInventario",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroFactura",
                table: "OrdenesCompraInventario",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaEmision",
                table: "OrdenesCompraInventario",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "OrdenesCompraInventario",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OrdenesCompraInventario",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "VerificadoPor",
                table: "OrdenesCirugiaRequisitos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequisitoCirugiaId",
                table: "OrdenesCirugiaRequisitos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OrdenCirugiaId",
                table: "OrdenesCirugiaRequisitos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVerificacion",
                table: "OrdenesCirugiaRequisitos",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Cumplido",
                table: "OrdenesCirugiaRequisitos",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OrdenesCirugiaRequisitos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreacion",
                table: "OrdenesCirugia",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SedeQuirofanoId",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SedeOrigenId",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SalaQuirofano",
                table: "OrdenesCirugia",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteId",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MotivoCancelacion",
                table: "OrdenesCirugia",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModalidadAnestesia",
                table: "OrdenesCirugia",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaHoraProgramada",
                table: "OrdenesCirugia",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "OrdenesCirugia",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "OrdenesCirugia",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<byte>(
                name: "EsAlquilado",
                table: "OrdenesCirugia",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionCirugia",
                table: "OrdenesCirugia",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "AreaClinicaOrigenId",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreaClinicaId",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "Notifications",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "TargetUserId",
                table: "Notifications",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TargetRole",
                table: "Notifications",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<byte>(
                name: "IsRead",
                table: "Notifications",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ActionUrl",
                table: "Notifications",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Notifications",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "MovimientosInsumo",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "MovimientosInsumo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UnidadMedidaOriginal",
                table: "MovimientosInsumo",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "SedeId",
                table: "MovimientosInsumo",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "MovimientosInsumo",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "MovimientosInsumo",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "MovimientosInsumo",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "MovimientosInsumo",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "MotivosAutorizacion",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "MotivosAutorizacion",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MotivosAutorizacion",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Simbolo",
                table: "Monedas",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Monedas",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<byte>(
                name: "EsBaseUsd",
                table: "Monedas",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Monedas",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Medicos",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Medicos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EspecialidadId",
                table: "Medicos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "Medicos",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Medicos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioOperador",
                table: "LogsAsignacionHonorario",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TipoAccion",
                table: "LogsAsignacionHonorario",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "LogsAsignacionHonorario",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreServicio",
                table: "LogsAsignacionHonorario",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoNuevoNombre",
                table: "LogsAsignacionHonorario",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoNuevoId",
                table: "LogsAsignacionHonorario",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoAnteriorNombre",
                table: "LogsAsignacionHonorario",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoAnteriorId",
                table: "LogsAsignacionHonorario",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAccion",
                table: "LogsAsignacionHonorario",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DetalleServicioId",
                table: "LogsAsignacionHonorario",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LogsAsignacionHonorario",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Concentracion",
                table: "InsumosPrincipiosActivos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PrincipioActivoId",
                table: "InsumosPrincipiosActivos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "InsumosPrincipiosActivos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OrdenCirugiaId",
                table: "InsumosCirugiasPacientes",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "InsumosCirugiasPacientes",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "InsumosCirugiasPacientes",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "InsumosCirugiasPacientes",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UnidadMedidaBase",
                table: "Insumos",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ReactivosCombinados",
                table: "Insumos",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "PermiteFraccionamiento",
                table: "Insumos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: (byte)1,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<byte>(
                name: "OcultoEnTraslados",
                table: "Insumos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Insumos",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<byte>(
                name: "IsDeleted",
                table: "Insumos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Indicaciones",
                table: "Insumos",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Insumos",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInactivacion",
                table: "Insumos",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Insumos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CategoriaInsumoId",
                table: "Insumos",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Insumos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Medicamento",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Medicamento");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Insumos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "IncidenciasHorario",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Inicio",
                table: "IncidenciasHorario",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fin",
                table: "IncidenciasHorario",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "IncidenciasHorario",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "IncidenciasHorario",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreadoPor",
                table: "IncidenciasHorario",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "IncidenciasHorario",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "HorariosAtencionMedicos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HoraInicio",
                table: "HorariosAtencionMedicos",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HoraFin",
                table: "HorariosAtencionMedicos",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "HorariosAtencionMedicos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreo",
                table: "HonorariumMappingRules",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "HonorariumMappingRules",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<byte>(
                name: "IsActive",
                table: "HonorariumMappingRules",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "HonorariumMappingRules",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "HonorariumMappingRules",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "HonorariumMappingRules",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioModifico",
                table: "HonorariosMedicosServicios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ServicioId",
                table: "HonorariosMedicosServicios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "HonorariosMedicosServicios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "HonorariosMedicosServicios",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "HonorariosMedicosServicios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioConfiguro",
                table: "HonorariosConfig",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NotasConfig",
                table: "HonorariosConfig",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoDefaultId",
                table: "HonorariosConfig",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaConfiguracion",
                table: "HonorariosConfig",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CategoriaServicio",
                table: "HonorariosConfig",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "HonorariosConfig",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "HistorialModificacionCuentas",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TipoIngresoNuevo",
                table: "HistorialModificacionCuentas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoIngresoAnterior",
                table: "HistorialModificacionCuentas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteNuevoNombre",
                table: "HistorialModificacionCuentas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteNuevoId",
                table: "HistorialModificacionCuentas",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteAnteriorNombre",
                table: "HistorialModificacionCuentas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteAnteriorId",
                table: "HistorialModificacionCuentas",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "HistorialModificacionCuentas",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "HistorialModificacionCuentas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ConvenioNuevoNombre",
                table: "HistorialModificacionCuentas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConvenioAnteriorNombre",
                table: "HistorialModificacionCuentas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "HistorialModificacionCuentas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "HistorialModificacionCuentaId1",
                table: "HistorialModificacionCuentaDetalles",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HistorialModificacionCuentaId",
                table: "HistorialModificacionCuentaDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "DetalleServicioId",
                table: "HistorialModificacionCuentaDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "HistorialModificacionCuentaDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "GarantiasItems",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "GarantiasItems",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CuentaPorCobrarId",
                table: "GarantiasItems",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "GarantiasItems",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosFiscales",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "EstadosFiscales",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "EstadosFiscales",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosCuenta",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "EstadosCuenta",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "EstadosCuenta",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosCitaMedica",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "EstadosCitaMedica",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "EstadosCitaMedica",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadosCaja",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "EstadosCaja",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "EstadosCaja",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Especialidades",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "Especialidades",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Especialidades",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioAsociado",
                table: "ErrorTickets",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StackTrace",
                table: "ErrorTickets",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ResueltoPor",
                table: "ErrorTickets",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Resuelto",
                table: "ErrorTickets",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "RequestPath",
                table: "ErrorTickets",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "MetodoHTTP",
                table: "ErrorTickets",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "MensajeExcepcion",
                table: "ErrorTickets",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaResolucion",
                table: "ErrorTickets",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "ErrorTickets",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ComentariosResolucion",
                table: "ErrorTickets",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ErrorTickets",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "DocumentLogs",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DocumentLogs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "DocumentLogs",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceId",
                table: "DocumentLogs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "DocumentLogs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "DocumentLogs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "DocumentLogs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DocumentLogs",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "DetallesServiciosMedicosResponsables",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "DetallesServiciosMedicosResponsables",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "DetalleServicioCuentaId",
                table: "DetallesServiciosMedicosResponsables",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DetallesServiciosMedicosResponsables",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioTecnico",
                table: "DetallesServicioCuenta",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCarga",
                table: "DetallesServicioCuenta",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TipoServicio",
                table: "DetallesServicioCuenta",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ServicioId",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<byte>(
                name: "Realizado",
                table: "DetallesServicioCuenta",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoResponsableId",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LegacyMappingId",
                table: "DetallesServicioCuenta",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "IncluidoEnTarifaBase",
                table: "DetallesServicioCuenta",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<decimal>(
                name: "Honorario",
                table: "DetallesServicioCuenta",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRealizacion",
                table: "DetallesServicioCuenta",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCarga",
                table: "DetallesServicioCuenta",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DetallePadreId",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "DetallesServicioCuenta",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CategoriaHonorario",
                table: "DetallesServicioCuenta",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreaClinicaId",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DetallesServicioCuenta",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCarga",
                table: "DetallesPago",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenciaBancaria",
                table: "DetallesPago",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReciboFacturaId",
                table: "DetallesPago",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPagoId",
                table: "DetallesPago",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPago",
                table: "DetallesPago",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaPago",
                table: "DetallesPago",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DetallesPago",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioValidacion",
                table: "CuentasServicios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCarga",
                table: "CuentasServicios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioAuditoria",
                table: "CuentasServicios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoIngreso",
                table: "CuentasServicios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SubAreaClinica",
                table: "CuentasServicios",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProcesamientoEstado",
                table: "CuentasServicios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PersonalRelevo",
                table: "CuentasServicios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaValidacion",
                table: "CuentasServicios",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCierre",
                table: "CuentasServicios",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCarga",
                table: "CuentasServicios",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAuditoria",
                table: "CuentasServicios",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "CuentasServicios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DestinoPaciente",
                table: "CuentasServicios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CuentaPrincipalId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CamaRetenidaId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreaClinicaId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioAuditoria",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuienAutorizo",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PacienteId",
                table: "CuentasPorCobrar",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<byte>(
                name: "IsAudited",
                table: "CuentasPorCobrar",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "InformacionAdicional",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "GarantiaGenerada",
                table: "CuentasPorCobrar",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "CuentasPorCobrar",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAuditoria",
                table: "CuentasPorCobrar",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorProcedimiento",
                table: "CuentasPorCobrar",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "CuentasPorCobrar",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<byte>(
                name: "CompromisoGenerado",
                table: "CuentasPorCobrar",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CuentasPorCobrar",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UltimaActualizacion",
                table: "ConvenioPerfilPrecios",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "ConvenioPerfilPrecios",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ConvenioPerfilPrecios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "ConsumosServiciosRealizados",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaConsumo",
                table: "ConsumosServiciosRealizados",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DetalleServicioCuentaId",
                table: "ConsumosServiciosRealizados",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ConsumosServiciosRealizados",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UltimaActualizacion",
                table: "ConfiguracionGeneral",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Rif",
                table: "ConfiguracionGeneral",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NombreEmpresa",
                table: "ConfiguracionGeneral",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "MostrarDetalleFacturacion",
                table: "ConfiguracionGeneral",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte>(
                name: "FacturarLaboratorio",
                table: "ConfiguracionGeneral",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ClaveSupervisor",
                table: "ConfiguracionGeneral",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ConfiguracionGeneral",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreacion",
                table: "CompromisosPago",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "Omitido",
                table: "CompromisosPago",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "CompromisosPago",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "CompromisosPago",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CuentaPorCobrarId",
                table: "CompromisosPago",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CompromisosPago",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "PacienteId",
                table: "CitasMedicas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "CitasMedicas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraPautada",
                table: "CitasMedicas",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "CitasMedicas",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "CitasMedicas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CuentaServicioId",
                table: "CitasMedicas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Comentario",
                table: "CitasMedicas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreaClinicaId",
                table: "CitasMedicas",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CitasMedicas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioRegistro",
                table: "CirugiasObservacionesHistorial",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "OrdenCirugiaId",
                table: "CirugiasObservacionesHistorial",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "CirugiasObservacionesHistorial",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "CirugiasObservacionesHistorial",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CirugiasObservacionesHistorial",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OrdenCirugiaId",
                table: "CirugiasMedicosHonorarios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "CirugiasMedicosHonorarios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "EspecialidadId",
                table: "CirugiasMedicosHonorarios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<byte>(
                name: "EsCirujanoPrincipal",
                table: "CirugiasMedicosHonorarios",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CirugiasMedicosHonorarios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "CirugiaLogs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "CirugiaLogs",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "OrdenCirugiaId",
                table: "CirugiaLogs",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Evento",
                table: "CirugiaLogs",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Detalle",
                table: "CirugiaLogs",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CirugiaLogs",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "InsumoId",
                table: "CierresInventarioDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CierreInventarioId",
                table: "CierresInventarioDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CierresInventarioDetalles",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Usuario",
                table: "CierresInventario",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SedeId",
                table: "CierresInventario",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "CierresInventario",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCierre",
                table: "CierresInventario",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CierresInventario",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CategoriasInsumo",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "CategoriasInsumo",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "CategoriasInsumo",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "CategoriasInsumo",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CategoriasInsumo",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "CatalogoMetodosPago",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CatalogoMetodosPago",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<byte>(
                name: "EsVuelto",
                table: "CatalogoMetodosPago",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte>(
                name: "EsUSD",
                table: "CatalogoMetodosPago",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "CatalogoMetodosPago",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CatalogoMetodosPago",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "CajasDiarias",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NombreUsuario",
                table: "CajasDiarias",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCierre",
                table: "CajasDiarias",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaApertura",
                table: "CajasDiarias",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "CajasDiarias",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CajasDiarias",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MetodoPagoId",
                table: "CajasDeclaracionesMetodos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CajaDiariaId1",
                table: "CajasDeclaracionesMetodos",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CajaDiariaId",
                table: "CajasDeclaracionesMetodos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CajasDeclaracionesMetodos",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Motivo",
                table: "BloqueosHorarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MedicoId",
                table: "BloqueosHorarios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraPautada",
                table: "BloqueosHorarios",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "BloqueosHorarios",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "BloqueosHorarios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioOperador",
                table: "AuditLogsPrecios",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<decimal>(
                name: "NuevoHonorario",
                table: "AuditLogsPrecios",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "HonorarioAnterior",
                table: "AuditLogsPrecios",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "AuditLogsPrecios",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DetalleServicioId",
                table: "AuditLogsPrecios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionServicio",
                table: "AuditLogsPrecios",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "AutorizadoPor",
                table: "AuditLogsPrecios",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AuditLogsPrecios",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AuditLogs",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "AuditLogs",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "OldValue",
                table: "AuditLogs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValue",
                table: "AuditLogs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "AuditLogs",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AuditLogs",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ServicioTarifaBaseId",
                table: "AreasClinicas",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SedeId",
                table: "AreasClinicas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "AreasClinicas",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<byte>(
                name: "EsSubAreaAlmacenPrincipal",
                table: "AreasClinicas",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte>(
                name: "EsAreaAdmision",
                table: "AreasClinicas",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "AreasClinicas",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AreaPadreId",
                table: "AreasClinicas",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Activo",
                table: "AreasClinicas",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AreasClinicas",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "CategoriasInsumo",
                columns: new[] { "Id", "Activo", "Codigo", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { "50000000-0000-0000-0000-000000000001", (byte)1, "MED", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Medicamento" },
                    { "50000000-0000-0000-0000-000000000002", (byte)1, "DESC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Descartable" },
                    { "50000000-0000-0000-0000-000000000003", (byte)1, "MAT-MED", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Material Médico" },
                    { "50000000-0000-0000-0000-000000000004", (byte)1, "REACT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reactivo" },
                    { "50000000-0000-0000-0000-000000000005", (byte)1, "MAT-QX", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Material Quirúrgico" },
                    { "50000000-0000-0000-0000-000000000006", (byte)1, "OTRO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Otro" }
                });

            migrationBuilder.UpdateData(
                table: "EstadosCaja",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCaja",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCaja",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCitaMedica",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCitaMedica",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCitaMedica",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCitaMedica",
                keyColumn: "Id",
                keyValue: 4,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCuenta",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCuenta",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCuenta",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosCuenta",
                keyColumn: "Id",
                keyValue: 4,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosFiscales",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosFiscales",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "EstadosFiscales",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 1,
                column: "EsBaseUsd",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 2,
                column: "EsBaseUsd",
                value: (byte)0);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 3,
                column: "EsBaseUsd",
                value: (byte)0);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 4,
                column: "EsBaseUsd",
                value: (byte)0);

            migrationBuilder.UpdateData(
                table: "Monedas",
                keyColumn: "Id",
                keyValue: 5,
                column: "EsBaseUsd",
                value: (byte)0);

            migrationBuilder.UpdateData(
                table: "MotivosAutorizacion",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "MotivosAutorizacion",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "MotivosAutorizacion",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.InsertData(
                table: "RequisitosCirugia",
                columns: new[] { "Id", "Descripcion", "EsActivo", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { "40000000-0000-0000-0000-000000000001", "Informe de cardiología y electrocardiograma vigente.", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Evaluación Cardiovascular / Riesgo Quirúrgico" },
                    { "40000000-0000-0000-0000-000000000002", "Hematología completa, TP, TPT, Glucemia, Urea, Creatinina y VIH/VDRL.", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exámenes Preoperatorios (Laboratorio)" },
                    { "40000000-0000-0000-0000-000000000003", "Firma del paciente o familiar responsable para procedimiento quirúrgico y anestesia.", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Consentimiento Informado Firmado" },
                    { "40000000-0000-0000-0000-000000000004", "Verificación por enfermería de ayuno estricto.", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ayuno Verificado (Mínimo 8 Horas)" },
                    { "40000000-0000-0000-0000-000000000005", "Aprobación formal firmada por el médico anestesiólogo.", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Valoración Anestésica" },
                    { "40000000-0000-0000-0000-000000000006", "Disponibilidad confirmada con Banco de Sangre (cuando aplique).", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reserva de Sangre / Hemoderivados" },
                    { "40000000-0000-0000-0000-000000000007", "Cama confirmada para el traslado post-quirúrgico.", (byte)1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Disponibilidad de Cama Postoperatoria (UCI / Hosp)" }
                });

            migrationBuilder.InsertData(
                table: "Sedes",
                columns: new[] { "Id", "Activo", "Codigo", "EsPrincipal", "Nombre" },
                values: new object[,]
                {
                    { "10000000-0000-0000-0000-000000000001", (byte)1, "SEDE-PRINCIPAL", (byte)1, "Almacén Principal / Farmacia Central" },
                    { "10000000-0000-0000-0000-000000000002", (byte)1, "SEDE-EMG", (byte)0, "Depósito Emergencia" },
                    { "10000000-0000-0000-0000-000000000003", (byte)1, "SEDE-HOSP", (byte)0, "Depósito Hospitalización" },
                    { "10000000-0000-0000-0000-000000000004", (byte)1, "SEDE-UCI", (byte)0, "Depósito UCI" },
                    { "10000000-0000-0000-0000-000000000005", (byte)1, "SEDE-CIRUGIA", (byte)0, "Quirófano / Pabellón Central" }
                });

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 1,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 2,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 3,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 4,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "TiposIngreso",
                keyColumn: "Id",
                keyValue: 5,
                column: "Activo",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "UnidadesMedida",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Activo", "EsFraccionable" },
                values: new object[] { (byte)1, (byte)1 });

            migrationBuilder.InsertData(
                table: "AreasClinicas",
                columns: new[] { "Id", "Activo", "AreaPadreId", "Codigo", "EsAreaAdmision", "EsSubAreaAlmacenPrincipal", "Estado", "Nombre", "SedeId", "ServicioTarifaBaseId" },
                values: new object[,]
                {
                    { "30000000-0000-0000-0000-000000000001", (byte)1, null, "BOX-1", (byte)1, (byte)0, 1, "Box Emergencia 1", "10000000-0000-0000-0000-000000000002", null },
                    { "30000000-0000-0000-0000-000000000002", (byte)1, null, "HAB-101", (byte)0, (byte)0, 1, "Habitación 101", "10000000-0000-0000-0000-000000000003", null },
                    { "30000000-0000-0000-0000-000000000003", (byte)1, null, "UCI-1", (byte)0, (byte)0, 1, "Cama UCI 1", "10000000-0000-0000-0000-000000000004", null },
                    { "30000000-0000-0000-0000-000000000004", (byte)1, null, "FARMACIA", (byte)0, (byte)0, 1, "Farmacia Central", "10000000-0000-0000-0000-000000000001", null },
                    { "30000000-0000-0000-0000-000000000005", (byte)1, null, "LABORATORIO", (byte)0, (byte)0, 1, "Laboratorio Central", "10000000-0000-0000-0000-000000000001", null },
                    { "30000000-0000-0000-0000-000000000006", (byte)1, null, "QX-1", (byte)0, (byte)0, 1, "Quirófano 1 (Cirugía Mayor)", "10000000-0000-0000-0000-000000000005", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PacientesAdmision_IdPacienteLegacy",
                table: "PacientesAdmision",
                column: "IdPacienteLegacy",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CierresInventarioDetalles_CierresInventario_CierreInventario~",
                table: "CierresInventarioDetalles",
                column: "CierreInventarioId",
                principalTable: "CierresInventario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumosServiciosRealizados_DetallesServicioCuenta_DetalleSe~",
                table: "ConsumosServiciosRealizados",
                column: "DetalleServicioCuentaId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesServiciosMedicosResponsables_DetallesServicioCuenta_~",
                table: "DetallesServiciosMedicosResponsables",
                column: "DetalleServicioCuentaId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_DetallesServicioCuenta_D~",
                table: "HistorialModificacionCuentaDetalles",
                column: "DetalleServicioId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCue~",
                table: "HistorialModificacionCuentaDetalles",
                column: "HistorialModificacionCuentaId",
                principalTable: "HistorialModificacionCuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialModificacionCuentaDetalles_HistorialModificacionCu~1",
                table: "HistorialModificacionCuentaDetalles",
                column: "HistorialModificacionCuentaId1",
                principalTable: "HistorialModificacionCuentas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogsAsignacionHonorario_DetallesServicioCuenta_DetalleServic~",
                table: "LogsAsignacionHonorario",
                column: "DetalleServicioId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
