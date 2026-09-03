using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoFiscalId",
                table: "RecibosFacturas",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = BORRADOR (EstadoFiscalConstants.BorradorId)

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "CuentasServicios",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = ABIERTA (EstadoCuentaConstants.AbiertaId)

            migrationBuilder.AddColumn<int>(
                name: "TipoIngresoId",
                table: "CuentasServicios",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = PARTICULAR (TipoIngresoConstants.ParticularId)

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "CajasDiarias",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = ABIERTA (EstadoCajaConstants.AbiertaId)

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
                name: "EstadosCuenta",
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
                name: "TiposIngreso",
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
                    table.PrimaryKey("PK_TiposIngreso", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_RecibosFacturas_EstadoFiscalId",
                table: "RecibosFacturas",
                column: "EstadoFiscalId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_EstadoId",
                table: "CuentasServicios",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasServicios_TipoIngresoId",
                table: "CuentasServicios",
                column: "TipoIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_CajasDiarias_EstadoId",
                table: "CajasDiarias",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCaja_Codigo",
                table: "EstadosCaja",
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
                name: "IX_TiposIngreso_Codigo",
                table: "TiposIngreso",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CajasDiarias_EstadosCaja_EstadoId",
                table: "CajasDiarias",
                column: "EstadoId",
                principalTable: "EstadosCaja",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasServicios_EstadosCuenta_EstadoId",
                table: "CuentasServicios",
                column: "EstadoId",
                principalTable: "EstadosCuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasServicios_TiposIngreso_TipoIngresoId",
                table: "CuentasServicios",
                column: "TipoIngresoId",
                principalTable: "TiposIngreso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecibosFacturas_EstadosFiscales_EstadoFiscalId",
                table: "RecibosFacturas",
                column: "EstadoFiscalId",
                principalTable: "EstadosFiscales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // ====================================================================
            // BACKFILL DE DATOS EXISTENTES (3FN)
            // ====================================================================

            // 1. CajasDiarias.EstadoId desde el texto legacy Estado
            migrationBuilder.Sql(@"
                UPDATE `CajasDiarias` c
                JOIN `EstadosCaja` e
                  ON UPPER(TRIM(c.`Estado`)) = e.`Codigo`
                  OR (e.`Codigo` = 'CERRADA_POR_ASISTENTE' AND UPPER(TRIM(c.`Estado`)) = 'CERRADAPORASISTENTE')
                SET c.`EstadoId` = e.`Id`;
            ");

            // 2. CuentasServicios.EstadoId desde el texto legacy Estado
            migrationBuilder.Sql(@"
                UPDATE `CuentasServicios` c
                JOIN `EstadosCuenta` e ON UPPER(TRIM(c.`Estado`)) = e.`Codigo`
                SET c.`EstadoId` = e.`Id`;
            ");

            // 3. CuentasServicios.TipoIngresoId desde el texto legacy TipoIngreso
            migrationBuilder.Sql(@"
                UPDATE `CuentasServicios` c
                JOIN `TiposIngreso` t
                  ON UPPER(TRIM(c.`TipoIngreso`)) = t.`Codigo`
                  OR (t.`Codigo` = 'HOSPITALIZACION' AND UPPER(TRIM(c.`TipoIngreso`)) = 'HOSPITALIZACIÓN')
                SET c.`TipoIngresoId` = t.`Id`;
            ");

            // 4. RecibosFacturas.EstadoFiscalId desde el texto legacy EstadoFiscal
            migrationBuilder.Sql(@"
                UPDATE `RecibosFacturas` r
                JOIN `EstadosFiscales` e ON UPPER(TRIM(r.`EstadoFiscal`)) = e.`Codigo`
                SET r.`EstadoFiscalId` = e.`Id`;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CajasDiarias_EstadosCaja_EstadoId",
                table: "CajasDiarias");

            migrationBuilder.DropForeignKey(
                name: "FK_CuentasServicios_EstadosCuenta_EstadoId",
                table: "CuentasServicios");

            migrationBuilder.DropForeignKey(
                name: "FK_CuentasServicios_TiposIngreso_TipoIngresoId",
                table: "CuentasServicios");

            migrationBuilder.DropForeignKey(
                name: "FK_RecibosFacturas_EstadosFiscales_EstadoFiscalId",
                table: "RecibosFacturas");

            migrationBuilder.DropTable(
                name: "EstadosCaja");

            migrationBuilder.DropTable(
                name: "EstadosCuenta");

            migrationBuilder.DropTable(
                name: "EstadosFiscales");

            migrationBuilder.DropTable(
                name: "TiposIngreso");

            migrationBuilder.DropIndex(
                name: "IX_RecibosFacturas_EstadoFiscalId",
                table: "RecibosFacturas");

            migrationBuilder.DropIndex(
                name: "IX_CuentasServicios_EstadoId",
                table: "CuentasServicios");

            migrationBuilder.DropIndex(
                name: "IX_CuentasServicios_TipoIngresoId",
                table: "CuentasServicios");

            migrationBuilder.DropIndex(
                name: "IX_CajasDiarias_EstadoId",
                table: "CajasDiarias");

            migrationBuilder.DropColumn(
                name: "EstadoFiscalId",
                table: "RecibosFacturas");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "TipoIngresoId",
                table: "CuentasServicios");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "CajasDiarias");
        }
    }
}
