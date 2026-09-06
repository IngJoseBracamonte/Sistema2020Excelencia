using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaInsumoReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaInsumoId",
                table: "Insumos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_CategoriaInsumoId",
                table: "Insumos",
                column: "CategoriaInsumoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Insumos_CategoriasInsumo_CategoriaInsumoId",
                table: "Insumos",
                column: "CategoriaInsumoId",
                principalTable: "CategoriasInsumo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insumos_CategoriasInsumo_CategoriaInsumoId",
                table: "Insumos");

            migrationBuilder.DropIndex(
                name: "IX_Insumos_CategoriaInsumoId",
                table: "Insumos");

            migrationBuilder.DropColumn(
                name: "CategoriaInsumoId",
                table: "Insumos");
        }
    }
}
