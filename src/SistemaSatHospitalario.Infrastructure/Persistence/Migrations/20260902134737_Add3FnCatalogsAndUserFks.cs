using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add3FnCatalogsAndUserFks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioAuditoriaId",
                table: "CuentasPorCobrar",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "MotivoAutorizacionId",
                table: "CompromisosPago",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioCreacionId",
                table: "CompromisosPago",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "CitasMedicas",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = PENDIENTE (EstadoCitaConstants.PendienteId)

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioRegistroId",
                table: "CirugiasObservacionesHistorial",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EstadosCitaMedica",
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
                    table.PrimaryKey("PK_EstadosCitaMedica", x => x.Id);
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
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosAutorizacion", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                table: "MotivosAutorizacion",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Autorizado por Dirección Médica" },
                    { 2, true, "Exoneración por Presidencia" },
                    { 3, true, "Convenio Institucional" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorCobrar_UsuarioAuditoriaId",
                table: "CuentasPorCobrar",
                column: "UsuarioAuditoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPago_MotivoAutorizacionId",
                table: "CompromisosPago",
                column: "MotivoAutorizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPago_UsuarioCreacionId",
                table: "CompromisosPago",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CitasMedicas_EstadoId",
                table: "CitasMedicas",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CirugiasObservacionesHistorial_UsuarioRegistroId",
                table: "CirugiasObservacionesHistorial",
                column: "UsuarioRegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCitaMedica_Codigo",
                table: "EstadosCitaMedica",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotivosAutorizacion_Nombre",
                table: "MotivosAutorizacion",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CitasMedicas_EstadosCitaMedica_EstadoId",
                table: "CitasMedicas",
                column: "EstadoId",
                principalTable: "EstadosCitaMedica",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompromisosPago_MotivosAutorizacion_MotivoAutorizacionId",
                table: "CompromisosPago",
                column: "MotivoAutorizacionId",
                principalTable: "MotivosAutorizacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // ====================================================================
            // BACKFILL DE DATOS EXISTENTES (3FN)
            // ====================================================================

            // 1. CitasMedicas.EstadoId desde el texto legacy Estado
            migrationBuilder.Sql(@"
                UPDATE `CitasMedicas` c
                JOIN `EstadosCitaMedica` e
                  ON UPPER(TRIM(c.`Estado`)) = e.`Codigo`
                  OR (e.`Codigo` = 'CANCELADA' AND UPPER(TRIM(c.`Estado`)) = 'CANCELADO')
                SET c.`EstadoId` = e.`Id`
                WHERE c.`EstadoId` IS NULL OR c.`EstadoId` = 0 OR c.`EstadoId` = 1;
            ");

            // 2. CirugiasObservacionesHistorial.UsuarioRegistroId: limpiar valores
            //    que no sean GUIDs válidos antes de la conversión de tipo ya aplicada.
            //    (El AlterColumn a char(36) ya ocurrió; los valores no-GUID quedan NULL
            //    o truncados — este UPDATE normaliza los que sí parsean.)
            //    Nota: en MySQL 8 la conversión longtext->char(36) deja strings inválidos
            //    como texto truncado; se anulan para evitar falsos positivos.
            migrationBuilder.Sql(@"
                UPDATE `CirugiasObservacionesHistorial`
                SET `UsuarioRegistroId` = NULL
                WHERE `UsuarioRegistroId` IS NOT NULL
                  AND (CHAR_LENGTH(`UsuarioRegistroId`) <> 36
                       OR `UsuarioRegistroId` NOT REGEXP '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$');
            ");

            // 3. CuentasPorCobrar.UsuarioAuditoriaId desde el username legacy
            migrationBuilder.Sql(@"
                UPDATE `CuentasPorCobrar` c
                JOIN `Usuarios` u ON c.`UsuarioAuditoria` = u.`UserName`
                SET c.`UsuarioAuditoriaId` = u.`Id`
                WHERE c.`UsuarioAuditoria` IS NOT NULL
                  AND c.`UsuarioAuditoriaId` IS NULL;
            ");

            // 4. CompromisosPago.UsuarioCreacionId desde el username legacy
            migrationBuilder.Sql(@"
                UPDATE `CompromisosPago` c
                JOIN `Usuarios` u ON c.`UsuarioCreacion` = u.`UserName`
                SET c.`UsuarioCreacionId` = u.`Id`
                WHERE c.`UsuarioCreacion` IS NOT NULL
                  AND c.`UsuarioCreacionId` IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitasMedicas_EstadosCitaMedica_EstadoId",
                table: "CitasMedicas");

            migrationBuilder.DropForeignKey(
                name: "FK_CompromisosPago_MotivosAutorizacion_MotivoAutorizacionId",
                table: "CompromisosPago");

            migrationBuilder.DropTable(
                name: "EstadosCitaMedica");

            migrationBuilder.DropTable(
                name: "MotivosAutorizacion");

            migrationBuilder.DropIndex(
                name: "IX_CuentasPorCobrar_UsuarioAuditoriaId",
                table: "CuentasPorCobrar");

            migrationBuilder.DropIndex(
                name: "IX_CompromisosPago_MotivoAutorizacionId",
                table: "CompromisosPago");

            migrationBuilder.DropIndex(
                name: "IX_CompromisosPago_UsuarioCreacionId",
                table: "CompromisosPago");

            migrationBuilder.DropIndex(
                name: "IX_CitasMedicas_EstadoId",
                table: "CitasMedicas");

            migrationBuilder.DropIndex(
                name: "IX_CirugiasObservacionesHistorial_UsuarioRegistroId",
                table: "CirugiasObservacionesHistorial");

            migrationBuilder.DropColumn(
                name: "UsuarioAuditoriaId",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "MotivoAutorizacionId",
                table: "CompromisosPago");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "CompromisosPago");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "CitasMedicas");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioRegistroId",
                table: "CirugiasObservacionesHistorial",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
