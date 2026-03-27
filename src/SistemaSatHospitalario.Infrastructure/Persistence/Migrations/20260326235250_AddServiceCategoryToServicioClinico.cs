using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceCategoryToServicioClinico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "ServiciosClinicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Data Patch: Categorizar ítems existentes (Pachón Pro V5.2)
            migrationBuilder.Sql("UPDATE ServiciosClinicos SET Category = 1 WHERE TipoServicio LIKE '%CONSULTA%' OR TipoServicio LIKE '%MEDIC%' OR TipoServicio LIKE '%GINECO%' OR TipoServicio LIKE '%PEDIAT%'");
            migrationBuilder.Sql("UPDATE ServiciosClinicos SET Category = 3 WHERE TipoServicio LIKE '%RX%' OR TipoServicio LIKE '%IMAGEN%' OR TipoServicio LIKE '%ECO%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "ServiciosClinicos");
        }
    }
}
