using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy.Migrations
{
    /// <inheritdoc />
    public partial class InitialLegacySync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SNAPSHOT PINNING: Left empty by design to secure legacy DB. Let EF think tables were created.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // SNAPSHOT PINNING: Left empty by design to secure legacy DB. 
        }
    }
}
