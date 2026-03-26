using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConvenioToCompanyModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Dropping foreign keys and indexes that depend on SegurosConvenios.Id
            /*migrationBuilder.DropForeignKey(
                name: "FK_PreciosServicioConvenio_SegurosConvenios_SeguroConvenioId",
                table: "PreciosServicioConvenio");

            migrationBuilder.DropForeignKey(
                name: "FK_ConvenioPerfilPrecios_SegurosConvenios_SeguroConvenioId",
                table: "ConvenioPerfilPrecios");

            migrationBuilder.DropIndex(
                name: "IX_ConvenioPerfilPrecios_SeguroConvenioId_PerfilId",
                table: "ConvenioPerfilPrecios");*/

            /*migrationBuilder.DropColumn(
                name: "PorcentajeCobertura",
                table: "SegurosConvenios");*/

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "SegurosConvenios",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SegurosConvenios",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "SegurosConvenios",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SegurosConvenios",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Rtn",
                table: "SegurosConvenios",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "SegurosConvenios",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            // Restore foreign keys and indexes
            migrationBuilder.CreateIndex(
                name: "IX_ConvenioPerfilPrecios_SeguroConvenioId_PerfilId",
                table: "ConvenioPerfilPrecios",
                columns: new[] { "SeguroConvenioId", "PerfilId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConvenioPerfilPrecios_SegurosConvenios_SeguroConvenioId",
                table: "ConvenioPerfilPrecios",
                column: "SeguroConvenioId",
                principalTable: "SegurosConvenios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreciosServicioConvenio_SegurosConvenios_SeguroConvenioId",
                table: "PreciosServicioConvenio",
                column: "SeguroConvenioId",
                principalTable: "SegurosConvenios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Dropping foreign keys and indexes before reverting Id column change
            migrationBuilder.DropForeignKey(
                name: "FK_PreciosServicioConvenio_SegurosConvenios_SeguroConvenioId",
                table: "PreciosServicioConvenio");

            migrationBuilder.DropForeignKey(
                name: "FK_ConvenioPerfilPrecios_SegurosConvenios_SeguroConvenioId",
                table: "ConvenioPerfilPrecios");

            migrationBuilder.DropIndex(
                name: "IX_ConvenioPerfilPrecios_SeguroConvenioId_PerfilId",
                table: "ConvenioPerfilPrecios");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "SegurosConvenios");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "SegurosConvenios");

            migrationBuilder.DropColumn(
                name: "Rtn",
                table: "SegurosConvenios");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "SegurosConvenios");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "SegurosConvenios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SegurosConvenios",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<decimal>(
                name: "PorcentajeCobertura",
                table: "SegurosConvenios",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            // Re-creating foreign keys and indexes in the legacy state (if possible)
            migrationBuilder.CreateIndex(
                name: "IX_ConvenioPerfilPrecios_SeguroConvenioId_PerfilId",
                table: "ConvenioPerfilPrecios",
                columns: new[] { "SeguroConvenioId", "PerfilId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConvenioPerfilPrecios_SegurosConvenios_SeguroConvenioId",
                table: "ConvenioPerfilPrecios",
                column: "SeguroConvenioId",
                principalTable: "SegurosConvenios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreciosServicioConvenio_SegurosConvenios_SeguroConvenioId",
                table: "PreciosServicioConvenio",
                column: "SeguroConvenioId",
                principalTable: "SegurosConvenios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
