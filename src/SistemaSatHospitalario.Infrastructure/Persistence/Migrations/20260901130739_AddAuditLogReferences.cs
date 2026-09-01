using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_MedicoAnteriorId",
                table: "LogsAsignacionHonorario",
                column: "MedicoAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsAsignacionHonorario_MedicoNuevoId",
                table: "LogsAsignacionHonorario",
                column: "MedicoNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogsPrecios_DetalleServicioId",
                table: "AuditLogsPrecios",
                column: "DetalleServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogsPrecios_DetallesServicioCuenta_DetalleServicioId",
                table: "AuditLogsPrecios",
                column: "DetalleServicioId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsAsignacionHonorario_DetallesServicioCuenta_DetalleServic~",
                table: "LogsAsignacionHonorario",
                column: "DetalleServicioId",
                principalTable: "DetallesServicioCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsAsignacionHonorario_Medicos_MedicoAnteriorId",
                table: "LogsAsignacionHonorario",
                column: "MedicoAnteriorId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LogsAsignacionHonorario_Medicos_MedicoNuevoId",
                table: "LogsAsignacionHonorario",
                column: "MedicoNuevoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogsPrecios_DetallesServicioCuenta_DetalleServicioId",
                table: "AuditLogsPrecios");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsAsignacionHonorario_DetallesServicioCuenta_DetalleServic~",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsAsignacionHonorario_Medicos_MedicoAnteriorId",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsAsignacionHonorario_Medicos_MedicoNuevoId",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropIndex(
                name: "IX_LogsAsignacionHonorario_MedicoAnteriorId",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropIndex(
                name: "IX_LogsAsignacionHonorario_MedicoNuevoId",
                table: "LogsAsignacionHonorario");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogsPrecios_DetalleServicioId",
                table: "AuditLogsPrecios");
        }
    }
}
