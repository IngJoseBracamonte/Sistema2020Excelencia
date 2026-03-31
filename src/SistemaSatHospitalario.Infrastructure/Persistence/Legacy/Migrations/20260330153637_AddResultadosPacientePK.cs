using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy.Migrations
{
    /// <inheritdoc />
    public partial class AddResultadosPacientePK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EF intentó borrar la llave compuesta que registramos en el InitialSnapshot.
            // Pero como la base de datos real (Legacy) es "keyless" y vaciamos el Snapshot, esto daría error.
            // Por ende, comentamos esta línea para que MySQL simplemente agregue la columna limpia.
            // migrationBuilder.DropPrimaryKey(name: "PK_ResultadosPaciente", table: "ResultadosPaciente");

            migrationBuilder.AddColumn<int>(
                name: "IdResultadoPaciente",
                table: "ResultadosPaciente",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResultadosPaciente",
                table: "ResultadosPaciente",
                column: "IdResultadoPaciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        
        }
    }
}
