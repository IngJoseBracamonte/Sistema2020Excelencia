using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMetodoPagoReferenceToDetallePago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPago_CatalogoMetodosPago_MetodoPagoId",
                table: "DetallesPago");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPago_CatalogoMetodosPago_MetodoPagoId",
                table: "DetallesPago",
                column: "MetodoPagoId",
                principalTable: "CatalogoMetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPago_CatalogoMetodosPago_MetodoPagoId",
                table: "DetallesPago");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPago_CatalogoMetodosPago_MetodoPagoId",
                table: "DetallesPago",
                column: "MetodoPagoId",
                principalTable: "CatalogoMetodosPago",
                principalColumn: "Id");
        }
    }
}
