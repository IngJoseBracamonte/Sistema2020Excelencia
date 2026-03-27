using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V11_ModernIdentity_AutoStub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // V11.2 Fix: Eliminar FKs existentes que dependen de PacienteId (int) antes de la conversión a GUID
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesDeServicio_PacientesAdmision_PacienteId",
                table: "OrdenesDeServicio");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPago_CuentasPorCobrar_CuentaPorCobrarId",
                table: "DetallesPago");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPago_CuentaPorCobrarId",
                table: "DetallesPago");

            migrationBuilder.DropColumn(
                name: "CuentaPorCobrarId",
                table: "DetallesPago");

            migrationBuilder.DropColumn(
                name: "SaldoPendienteBase",
                table: "CuentasPorCobrar");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "CuentasServicios",
                newName: "FechaCarga");

            migrationBuilder.RenameColumn(
                name: "FechaEmision",
                table: "CuentasPorCobrar",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "EstadoAtencion",
                table: "CitasMedicas",
                newName: "Estado");

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "RecibosFacturas",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PacientesAdmision",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IdPacienteLegacy",
                table: "PacientesAdmision",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "OrdenesDeServicio",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "CuentasServicios",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCarga",
                table: "CuentasServicios",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "CuentasPorCobrar",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "PacienteId",
                table: "CitasMedicas",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_PacientesAdmision_IdPacienteLegacy",
                table: "PacientesAdmision",
                column: "IdPacienteLegacy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_CuentaServicioId",
                table: "CitasMedicas",
                column: "CuentaServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_MedicoId",
                table: "CitasMedicas",
                column: "MedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CitasMedicas_CuentasServicios_CuentaServicioId",
                table: "CitasMedicas",
                column: "CuentaServicioId",
                principalTable: "CuentasServicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CitasMedicas_Medicos_MedicoId",
                table: "CitasMedicas",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // V11.2 Fix: Recrear FK de Ordenes de Servicio con el nuevo tipo GUID
            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesDeServicio_PacientesAdmision_PacienteId",
                table: "OrdenesDeServicio",
                column: "PacienteId",
                principalTable: "PacientesAdmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitasMedicas_CuentasServicios_CuentaServicioId",
                table: "CitasMedicas");

            migrationBuilder.DropForeignKey(
                name: "FK_CitasMedicas_Medicos_MedicoId",
                table: "CitasMedicas");

            migrationBuilder.DropIndex(
                name: "IX_PacientesAdmision_IdPacienteLegacy",
                table: "PacientesAdmision");

            migrationBuilder.DropIndex(
                name: "IX_CitasMedicas_CuentaServicioId",
                table: "CitasMedicas");

            migrationBuilder.DropIndex(
                name: "IX_CitasMedicas_MedicoId",
                table: "CitasMedicas");

            migrationBuilder.DropColumn(
                name: "IdPacienteLegacy",
                table: "PacientesAdmision");

            migrationBuilder.DropColumn(
                name: "UsuarioCarga",
                table: "CuentasServicios");

            migrationBuilder.RenameColumn(
                name: "FechaCarga",
                table: "CuentasServicios",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "CuentasPorCobrar",
                newName: "FechaEmision");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "CitasMedicas",
                newName: "EstadoAtencion");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "RecibosFacturas",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PacientesAdmision",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "OrdenesDeServicio",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "CuentaPorCobrarId",
                table: "DetallesPago",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "CuentasServicios",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "CuentasPorCobrar",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoPendienteBase",
                table: "CuentasPorCobrar",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "CitasMedicas",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPago_CuentaPorCobrarId",
                table: "DetallesPago",
                column: "CuentaPorCobrarId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPago_CuentasPorCobrar_CuentaPorCobrarId",
                table: "DetallesPago",
                column: "CuentaPorCobrarId",
                principalTable: "CuentasPorCobrar",
                principalColumn: "Id");
        }
    }
}
