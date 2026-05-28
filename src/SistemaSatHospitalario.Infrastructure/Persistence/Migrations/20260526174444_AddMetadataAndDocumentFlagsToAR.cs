using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaSatHospitalario.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMetadataAndDocumentFlagsToAR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP PROCEDURE IF EXISTS AddMetadataColumnsIfNotExist;
                CREATE PROCEDURE AddMetadataColumnsIfNotExist()
                BEGIN
                    DECLARE col_exists INT;
                    
                    -- Check DoctorProcedimiento
                    SELECT COUNT(*) INTO col_exists 
                    FROM information_schema.COLUMNS 
                    WHERE TABLE_SCHEMA = DATABASE() 
                      AND TABLE_NAME = 'CuentasPorCobrar' 
                      AND COLUMN_NAME = 'DoctorProcedimiento';
                      
                    IF col_exists = 0 THEN
                        ALTER TABLE CuentasPorCobrar ADD COLUMN DoctorProcedimiento longtext NULL;
                    END IF;
                    
                    -- Check InformacionAdicional
                    SELECT COUNT(*) INTO col_exists 
                    FROM information_schema.COLUMNS 
                    WHERE TABLE_SCHEMA = DATABASE() 
                      AND TABLE_NAME = 'CuentasPorCobrar' 
                      AND COLUMN_NAME = 'InformacionAdicional';
                      
                    IF col_exists = 0 THEN
                        ALTER TABLE CuentasPorCobrar ADD COLUMN InformacionAdicional longtext NULL;
                    END IF;
                    
                    -- Check QuienAutorizo
                    SELECT COUNT(*) INTO col_exists 
                    FROM information_schema.COLUMNS 
                    WHERE TABLE_SCHEMA = DATABASE() 
                      AND TABLE_NAME = 'CuentasPorCobrar' 
                      AND COLUMN_NAME = 'QuienAutorizo';
                      
                    IF col_exists = 0 THEN
                        ALTER TABLE CuentasPorCobrar ADD COLUMN QuienAutorizo longtext NULL;
                    END IF;
                END;
                CALL AddMetadataColumnsIfNotExist();
                DROP PROCEDURE IF EXISTS AddMetadataColumnsIfNotExist;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorProcedimiento",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "InformacionAdicional",
                table: "CuentasPorCobrar");

            migrationBuilder.DropColumn(
                name: "QuienAutorizo",
                table: "CuentasPorCobrar");
        }
    }
}
