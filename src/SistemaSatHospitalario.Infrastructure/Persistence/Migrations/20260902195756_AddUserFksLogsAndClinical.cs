using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFksLogsAndClinical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioRegistroId",
                table: "ValoracionesFisicas",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioRegistroId",
                table: "TriagesEnfermeria",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioIdentityId",
                table: "TransferenciasReposicionStock",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioDespachoId",
                table: "SolicitudesInsumosCirugia",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioSolicitudId",
                table: "SolicitudesInsumosCirugia",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioIdentityId",
                table: "ReservasTemporales",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreadorId",
                table: "PedidosInterSede",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioIdentityId",
                table: "PagosProveedores",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "OrdenesCirugia",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetUserGuidId",
                table: "Notifications",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioIdentityId",
                table: "MovimientosInsumo",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioOperadorId",
                table: "LogsAsignacionHonorario",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreoId",
                table: "HonorariumMappingRules",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioModificoId",
                table: "HonorariosMedicosServicios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "ResueltoPorId",
                table: "ErrorTickets",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioAsociadoId",
                table: "ErrorTickets",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioIdentityId",
                table: "DocumentLogs",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioIdentityId",
                table: "CirugiaLogs",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "CierresInventario",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "AutorizadoPorId",
                table: "AuditLogsPrecios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioOperadorId",
                table: "AuditLogsPrecios",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioIdentityId",
                table: "AuditLogs",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionesFisicas_UsuarioRegistroId",
                table: "ValoracionesFisicas",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_TriagesEnfermeria_UsuarioRegistroId",
                table: "TriagesEnfermeria",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciasReposicionStock_UsuarioIdentityId",
                table: "TransferenciasReposicionStock",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInsumosCirugia_UsuarioDespachoId",
                table: "SolicitudesInsumosCirugia",
                column: "UsuarioDespachoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInsumosCirugia_UsuarioSolicitudId",
                table: "SolicitudesInsumosCirugia",
                column: "UsuarioSolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasTemporales_UsuarioIdentityId",
                table: "ReservasTemporales",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_UsuarioCreadorId",
                table: "PedidosInterSede",
                column: "UsuarioCreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosProveedores_UsuarioIdentityId",
                table: "PagosProveedores",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCirugia_UsuarioCreacionId",
                table: "OrdenesCirugia",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetUserGuidId",
                table: "Notifications",
                column: "TargetUserGuidId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_UsuarioIdentityId",
                table: "MovimientosInsumo",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_UsuarioOperadorId",
                table: "LogsAsignacionHonorario",
                column: "UsuarioOperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariumMappingRules_UsuarioCreoId",
                table: "HonorariumMappingRules",
                column: "UsuarioCreoId");

            migrationBuilder.CreateIndex(
                name: "IX_HonorariosMedicosServicios_UsuarioModificoId",
                table: "HonorariosMedicosServicios",
                column: "UsuarioModificoId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTickets_ResueltoPorId",
                table: "ErrorTickets",
                column: "ResueltoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTickets_UsuarioAsociadoId",
                table: "ErrorTickets",
                column: "UsuarioAsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentLogs_UsuarioIdentityId",
                table: "DocumentLogs",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiaLogs_UsuarioIdentityId",
                table: "CirugiaLogs",
                column: "UsuarioIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventario_UsuarioId",
                table: "CierresInventario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsPrecios_AutorizadoPorId",
                table: "AuditLogsPrecios",
                column: "AutorizadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsPrecios_UsuarioOperadorId",
                table: "AuditLogsPrecios",
                column: "UsuarioOperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UsuarioIdentityId",
                table: "AuditLogs",
                column: "UsuarioIdentityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ValoracionesFisicas_UsuarioRegistroId",
                table: "ValoracionesFisicas");

            migrationBuilder.DropIndex(
                name: "IX_TriagesEnfermeria_UsuarioRegistroId",
                table: "TriagesEnfermeria");

            migrationBuilder.DropIndex(
                name: "IX_TransferenciasReposicionStock_UsuarioIdentityId",
                table: "TransferenciasReposicionStock");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesInsumosCirugia_UsuarioDespachoId",
                table: "SolicitudesInsumosCirugia");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesInsumosCirugia_UsuarioSolicitudId",
                table: "SolicitudesInsumosCirugia");

            migrationBuilder.DropIndex(
                name: "IX_ReservasTemporales_UsuarioIdentityId",
                table: "ReservasTemporales");

            migrationBuilder.DropIndex(
                name: "IX_PedidosInterSede_UsuarioCreadorId",
                table: "PedidosInterSede");

            migrationBuilder.DropIndex(
                name: "IX_PagosProveedores_UsuarioIdentityId",
                table: "PagosProveedores");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesCirugia_UsuarioCreacionId",
                table: "OrdenesCirugia");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TargetUserGuidId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInsumo_UsuarioIdentityId",
                table: "MovimientosInsumo");

            migrationBuilder.DropIndex(
                name: "IX_LogsAsignacionHonorario_UsuarioOperadorId",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropIndex(
                name: "IX_HonorariumMappingRules_UsuarioCreoId",
                table: "HonorariumMappingRules");

            migrationBuilder.DropIndex(
                name: "IX_HonorariosMedicosServicios_UsuarioModificoId",
                table: "HonorariosMedicosServicios");

            migrationBuilder.DropIndex(
                name: "IX_ErrorTickets_ResueltoPorId",
                table: "ErrorTickets");

            migrationBuilder.DropIndex(
                name: "IX_ErrorTickets_UsuarioAsociadoId",
                table: "ErrorTickets");

            migrationBuilder.DropIndex(
                name: "IX_DocumentLogs_UsuarioIdentityId",
                table: "DocumentLogs");

            migrationBuilder.DropIndex(
                name: "IX_CirugiaLogs_UsuarioIdentityId",
                table: "CirugiaLogs");

            migrationBuilder.DropIndex(
                name: "IX_CierresInventario_UsuarioId",
                table: "CierresInventario");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogsPrecios_AutorizadoPorId",
                table: "AuditLogsPrecios");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogsPrecios_UsuarioOperadorId",
                table: "AuditLogsPrecios");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UsuarioIdentityId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UsuarioRegistroId",
                table: "ValoracionesFisicas");

            migrationBuilder.DropColumn(
                name: "UsuarioRegistroId",
                table: "TriagesEnfermeria");

            migrationBuilder.DropColumn(
                name: "UsuarioIdentityId",
                table: "TransferenciasReposicionStock");

            migrationBuilder.DropColumn(
                name: "UsuarioDespachoId",
                table: "SolicitudesInsumosCirugia");

            migrationBuilder.DropColumn(
                name: "UsuarioSolicitudId",
                table: "SolicitudesInsumosCirugia");

            migrationBuilder.DropColumn(
                name: "UsuarioIdentityId",
                table: "ReservasTemporales");

            migrationBuilder.DropColumn(
                name: "UsuarioCreadorId",
                table: "PedidosInterSede");

            migrationBuilder.DropColumn(
                name: "UsuarioIdentityId",
                table: "PagosProveedores");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "OrdenesCirugia");

            migrationBuilder.DropColumn(
                name: "TargetUserGuidId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UsuarioIdentityId",
                table: "MovimientosInsumo");

            migrationBuilder.DropColumn(
                name: "UsuarioOperadorId",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropColumn(
                name: "UsuarioCreoId",
                table: "HonorariumMappingRules");

            migrationBuilder.DropColumn(
                name: "UsuarioModificoId",
                table: "HonorariosMedicosServicios");

            migrationBuilder.DropColumn(
                name: "ResueltoPorId",
                table: "ErrorTickets");

            migrationBuilder.DropColumn(
                name: "UsuarioAsociadoId",
                table: "ErrorTickets");

            migrationBuilder.DropColumn(
                name: "UsuarioIdentityId",
                table: "DocumentLogs");

            migrationBuilder.DropColumn(
                name: "UsuarioIdentityId",
                table: "CirugiaLogs");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "CierresInventario");

            migrationBuilder.DropColumn(
                name: "AutorizadoPorId",
                table: "AuditLogsPrecios");

            migrationBuilder.DropColumn(
                name: "UsuarioOperadorId",
                table: "AuditLogsPrecios");

            migrationBuilder.DropColumn(
                name: "UsuarioIdentityId",
                table: "AuditLogs");
        }
    }
}
