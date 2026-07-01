using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiereInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockActual",
                table: "Insumos");

            migrationBuilder.AddColumn<bool>(
                name: "RequiereInventario",
                table: "ServiciosClinicos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SedeId",
                table: "MovimientosInsumo",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Insumos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Medicamento")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "PermiteFraccionamiento",
                table: "Insumos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SedeId",
                table: "CierresInventario",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "DetallesServiciosMedicosResponsables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DetalleServicioCuentaId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MedicoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Rol = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontoHonorario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesServiciosMedicosResponsables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesServiciosMedicosResponsables_DetallesServicioCuenta_~",
                        column: x => x.DetalleServicioCuentaId,
                        principalTable: "DetallesServicioCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesServiciosMedicosResponsables_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    EsPrincipal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
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
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreasClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreasClinicas_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
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
                    UsuarioCreador = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "longtext", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StocksSede",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InsumoId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SedeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StockActual = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    StockMaximo = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true)
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
                        onDelete: ReferentialAction.Cascade);
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
                    CantidadDespachada = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CantidadRecibida = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInsumo_SedeId",
                table: "MovimientosInsumo",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresInventario_SedeId",
                table: "CierresInventario",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_AreasClinicas_SedeId_Codigo",
                table: "AreasClinicas",
                columns: new[] { "SedeId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServiciosMedicosResponsables_DetalleServicioCuentaId",
                table: "DetallesServiciosMedicosResponsables",
                column: "DetalleServicioCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesServiciosMedicosResponsables_MedicoId",
                table: "DetallesServiciosMedicosResponsables",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_Correlativo",
                table: "PedidosInterSede",
                column: "Correlativo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_SedeProveedoraId",
                table: "PedidosInterSede",
                column: "SedeProveedoraId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSede_SedeSolicitanteId",
                table: "PedidosInterSede",
                column: "SedeSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSedeDetalles_InsumoId",
                table: "PedidosInterSedeDetalles",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosInterSedeDetalles_PedidoInterSedeId",
                table: "PedidosInterSedeDetalles",
                column: "PedidoInterSedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Codigo",
                table: "Sedes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StocksSede_InsumoId",
                table: "StocksSede",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_StocksSede_SedeId_InsumoId",
                table: "StocksSede",
                columns: new[] { "SedeId", "InsumoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CierresInventario_Sedes_SedeId",
                table: "CierresInventario",
                column: "SedeId",
                principalTable: "Sedes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosInsumo_Sedes_SedeId",
                table: "MovimientosInsumo",
                column: "SedeId",
                principalTable: "Sedes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CierresInventario_Sedes_SedeId",
                table: "CierresInventario");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosInsumo_Sedes_SedeId",
                table: "MovimientosInsumo");

            migrationBuilder.DropTable(
                name: "AreasClinicas");

            migrationBuilder.DropTable(
                name: "DetallesServiciosMedicosResponsables");

            migrationBuilder.DropTable(
                name: "PedidosInterSedeDetalles");

            migrationBuilder.DropTable(
                name: "StocksSede");

            migrationBuilder.DropTable(
                name: "PedidosInterSede");

            migrationBuilder.DropTable(
                name: "Sedes");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInsumo_SedeId",
                table: "MovimientosInsumo");

            migrationBuilder.DropIndex(
                name: "IX_CierresInventario_SedeId",
                table: "CierresInventario");

            migrationBuilder.DropColumn(
                name: "RequiereInventario",
                table: "ServiciosClinicos");

            migrationBuilder.DropColumn(
                name: "SedeId",
                table: "MovimientosInsumo");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Insumos");

            migrationBuilder.DropColumn(
                name: "PermiteFraccionamiento",
                table: "Insumos");

            migrationBuilder.DropColumn(
                name: "SedeId",
                table: "CierresInventario");

            migrationBuilder.AddColumn<decimal>(
                name: "StockActual",
                table: "Insumos",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
