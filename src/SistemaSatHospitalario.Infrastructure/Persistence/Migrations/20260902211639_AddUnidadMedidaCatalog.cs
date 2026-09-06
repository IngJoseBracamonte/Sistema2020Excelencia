using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnidadMedidaCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnidadMedidaConsumoId",
                table: "ServiciosInsumoRecetas",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = UNIDAD (UnidadMedidaConstants.UnidadId)

            migrationBuilder.AddColumn<int>(
                name: "UnidadMedidaOriginalId",
                table: "MovimientosInsumo",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = UNIDAD (UnidadMedidaConstants.UnidadId)

            migrationBuilder.AddColumn<int>(
                name: "UnidadMedidaId",
                table: "Insumos",
                type: "int",
                nullable: false,
                defaultValue: 1); // 1 = UNIDAD (UnidadMedidaConstants.UnidadId)

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
                    EsFraccionable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosInsumoRecetas_UnidadMedidaConsumoId",
                table: "ServiciosInsumoRecetas",
                column: "UnidadMedidaConsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_UnidadMedidaOriginalId",
                table: "MovimientosInsumo",
                column: "UnidadMedidaOriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_UnidadMedidaId",
                table: "Insumos",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_Codigo",
                table: "UnidadesMedida",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Insumos_UnidadesMedida_UnidadMedidaId",
                table: "Insumos",
                column: "UnidadMedidaId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosInsumo_UnidadesMedida_UnidadMedidaOriginalId",
                table: "MovimientosInsumo",
                column: "UnidadMedidaOriginalId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosInsumoRecetas_UnidadesMedida_UnidadMedidaConsumoId",
                table: "ServiciosInsumoRecetas",
                column: "UnidadMedidaConsumoId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // ====================================================================
            // BACKFILL DE DATOS EXISTENTES (3FN)
            // El enum legacy se persiste como varchar(20) con el nombre del valor
            // (UNIDAD, KG, G, DG, MG, L, ML). El backfill mapea por código.
            // ====================================================================

            // 1. Insumos.UnidadMedidaId desde el enum legacy UnidadMedidaBase
            migrationBuilder.Sql(@"
                UPDATE `Insumos` i
                JOIN `UnidadesMedida` u ON UPPER(TRIM(i.`UnidadMedidaBase`)) = u.`Codigo`
                SET i.`UnidadMedidaId` = u.`Id`;
            ");

            // 2. ServiciosInsumoRecetas.UnidadMedidaConsumoId desde el enum legacy
            migrationBuilder.Sql(@"
                UPDATE `ServiciosInsumoRecetas` r
                JOIN `UnidadesMedida` u ON UPPER(TRIM(r.`UnidadMedidaConsumo`)) = u.`Codigo`
                SET r.`UnidadMedidaConsumoId` = u.`Id`;
            ");

            // 3. MovimientosInsumo.UnidadMedidaOriginalId desde el enum legacy
            migrationBuilder.Sql(@"
                UPDATE `MovimientosInsumo` m
                JOIN `UnidadesMedida` u ON UPPER(TRIM(m.`UnidadMedidaOriginal`)) = u.`Codigo`
                SET m.`UnidadMedidaOriginalId` = u.`Id`;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insumos_UnidadesMedida_UnidadMedidaId",
                table: "Insumos");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosInsumo_UnidadesMedida_UnidadMedidaOriginalId",
                table: "MovimientosInsumo");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosInsumoRecetas_UnidadesMedida_UnidadMedidaConsumoId",
                table: "ServiciosInsumoRecetas");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");

            migrationBuilder.DropIndex(
                name: "IX_ServiciosInsumoRecetas_UnidadMedidaConsumoId",
                table: "ServiciosInsumoRecetas");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInsumo_UnidadMedidaOriginalId",
                table: "MovimientosInsumo");

            migrationBuilder.DropIndex(
                name: "IX_Insumos_UnidadMedidaId",
                table: "Insumos");

            migrationBuilder.DropColumn(
                name: "UnidadMedidaConsumoId",
                table: "ServiciosInsumoRecetas");

            migrationBuilder.DropColumn(
                name: "UnidadMedidaOriginalId",
                table: "MovimientosInsumo");

            migrationBuilder.DropColumn(
                name: "UnidadMedidaId",
                table: "Insumos");
        }
    }
}
