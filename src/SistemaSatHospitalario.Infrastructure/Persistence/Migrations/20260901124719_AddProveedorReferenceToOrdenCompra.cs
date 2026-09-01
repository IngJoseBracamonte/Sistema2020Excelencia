using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProveedorReferenceToOrdenCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompraInventario_ProveedorId",
                table: "OrdenesCompraInventario",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesCompraInventario_Proveedores_ProveedorId",
                table: "OrdenesCompraInventario",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesCompraInventario_Proveedores_ProveedorId",
                table: "OrdenesCompraInventario");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesCompraInventario_ProveedorId",
                table: "OrdenesCompraInventario");
        }
    }
}
