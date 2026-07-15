using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoServicioTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoServicioId",
                table: "ServiciosClinicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoServicioId",
                table: "DetallesServicioCuenta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TiposServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposServicio", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "TiposServicio",
                columns: new[] { "Id", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, "MED", "Medico" },
                    { 2, "LAB", "Laboratorio" },
                    { 3, "RX", "RX" },
                    { 4, "TOMO", "Tomo" },
                    { 5, "INS", "Insumo" }
                });

            // Actualizar registros existentes basados en la columna de string antigua
            migrationBuilder.Sql("UPDATE DetallesServicioCuenta SET TipoServicioId = 1 WHERE TipoServicio IN ('Medico', 'Medica', 'MÉDICO', 'MÉDICA', 'CONSULTA', 'MEDICO')");
            migrationBuilder.Sql("UPDATE DetallesServicioCuenta SET TipoServicioId = 2 WHERE TipoServicio IN ('Laboratorio', 'LABORATORIO', 'LAB', 'laboratorio')");
            migrationBuilder.Sql("UPDATE DetallesServicioCuenta SET TipoServicioId = 3 WHERE TipoServicio = 'RX'");
            migrationBuilder.Sql("UPDATE DetallesServicioCuenta SET TipoServicioId = 4 WHERE TipoServicio IN ('Tomo', 'TOMO', 'TOMOGRAFIA', 'TOMOGRAFÍA')");
            migrationBuilder.Sql("UPDATE DetallesServicioCuenta SET TipoServicioId = 5 WHERE TipoServicioId = 0 OR TipoServicioId IS NULL");

            migrationBuilder.Sql("UPDATE ServiciosClinicos SET TipoServicioId = 1 WHERE TipoServicio IN ('Medico', 'Medica', 'MÉDICO', 'MÉDICA', 'CONSULTA', 'MEDICO')");
            migrationBuilder.Sql("UPDATE ServiciosClinicos SET TipoServicioId = 2 WHERE TipoServicio IN ('Laboratorio', 'LABORATORIO', 'LAB', 'laboratorio')");
            migrationBuilder.Sql("UPDATE ServiciosClinicos SET TipoServicioId = 3 WHERE TipoServicio = 'RX'");
            migrationBuilder.Sql("UPDATE ServiciosClinicos SET TipoServicioId = 4 WHERE TipoServicio IN ('Tomo', 'TOMO', 'TOMOGRAFIA', 'TOMOGRAFÍA')");
            migrationBuilder.Sql("UPDATE ServiciosClinicos SET TipoServicioId = 5 WHERE TipoServicioId = 0 OR TipoServicioId IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosClinicos_TipoServicioId",
                table: "ServiciosClinicos",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServicioCuenta_TipoServicioId",
                table: "DetallesServicioCuenta",
                column: "TipoServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesServicioCuenta_TiposServicio_TipoServicioId",
                table: "DetallesServicioCuenta",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiciosClinicos_TiposServicio_TipoServicioId",
                table: "ServiciosClinicos",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesServicioCuenta_TiposServicio_TipoServicioId",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiciosClinicos_TiposServicio_TipoServicioId",
                table: "ServiciosClinicos");

            migrationBuilder.DropTable(
                name: "TiposServicio");

            migrationBuilder.DropIndex(
                name: "IX_ServiciosClinicos_TipoServicioId",
                table: "ServiciosClinicos");

            migrationBuilder.DropIndex(
                name: "IX_DetallesServicioCuenta_TipoServicioId",
                table: "DetallesServicioCuenta");

            migrationBuilder.DropColumn(
                name: "TipoServicioId",
                table: "ServiciosClinicos");

            migrationBuilder.DropColumn(
                name: "TipoServicioId",
                table: "DetallesServicioCuenta");
        }
    }
}
