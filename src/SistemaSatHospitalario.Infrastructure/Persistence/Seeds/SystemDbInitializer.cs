using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Identity.Seeds;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;
using System.Linq;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Seeds
{
    public class SystemDbInitializer : IDatabaseInitializer
    {
        private readonly SatHospitalarioDbContext _context;
        private readonly ILegacyLabRepository _legacyRepository;
        private readonly ILogger<SystemDbInitializer> _logger;

        public SystemDbInitializer(
            SatHospitalarioDbContext context, 
            ILegacyLabRepository legacyRepository,
            ILogger<SystemDbInitializer> logger)
        {
            _context = context;
            _legacyRepository = legacyRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Ignorando aplicación de migraciones para System Database (inicialización manual de DB).");

                // Self-healing: Ensure Direccion column exists in PacientesAdmision table (V12.1 Requirement)
                try
                {
                    bool hasDireccion = false;
                    var conn = _context.Database.GetDbConnection();
                    bool closeConnection = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConnection = true;
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                        if (_context.Database.IsSqlite())
                        {
                            cmd.CommandText = "PRAGMA table_info(PacientesAdmision);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var colName = reader["name"]?.ToString();
                                    if (colName != null && colName.Equals("Direccion", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasDireccion = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `PacientesAdmision` LIKE 'Direccion';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    hasDireccion = true;
                                }
                            }
                        }
                    }
                    if (closeConnection)
                    {
                        await conn.CloseAsync();
                    }

                    if (!hasDireccion)
                    {
                        _logger.LogInformation("La columna 'Direccion' no existe en PacientesAdmision. Ejecutando ALTER TABLE...");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `PacientesAdmision` ADD COLUMN `Direccion` VARCHAR(500) NULL;");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear la columna 'Direccion' en PacientesAdmision.");
                }

                // Self-healing: Ensure AreaClinicaId column exists in CitasMedicas table (V16.3 Requirement)
                try
                {
                    bool hasAreaClinicaId = false;
                    var conn = _context.Database.GetDbConnection();
                    bool closeConnection = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConnection = true;
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                        if (_context.Database.IsSqlite())
                        {
                            cmd.CommandText = "PRAGMA table_info(CitasMedicas);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var colName = reader["name"]?.ToString();
                                    if (colName != null && colName.Equals("AreaClinicaId", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasAreaClinicaId = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `CitasMedicas` LIKE 'AreaClinicaId';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    hasAreaClinicaId = true;
                                }
                            }
                        }
                    }
                    if (closeConnection)
                    {
                        await conn.CloseAsync();
                    }

                    if (!hasAreaClinicaId)
                    {
                        _logger.LogInformation("La columna 'AreaClinicaId' no existe en CitasMedicas. Ejecutando ALTER TABLE...");
                        if (_context.Database.IsSqlite())
                        {
                            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `CitasMedicas` ADD COLUMN `AreaClinicaId` TEXT NULL;");
                        }
                        else
                        {
                            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `CitasMedicas` ADD COLUMN `AreaClinicaId` CHAR(36) NULL;");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear la columna 'AreaClinicaId' en CitasMedicas.");
                }

                // Self-healing: Ensure AreaClinicaId, SubAreaClinica and CamaRetenidaId columns exist in CuentasServicios table
                try
                {
                    bool hasAreaClinicaId = false;
                    bool hasSubAreaClinica = false;
                    bool hasCamaRetenidaId = false;
                    var conn = _context.Database.GetDbConnection();
                    bool closeConnection = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConnection = true;
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                        if (_context.Database.IsSqlite())
                        {
                            cmd.CommandText = "PRAGMA table_info(CuentasServicios);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("AreaClinicaId", StringComparison.OrdinalIgnoreCase))
                                        hasAreaClinicaId = true;
                                    if (name.Equals("SubAreaClinica", StringComparison.OrdinalIgnoreCase))
                                        hasSubAreaClinica = true;
                                    if (name.Equals("CamaRetenidaId", StringComparison.OrdinalIgnoreCase))
                                        hasCamaRetenidaId = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `CuentasServicios` WHERE Field IN ('AreaClinicaId', 'SubAreaClinica', 'CamaRetenidaId');";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var field = reader["Field"]?.ToString() ?? string.Empty;
                                    if (field.Equals("AreaClinicaId", StringComparison.OrdinalIgnoreCase))
                                        hasAreaClinicaId = true;
                                    if (field.Equals("SubAreaClinica", StringComparison.OrdinalIgnoreCase))
                                        hasSubAreaClinica = true;
                                    if (field.Equals("CamaRetenidaId", StringComparison.OrdinalIgnoreCase))
                                        hasCamaRetenidaId = true;
                                }
                            }
                        }
                    }

                    if (!hasAreaClinicaId)
                    {
                        _logger.LogInformation("La columna 'AreaClinicaId' no existe en CuentasServicios. Ejecutando ALTER TABLE...");
                        if (_context.Database.IsSqlite())
                            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `CuentasServicios` ADD COLUMN `AreaClinicaId` TEXT NULL;");
                        else
                            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `CuentasServicios` ADD COLUMN `AreaClinicaId` CHAR(36) NULL;");
                    }

                    if (!hasSubAreaClinica)
                    {
                        _logger.LogInformation("La columna 'SubAreaClinica' no existe en CuentasServicios. Ejecutando ALTER TABLE...");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `CuentasServicios` ADD COLUMN `SubAreaClinica` VARCHAR(100) NULL;");
                    }

                    if (!hasCamaRetenidaId)
                    {
                        _logger.LogInformation("La columna 'CamaRetenidaId' no existe en CuentasServicios. Ejecutando ALTER TABLE...");
                        if (_context.Database.IsSqlite())
                            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `CuentasServicios` ADD COLUMN `CamaRetenidaId` TEXT NULL;");
                        else
                            await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `CuentasServicios` ADD COLUMN `CamaRetenidaId` CHAR(36) NULL;");
                    }

                    if (closeConnection)
                    {
                        await conn.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear las columnas AreaClinicaId/SubAreaClinica/CamaRetenidaId en CuentasServicios.");
                }

                // Self-healing: Ensure PermiteFraccionamiento and UnidadMedida columns exist in ServiciosClinicos table
                try
                {
                    bool hasFraccionamiento = false;
                    bool hasUnidadMedida = false;
                    var conn = _context.Database.GetDbConnection();
                    bool closeConnection = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConnection = true;
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                        if (_context.Database.IsSqlite())
                        {
                            cmd.CommandText = "PRAGMA table_info(ServiciosClinicos);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var colName = reader["name"]?.ToString() ?? string.Empty;
                                    if (colName.Equals("PermiteFraccionamiento", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasFraccionamiento = true;
                                    }
                                    else if (colName.Equals("UnidadMedida", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasUnidadMedida = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `ServiciosClinicos`;";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var field = reader["Field"]?.ToString() ?? string.Empty;
                                    if (field.Equals("PermiteFraccionamiento", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasFraccionamiento = true;
                                    }
                                    else if (field.Equals("UnidadMedida", StringComparison.OrdinalIgnoreCase))
                                    {
                                        hasUnidadMedida = true;
                                    }
                                }
                            }
                        }
                    }
                    if (closeConnection)
                    {
                        await conn.CloseAsync();
                    }

                    if (!hasFraccionamiento)
                    {
                        _logger.LogInformation("La columna 'PermiteFraccionamiento' no existe en ServiciosClinicos. Ejecutando ALTER TABLE...");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `ServiciosClinicos` ADD COLUMN `PermiteFraccionamiento` TINYINT(1) NOT NULL DEFAULT 0;");
                    }

                    if (!hasUnidadMedida)
                    {
                        _logger.LogInformation("La columna 'UnidadMedida' no existe en ServiciosClinicos. Ejecutando ALTER TABLE...");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE `ServiciosClinicos` ADD COLUMN `UnidadMedida` VARCHAR(50) NULL;");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear las columnas 'PermiteFraccionamiento' y 'UnidadMedida' en ServiciosClinicos.");
                }

                // Self-healing: Ensure new imaging and soft-delete columns exist in ServiciosClinicos, DetalleServicioCuenta and OrdenesImagenes
                try
                {
                    var conn = _context.Database.GetDbConnection();
                    bool closeConnection = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConnection = true;
                    }

                    bool isSqlite = _context.Database.IsSqlite();

                    // 1. ServiciosClinicos
                    bool hasServicioInformeId = false;
                    bool hasEsServicioInforme = false;
                    bool hasDesactivadoPorUsuarioId = false;
                    bool hasFechaDesactivacion = false;

                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(ServiciosClinicos);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("ServicioInformeId", StringComparison.OrdinalIgnoreCase)) hasServicioInformeId = true;
                                    if (name.Equals("EsServicioInforme", StringComparison.OrdinalIgnoreCase)) hasEsServicioInforme = true;
                                    if (name.Equals("DesactivadoPorUsuarioId", StringComparison.OrdinalIgnoreCase)) hasDesactivadoPorUsuarioId = true;
                                    if (name.Equals("FechaDesactivacion", StringComparison.OrdinalIgnoreCase)) hasFechaDesactivacion = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `ServiciosClinicos` WHERE Field IN ('ServicioInformeId', 'EsServicioInforme', 'DesactivadoPorUsuarioId', 'FechaDesactivacion');";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var field = reader["Field"]?.ToString() ?? string.Empty;
                                    if (field.Equals("ServicioInformeId", StringComparison.OrdinalIgnoreCase)) hasServicioInformeId = true;
                                    if (field.Equals("EsServicioInforme", StringComparison.OrdinalIgnoreCase)) hasEsServicioInforme = true;
                                    if (field.Equals("DesactivadoPorUsuarioId", StringComparison.OrdinalIgnoreCase)) hasDesactivadoPorUsuarioId = true;
                                    if (field.Equals("FechaDesactivacion", StringComparison.OrdinalIgnoreCase)) hasFechaDesactivacion = true;
                                }
                            }
                        }
                    }

                    if (!hasServicioInformeId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `ServiciosClinicos` ADD COLUMN `ServicioInformeId` TEXT NULL;" : "ALTER TABLE `ServiciosClinicos` ADD COLUMN `ServicioInformeId` CHAR(36) NULL;");
                    if (!hasEsServicioInforme)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `ServiciosClinicos` ADD COLUMN `EsServicioInforme` INTEGER NOT NULL DEFAULT 0;" : "ALTER TABLE `ServiciosClinicos` ADD COLUMN `EsServicioInforme` TINYINT(1) NOT NULL DEFAULT 0;");
                    if (!hasDesactivadoPorUsuarioId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `ServiciosClinicos` ADD COLUMN `DesactivadoPorUsuarioId` TEXT NULL;" : "ALTER TABLE `ServiciosClinicos` ADD COLUMN `DesactivadoPorUsuarioId` VARCHAR(255) NULL;");
                    if (!hasFechaDesactivacion)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `ServiciosClinicos` ADD COLUMN `FechaDesactivacion` TEXT NULL;" : "ALTER TABLE `ServiciosClinicos` ADD COLUMN `FechaDesactivacion` DATETIME NULL;");

                    // 2. DetalleServicioCuenta
                    bool hasDetallePadreId = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(DetallesServicioCuenta);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("DetallePadreId", StringComparison.OrdinalIgnoreCase)) hasDetallePadreId = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `DetallesServicioCuenta` LIKE 'DetallePadreId';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync()) hasDetallePadreId = true;
                            }
                        }
                    }
                    if (!hasDetallePadreId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `DetallesServicioCuenta` ADD COLUMN `DetallePadreId` TEXT NULL;" : "ALTER TABLE `DetallesServicioCuenta` ADD COLUMN `DetallePadreId` CHAR(36) NULL;");

                    bool hasTipoServicioId = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(DetallesServicioCuenta);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("TipoServicioId", StringComparison.OrdinalIgnoreCase)) hasTipoServicioId = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `DetallesServicioCuenta` LIKE 'TipoServicioId';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync()) hasTipoServicioId = true;
                            }
                        }
                    }
                    if (!hasTipoServicioId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `DetallesServicioCuenta` ADD COLUMN `TipoServicioId` INTEGER NOT NULL DEFAULT 5;" : "ALTER TABLE `DetallesServicioCuenta` ADD COLUMN `TipoServicioId` INT NOT NULL DEFAULT 5;");

                    bool hasUsuarioCargaId = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(DetallesServicioCuenta);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("UsuarioCargaId", StringComparison.OrdinalIgnoreCase)) hasUsuarioCargaId = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `DetallesServicioCuenta` LIKE 'UsuarioCargaId';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync()) hasUsuarioCargaId = true;
                            }
                        }
                    }
                    if (!hasUsuarioCargaId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `DetallesServicioCuenta` ADD COLUMN `UsuarioCargaId` TEXT NULL;" : "ALTER TABLE `DetallesServicioCuenta` ADD COLUMN `UsuarioCargaId` VARCHAR(255) NULL;");

                    bool hasCirugiaUsuarioRegistroId = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(CirugiasObservacionesHistorial);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("UsuarioRegistroId", StringComparison.OrdinalIgnoreCase)) hasCirugiaUsuarioRegistroId = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `CirugiasObservacionesHistorial` LIKE 'UsuarioRegistroId';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync()) hasCirugiaUsuarioRegistroId = true;
                            }
                        }
                    }
                    if (!hasCirugiaUsuarioRegistroId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CirugiasObservacionesHistorial` ADD COLUMN `UsuarioRegistroId` TEXT NULL;" : "ALTER TABLE `CirugiasObservacionesHistorial` ADD COLUMN `UsuarioRegistroId` VARCHAR(255) NULL;");

                    bool hasPagoMetodoPagoId = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(DetallesPago);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("MetodoPagoId", StringComparison.OrdinalIgnoreCase)) hasPagoMetodoPagoId = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `DetallesPago` LIKE 'MetodoPagoId';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync()) hasPagoMetodoPagoId = true;
                            }
                        }
                    }
                    if (!hasPagoMetodoPagoId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `DetallesPago` ADD COLUMN `MetodoPagoId` TEXT NULL;" : "ALTER TABLE `DetallesPago` ADD COLUMN `MetodoPagoId` CHAR(36) NULL;");

                    // 3. OrdenesImagenes
                    bool hasLinkInforme = false;
                    bool hasObservacionesMedico = false;
                    bool hasMedicoInterpreteId = false;
                    bool hasRequiereInforme = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(OrdenesImagenes);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("LinkInforme", StringComparison.OrdinalIgnoreCase)) hasLinkInforme = true;
                                    if (name.Equals("ObservacionesMedico", StringComparison.OrdinalIgnoreCase)) hasObservacionesMedico = true;
                                    if (name.Equals("MedicoInterpreteId", StringComparison.OrdinalIgnoreCase)) hasMedicoInterpreteId = true;
                                    if (name.Equals("RequiereInforme", StringComparison.OrdinalIgnoreCase)) hasRequiereInforme = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `OrdenesImagenes` WHERE Field IN ('LinkInforme', 'ObservacionesMedico', 'MedicoInterpreteId', 'RequiereInforme');";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var field = reader["Field"]?.ToString() ?? string.Empty;
                                    if (field.Equals("LinkInforme", StringComparison.OrdinalIgnoreCase)) hasLinkInforme = true;
                                    if (field.Equals("ObservacionesMedico", StringComparison.OrdinalIgnoreCase)) hasObservacionesMedico = true;
                                    if (field.Equals("MedicoInterpreteId", StringComparison.OrdinalIgnoreCase)) hasMedicoInterpreteId = true;
                                    if (field.Equals("RequiereInforme", StringComparison.OrdinalIgnoreCase)) hasRequiereInforme = true;
                                }
                            }
                        }
                    }
                    if (!hasLinkInforme)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `OrdenesImagenes` ADD COLUMN `LinkInforme` TEXT NULL;" : "ALTER TABLE `OrdenesImagenes` ADD COLUMN `LinkInforme` VARCHAR(1000) NULL;");
                    if (!hasObservacionesMedico)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `OrdenesImagenes` ADD COLUMN `ObservacionesMedico` TEXT NULL;" : "ALTER TABLE `OrdenesImagenes` ADD COLUMN `ObservacionesMedico` VARCHAR(2000) NULL;");
                    if (!hasMedicoInterpreteId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `OrdenesImagenes` ADD COLUMN `MedicoInterpreteId` TEXT NULL;" : "ALTER TABLE `OrdenesImagenes` ADD COLUMN `MedicoInterpreteId` CHAR(36) NULL;");
                    if (!hasRequiereInforme)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `OrdenesImagenes` ADD COLUMN `RequiereInforme` INTEGER NOT NULL DEFAULT 0;" : "ALTER TABLE `OrdenesImagenes` ADD COLUMN `RequiereInforme` TINYINT(1) NOT NULL DEFAULT 0;");

                    if (closeConnection)
                    {
                        await conn.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear las columnas nuevas de imagenología y soft delete.");
                }

                // Self-healing: Ensure PrincipiosActivos and InsumosPrincipiosActivos tables exist and Insumos has IsDeleted/FechaInactivacion columns
                try
                {
                    var conn = _context.Database.GetDbConnection();
                    bool closeConnection = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConnection = true;
                    }

                    bool isSqlite = _context.Database.IsSqlite();

                    // 0. Crear tabla CategoriasInsumo si no existe
                    if (isSqlite)
                    {
                        await _context.Database.ExecuteSqlRawAsync(@"
                            CREATE TABLE IF NOT EXISTS `CategoriasInsumo` (
                                `Id` TEXT NOT NULL PRIMARY KEY,
                                `Nombre` TEXT NOT NULL,
                                `Codigo` TEXT NULL,
                                `Activo` INTEGER NOT NULL DEFAULT 1,
                                `FechaCreacion` TEXT NOT NULL
                            );
                            CREATE UNIQUE INDEX IF NOT EXISTS `IX_CategoriasInsumo_Nombre` ON `CategoriasInsumo` (`Nombre`);
                        ");
                    }
                    else
                    {
                        await _context.Database.ExecuteSqlRawAsync(@"
                            CREATE TABLE IF NOT EXISTS `CategoriasInsumo` (
                                `Id` CHAR(36) NOT NULL,
                                `Nombre` VARCHAR(150) NOT NULL,
                                `Codigo` VARCHAR(50) NULL,
                                `Activo` TINYINT(1) NOT NULL DEFAULT 1,
                                `FechaCreacion` DATETIME NOT NULL,
                                PRIMARY KEY (`Id`),
                                UNIQUE KEY `IX_CategoriasInsumo_Nombre` (`Nombre`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                        ");
                    }

                    // 1. Crear tabla PrincipiosActivos si no existe
                    if (isSqlite)
                    {
                        await _context.Database.ExecuteSqlRawAsync(@"
                            CREATE TABLE IF NOT EXISTS `PrincipiosActivos` (
                                `Id` TEXT NOT NULL PRIMARY KEY,
                                `Nombre` TEXT NOT NULL,
                                `Activo` INTEGER NOT NULL DEFAULT 1
                            );
                            CREATE UNIQUE INDEX IF NOT EXISTS `IX_PrincipiosActivos_Nombre` ON `PrincipiosActivos` (`Nombre`);
                        ");
                    }
                    else
                    {
                        await _context.Database.ExecuteSqlRawAsync(@"
                            CREATE TABLE IF NOT EXISTS `PrincipiosActivos` (
                                `Id` CHAR(36) NOT NULL,
                                `Nombre` VARCHAR(200) NOT NULL,
                                `Activo` TINYINT(1) NOT NULL DEFAULT 1,
                                PRIMARY KEY (`Id`),
                                UNIQUE KEY `IX_PrincipiosActivos_Nombre` (`Nombre`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                        ");
                    }

                    // 2. Crear tabla InsumosPrincipiosActivos si no existe
                    if (isSqlite)
                    {
                        await _context.Database.ExecuteSqlRawAsync(@"
                            CREATE TABLE IF NOT EXISTS `InsumosPrincipiosActivos` (
                                `InsumoId` TEXT NOT NULL,
                                `PrincipioActivoId` TEXT NOT NULL,
                                `Concentracion` TEXT NULL,
                                PRIMARY KEY (`InsumoId`, `PrincipioActivoId`)
                            );
                        ");
                    }
                    else
                    {
                        await _context.Database.ExecuteSqlRawAsync(@"
                            CREATE TABLE IF NOT EXISTS `InsumosPrincipiosActivos` (
                                `InsumoId` CHAR(36) NOT NULL,
                                `PrincipioActivoId` CHAR(36) NOT NULL,
                                `Concentracion` VARCHAR(100) NULL,
                                PRIMARY KEY (`InsumoId`, `PrincipioActivoId`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                        ");
                    }

                    // 3. Verificar/crear columnas de transición y soft delete en Insumos
                    bool hasIsDeleted = false;
                    bool hasFechaInactivacion = false;
                    bool hasCategoriaInsumoId = false;

                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(Insumos);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase)) hasIsDeleted = true;
                                    if (name.Equals("FechaInactivacion", StringComparison.OrdinalIgnoreCase)) hasFechaInactivacion = true;
                                    if (name.Equals("CategoriaInsumoId", StringComparison.OrdinalIgnoreCase)) hasCategoriaInsumoId = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `Insumos` WHERE Field IN ('IsDeleted', 'FechaInactivacion', 'CategoriaInsumoId');";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var field = reader["Field"]?.ToString() ?? string.Empty;
                                    if (field.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase)) hasIsDeleted = true;
                                    if (field.Equals("FechaInactivacion", StringComparison.OrdinalIgnoreCase)) hasFechaInactivacion = true;
                                    if (field.Equals("CategoriaInsumoId", StringComparison.OrdinalIgnoreCase)) hasCategoriaInsumoId = true;
                                }
                            }
                        }
                    }

                    if (!hasIsDeleted)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `Insumos` ADD COLUMN `IsDeleted` INTEGER NOT NULL DEFAULT 0;" : "ALTER TABLE `Insumos` ADD COLUMN `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0;");
                    if (!hasFechaInactivacion)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `Insumos` ADD COLUMN `FechaInactivacion` TEXT NULL;" : "ALTER TABLE `Insumos` ADD COLUMN `FechaInactivacion` DATETIME NULL;");
                    if (!hasCategoriaInsumoId)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `Insumos` ADD COLUMN `CategoriaInsumoId` TEXT NULL;" : "ALTER TABLE `Insumos` ADD COLUMN `CategoriaInsumoId` CHAR(36) NULL;");

                    // 4. Verificar/crear columna ObservacionDespacho en PedidosInterSedeDetalles
                    bool hasObservacionDespacho = false;
                    using (var cmd = conn.CreateCommand())
                    {
                        if (isSqlite)
                        {
                            cmd.CommandText = "PRAGMA table_info(PedidosInterSedeDetalles);";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var name = reader["name"]?.ToString() ?? string.Empty;
                                    if (name.Equals("ObservacionDespacho", StringComparison.OrdinalIgnoreCase)) hasObservacionDespacho = true;
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `PedidosInterSedeDetalles` WHERE Field = 'ObservacionDespacho';";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync()) hasObservacionDespacho = true;
                            }
                        }
                    }

                    if (!hasObservacionDespacho)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `PedidosInterSedeDetalles` ADD COLUMN `ObservacionDespacho` TEXT NULL;" : "ALTER TABLE `PedidosInterSedeDetalles` ADD COLUMN `ObservacionDespacho` VARCHAR(500) NULL;");

                    if (closeConnection)
                    {
                        await conn.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo verificar/crear las tablas de PrincipiosActivos y columnas de soft delete en Insumos.");
                }

                // Auto-sanación y verificación de tablas y columnas del módulo quirúrgico
                await EnsureSurgicalTablesAndColumnsAsync();

                // Auto-sanación integral de compatibilidad para respaldos de producción
                await EnsureProductionCompatibilitySchemaAsync();

                await SeedEspecialidadesAsync();
                await SeedServiciosClinicosAsync();
                await SeedMedicosAsync();
                await SeedServiciosSugerenciasAsync();
                await SeedHonorariosMedicosServiciosAsync();
                await SeedConfiguracionAsync();
                await SeedTasaCambioAsync();
                
                // Senior Maintenance Pattern: Asegurar integridad de fechas de recaudación
                await FixOrphanPaymentDatesAsync();
                await SeedMonedasAsync();
                await SeedMetodosPagoAsync();
                await SeedTiposServicioAsync();
                await SeedCategoriasInsumoAsync();
                await SeedRequisitosCirugiaAsync();
                await SeedInventorySedesAndMigrateStockAsync();
                await SeedAreasClinicasAsync();
 
                _logger.LogInformation("System Database Inicializada Correctamente (Limpia para Pruebas Manuales).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inicializando System Database.");
                throw;
            }
        }

        private async Task SeedCategoriasInsumoAsync()
        {
            try
            {
                if (!await _context.CategoriasInsumo.AnyAsync())
                {
                    var defaultCategorias = new List<CategoriaInsumo>
                    {
                        new CategoriaInsumo("Medicamento", "MED", SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.CategoriaId_Medicamento),
                        new CategoriaInsumo("Descartable", "DESC", SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.CategoriaId_Descartable),
                        new CategoriaInsumo("Material Médico", "MAT-MED", SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.CategoriaId_MaterialMedico),
                        new CategoriaInsumo("Reactivo", "REACT", SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.CategoriaId_Reactivo),
                        new CategoriaInsumo("Material Quirúrgico", "MAT-QX", SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.CategoriaId_MaterialQuirurgico),
                        new CategoriaInsumo("Otro", "OTRO", SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.CategoriaId_Otro)
                    };

                    await _context.CategoriasInsumo.AddRangeAsync(defaultCategorias);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Categorías de insumo iniciales sembradas exitosamente ({Count} categorías).", defaultCategorias.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudieron sembrar las categorías de insumo iniciales.");
            }
        }

        private async Task SeedRequisitosCirugiaAsync()
        {
            try
            {
                if (!await _context.RequisitosCirugia.AnyAsync())
                {
                    var defaultRequisitos = new List<RequisitoCirugia>
                    {
                        new RequisitoCirugia("Evaluación Cardiovascular / Riesgo Quirúrgico", "Informe de cardiología y electrocardiograma vigente.", true, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.RequisitoId_Cardiovascular),
                        new RequisitoCirugia("Exámenes Preoperatorios (Laboratorio)", "Hematología completa, TP, TPT, Glucemia, Urea, Creatinina y VIH/VDRL.", true, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.RequisitoId_Laboratorio),
                        new RequisitoCirugia("Consentimiento Informado Firmado", "Firma del paciente o familiar responsable para procedimiento quirúrgico y anestesia.", true, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.RequisitoId_Consentimiento),
                        new RequisitoCirugia("Ayuno Verificado (Mínimo 8 Horas)", "Verificación por enfermería de ayuno estricto.", true, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.RequisitoId_Ayuno),
                        new RequisitoCirugia("Valoración Anestésica", "Aprobación formal firmada por el médico anestesiólogo.", true, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.RequisitoId_ValoracionAnestesica),
                        new RequisitoCirugia("Reserva de Sangre / Hemoderivados", "Disponibilidad confirmada con Banco de Sangre (cuando aplique).", true, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.RequisitoId_ReservaSangre),
                        new RequisitoCirugia("Disponibilidad de Cama Postoperatoria (UCI / Hosp)", "Cama confirmada para el traslado post-quirúrgico.", true, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.RequisitoId_CamaPostoperatoria)
                    };

                    await _context.RequisitosCirugia.AddRangeAsync(defaultRequisitos);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Requisitos de cirugía iniciales sembrados exitosamente ({Count} requisitos).", defaultRequisitos.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudieron sembrar los requisitos de cirugía iniciales.");
            }
        }

        /// <summary>
        /// Tablas de datos de prueba a purgar, en orden de dependencia (hijos primero).
        /// Lista interna controlada — no proviene de input externo.
        /// </summary>
        private static readonly IReadOnlyList<string> TablesToPurge = new[]
        {
            "DetallesPago",
            "RecibosFacturas",
            "HistorialModificacionCuentas",
            "DetallesServicioMedicosResponsables",
            "DetallesServicioCuenta",
            "ConsumosServiciosRealizados",
            "InsumosCirugiasPacientes",
            "CirugiaLogs",
            "OrdenesCirugia",
            "CuentasServicios",
            "PedidosInterSedeDetalles",
            "PedidosInterSede",
            "MovimientosInsumo",
            "CierresInventarioDetalles",
            "CierresInventario",
            "TriagesEnfermeria",
            "ValoracionesFisicas",
            "CitasMedicas",
            "OrdenesImagenes",
            "OrdenesDeServicio",
            "OrdenesRX",
            "CajasDiarias",
            "HistorialesLimpiezasCamas",
            "PacientesAdmision",
            "SegurosConvenios",
            "ServiciosInsumoRecetas",
            "InsumosPrincipiosActivos",
            "PrincipiosActivos",
            "CategoriasInsumo",
            "StocksSedes",
            "Insumos"
        };

        private async Task PurgeAllTestDataAsync()
        {
            _logger.LogInformation("[PURGE] Ejecutando limpieza completa de datos de prueba en la base de datos moderna...");

            try
            {
                var conn = _context.Database.GetDbConnection();
                var closeConnection = await EnsureConnectionOpenAsync(conn);
                var isSqlite = _context.Database.IsSqlite();

                using var cmd = conn.CreateCommand();
                await SetForeignKeyChecksAsync(cmd, isSqlite, enabled: false);
                await PurgeTablesAsync(cmd);
                await SetForeignKeyChecksAsync(cmd, isSqlite, enabled: true);

                if (closeConnection)
                {
                    await conn.CloseAsync();
                }

                _logger.LogInformation("[PURGE] Limpieza de datos de prueba completada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PURGE] Error durante la purga de datos de prueba.");
            }
        }

        /// <summary>Abre la conexión si está cerrada. Retorna true si este método la abrió (y por tanto debe cerrarla).</summary>
        private static async Task<bool> EnsureConnectionOpenAsync(System.Data.Common.DbConnection conn)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                return false;
            }

            await conn.OpenAsync();
            return true;
        }

        /// <summary>Habilita o deshabilita las FK checks según el proveedor de base de datos.</summary>
        private static async Task SetForeignKeyChecksAsync(System.Data.Common.DbCommand cmd, bool isSqlite, bool enabled)
        {
            cmd.CommandText = (isSqlite, enabled) switch
            {
                (true, true) => "PRAGMA foreign_keys = ON;",
                (true, false) => "PRAGMA foreign_keys = OFF;",
                (false, true) => "SET FOREIGN_KEY_CHECKS = 1;",
                (false, false) => "SET FOREIGN_KEY_CHECKS = 0;"
            };
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>Ejecuta el DELETE por cada tabla de la lista controlada, tolerando errores individuales.</summary>
        private async Task PurgeTablesAsync(System.Data.Common.DbCommand cmd)
        {
            foreach (var table in TablesToPurge)
            {
                try
                {
                    // Seguro: 'table' proviene de la lista estática TablesToPurge, no de input externo.
                    cmd.CommandText = $"DELETE FROM `{table}`;";
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[PURGE] Aviso al limpiar tabla {Table}", table);
                }
            }
        }

        private async Task SeedConveniosAsync()
        {
            // Pruebas manuales: No sembrar convenios automáticos
            await Task.CompletedTask;
        }

        private async Task SeedServiciosClinicosAsync()
        {
            var defaults = new List<ServicioClinico>
            {
                new ServicioClinico("S001", "Consulta Medica General", 30.00m, "Consulta") { HonorariumCategory = "CONSULTA" },
                new ServicioClinico("S002", "Radiografía Tórax", 45.00m, "RX") { HonorariumCategory = "RX" },
                new ServicioClinico("S003", "Informe Médico Especializado", 15.00m, "Informe") { HonorariumCategory = "INFORME" },
                new ServicioClinico("S004", "Consulta Ginecologica", 60.00m, "Consulta") { HonorariumCategory = "CONSULTA" },
                new ServicioClinico("S005", "Citologia", 25.00m, "Citologia") { HonorariumCategory = "CITOLOGIA" },
                new ServicioClinico("S006", "Eco Ginecologico", 40.00m, "Eco") { HonorariumCategory = "INFORME" },
                new ServicioClinico("S007", "Tomografía Axial Computarizada (TAC) Cráneo", 120.00m, "TOMO") { HonorariumCategory = "TOMO" },
                new ServicioClinico("MED-01", "Ibuprofeno 600mg (Medicamento)", 5.00m, "Medicamento") { Category = ServiceCategory.Insumo, HonorariumCategory = "MEDICAMENTO" },
                new ServicioClinico("HOSP-EMG-01", "Cargo por Traslado / Estancia Emergencia", 300.00m, "Hospitalario") { HonorariumCategory = "HOSPITALARIO" },
                new ServicioClinico("HOSP-HOS-01", "Cargo por Traslado / Estancia Hospitalización", 450.00m, "Hospitalario") { HonorariumCategory = "HOSPITALARIO" },
                new ServicioClinico("HOSP-UCI-01", "Cargo por Traslado / Estancia Unidad de Cuidados Intensivos", 600.00m, "Hospitalario") { HonorariumCategory = "HOSPITALARIO" }
            };

            foreach (var s in defaults)
            {
                var existing = await _context.ServiciosClinicos.FirstOrDefaultAsync(x => x.Codigo == s.Codigo);
                if (existing == null)
                {
                    _context.ServiciosClinicos.Add(s);
                }
                else if (string.IsNullOrEmpty(existing.HonorariumCategory))
                {
                    // Senior Maintenance: Actualizamos servicios existentes sin categoría (Migración Pro)
                    existing.HonorariumCategory = s.HonorariumCategory;
                }
            }

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();

            // Link Consulta Ginecologica with specialty Ginecología
            var specGine = await _context.Especialidades.FirstOrDefaultAsync(e => e.Nombre == "Ginecología");
            var consultaGine = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S004");
            if (specGine != null && consultaGine != null && consultaGine.EspecialidadId != specGine.Id)
            {
                consultaGine.SetEspecialidad(specGine.Id);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedEspecialidadesAsync()
        {
            var names = new[] { "Diagnóstico Diferencial", "Oncología", "Cardiología", "Pediatría", "Traumatología", "Ginecología", "Imagenología" };

            foreach (var name in names)
            {
                if (!await _context.Especialidades.AnyAsync(e => e.Nombre == name))
                {
                    _context.Especialidades.Add(new Especialidad(name));
                }
            }

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
        }

        private async Task SeedMedicosAsync()
        {
            var medicDefaults = new[]
            {
                (Name: "Gregory House", Speciality: "Diagnóstico Diferencial"),
                (Name: "James Wilson", Speciality: "Oncología"),
                (Name: "Stephen Strange", Speciality: "Cardiología"),
                (Name: "Patch Adams", Speciality: "Pediatría"),
                (Name: "John Watson", Speciality: "Traumatología"),
                (Name: "Lisa Cuddy", Speciality: "Ginecología"),
                (Name: "José Bracamonte", Speciality: "Imagenología"),
                (Name: "María Gutiérrez", Speciality: "Imagenología")
            };

            foreach (var m in medicDefaults)
            {
                if (!await _context.Medicos.AnyAsync(x => x.Nombre == m.Name))
                {
                    var spec = await _context.Especialidades.FirstOrDefaultAsync(e => e.Nombre == m.Speciality);
                    if (spec != null)
                    {
                        _context.Medicos.Add(new Medico(m.Name, spec.Id));
                    }
                }
            }

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
        }

        private async Task SeedServiciosSugerenciasAsync()
        {
            var consultaGine = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S004");
            var citologia = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S005");
            var ecoGine = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S006");
            var rxTorax = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S002");
            var informeMedico = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S003");

            if (consultaGine != null && citologia != null)
            {
                var exists = await _context.ServiciosSugerencias.AnyAsync(ss => ss.ServicioOrigenId == consultaGine.Id && ss.ServicioSugeridoId == citologia.Id);
                if (!exists)
                {
                    _context.ServiciosSugerencias.Add(new ServicioSugerencia(consultaGine.Id, citologia.Id));
                }
            }

            if (consultaGine != null && ecoGine != null)
            {
                var exists = await _context.ServiciosSugerencias.AnyAsync(ss => ss.ServicioOrigenId == consultaGine.Id && ss.ServicioSugeridoId == ecoGine.Id);
                if (!exists)
                {
                    _context.ServiciosSugerencias.Add(new ServicioSugerencia(consultaGine.Id, ecoGine.Id));
                }
            }

            if (rxTorax != null && informeMedico != null)
            {
                var exists = await _context.ServiciosSugerencias.AnyAsync(ss => ss.ServicioOrigenId == rxTorax.Id && ss.ServicioSugeridoId == informeMedico.Id);
                if (!exists)
                {
                    _context.ServiciosSugerencias.Add(new ServicioSugerencia(rxTorax.Id, informeMedico.Id));
                }
            }

            if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
        }

        private async Task SeedHonorariosMedicosServiciosAsync()
        {
            var jose = await _context.Medicos.FirstOrDefaultAsync(m => m.Nombre == "José Bracamonte");
            var maria = await _context.Medicos.FirstOrDefaultAsync(m => m.Nombre == "María Gutiérrez");
            var informe = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "S003");

            if (informe != null)
            {
                if (jose != null)
                {
                    var exists = await _context.HonorariosMedicosServicios.AnyAsync(h => h.ServicioId == informe.Id && h.MedicoId == jose.Id);
                    if (!exists)
                    {
                        _context.HonorariosMedicosServicios.Add(new HonorarioMedicoServicio(informe.Id, jose.Id, 10.00m, "System"));
                    }
                }

                if (maria != null)
                {
                    var exists = await _context.HonorariosMedicosServicios.AnyAsync(h => h.ServicioId == informe.Id && h.MedicoId == maria.Id);
                    if (!exists)
                    {
                        _context.HonorariosMedicosServicios.Add(new HonorarioMedicoServicio(informe.Id, maria.Id, 8.00m, "System"));
                    }
                }

                if (_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
            }
        }

        private async Task SeedPacientesAsync()
        {
            // Pruebas manuales: No cargar pacientes de prueba automáticamente
            await Task.CompletedTask;
        }

        private async Task SeedCajaDiariaAsync()
        {
            // Pruebas manuales: No abrir cajas de prueba automáticamente
            await Task.CompletedTask;
        }

        private async Task SeedConfiguracionAsync()
        {
            if (!await _context.ConfiguracionGeneral.AnyAsync())
            {
                _context.ConfiguracionGeneral.Add(
                    new ConfiguracionGeneral("SAT HOSPITALARIO - EXCELENCIA", "J-12345678-9", 16.00m, "1234", false, false)
                );
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedTasaCambioAsync()
        {
            // Solo sembramos si no hay NINGUNA tasa definida.
            if (!await _context.TasaCambio.AnyAsync())
            {
                _logger.LogInformation("Sembrando tasa de cambio inicial (Baseline 36.50)...");
                _context.TasaCambio.Add(new TasaCambio(36.50m));
                await _context.SaveChangesAsync();
            }
        }

        private async Task FixOrphanPaymentDatesAsync()
        {
            try 
            {
                // Corregimos cualquier pago con fecha default (0001-01-01) igualando a la del recibo padre.
                // Esto asegura que el Dashboard sea preciso con datos anteriores al cambio de esquema.
                int affected = await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE DetallesPago d JOIN RecibosFacturas r ON d.ReciboFacturaId = r.Id SET d.FechaPago = r.FechaEmision WHERE d.FechaPago = '0001-01-01 00:00:00'"
                );

                if (affected > 0)
                {
                    _logger.LogInformation($"Auto-Mantenimiento: Se corrigieron {affected} fechas de recaudación históricas.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "El auto-mantenimiento de fechas de pago falló. El Dashboard podría mostrar datos incompletos transitoriamente.");
            }
        }
        private async Task SeedMetodosPagoAsync()
        {
            if (await _context.CatalogoMetodosPago.AnyAsync()) return;

            _logger.LogInformation("Sembrando catálogo de métodos de pago y vueltos...");

            var metodos = new List<CatalogoMetodoPago>
            {
                // Métodos de Pago (GrupoMoneda: 1 = USD, 2 = VES)
                new CatalogoMetodoPago("EFECTIVO DOLAR ($)", "Dolar Efectivo", 1, false, 1),
                new CatalogoMetodoPago("ZELLE", "Zelle", 1, false, 2),
                new CatalogoMetodoPago("USDT (BINANCE)", "USDT", 1, false, 3),
                new CatalogoMetodoPago("PUNTO DE VENTA USD", "Punto Dolares", 1, false, 4),
                new CatalogoMetodoPago("EFECTIVO (BS)", "Efectivo BS", 2, false, 5),
                new CatalogoMetodoPago("PAGO MÓVIL", "Pago Movil", 2, false, 6),
                new CatalogoMetodoPago("TRANSFERENCIA", "Transferencia", 2, false, 7),
                new CatalogoMetodoPago("PUNTO DE VENTA BS", "Punto", 2, false, 8),

                // Métodos de Vuelto
                new CatalogoMetodoPago("VUELTO EFECTIVO ($)", "Vuelto Efectivo USD", 1, true, 1),
                new CatalogoMetodoPago("VUELTO PAGO MÓVIL (BS)", "Vuelto Pago Movil", 2, true, 2),
                new CatalogoMetodoPago("VUELTO EFECTIVO (BS)", "Vuelto Efectivo BS", 2, true, 3)
            };

            _context.CatalogoMetodosPago.AddRange(metodos);
            await _context.SaveChangesAsync();
        }
        private async Task SeedMonedasAsync()
        {
            if (await _context.Monedas.AnyAsync()) return;

            _logger.LogInformation("Sembrando monedas en la base de datos...");

            var monedas = new List<Moneda>
            {
                new Moneda(1, "USD", "Dólar", "$", true),
                new Moneda(2, "VES", "Bolívar", "Bs.", false),
                new Moneda(3, "EUR", "Euro", "€", false),
                new Moneda(4, "COP", "Peso Colombiano", "COP$", false),
                new Moneda(5, "ARS", "Peso Argentino", "ARS$", false)
            };

            _context.Monedas.AddRange(monedas);
            await _context.SaveChangesAsync();
        }

        private async Task SeedHonorarioConfigAsync()
        {
            if (await _context.HonorariosConfig.AnyAsync()) return;

            _logger.LogInformation("Sembrando configuración inicial de honorarios...");

            // Intentamos obtener un médico por defecto para el seed (Gregory House si existe)
            var medicoDefault = await _context.Medicos.FirstOrDefaultAsync(m => m.Nombre.Contains("House"));
            var usuario = "Sistema";

            var categories = new[] { "CONSULTA", "RX", "INFORME", "CITOLOGIA" };

            foreach (var cat in categories)
            {
                var conf = new HonorarioConfig(cat, usuario);
                if (medicoDefault != null) conf.AsignarMedicoDefault(medicoDefault.Id, usuario, "Auto-asignado por Initializer");
                _context.HonorariosConfig.Add(conf);
            }

            // También sembramos las reglas de mapeo para fallback (compatibilidad con Sistema 2020 si aplicara)
            if (!await _context.HonorariumMappingRules.AnyAsync())
            {
                _context.HonorariumMappingRules.Add(new HonorariumMappingRule("RX", "RX", MappingRuleType.Contains, 1, usuario));
                _context.HonorariumMappingRules.Add(new HonorariumMappingRule("RADI", "RX", MappingRuleType.Contains, 2, usuario));
                _context.HonorariumMappingRules.Add(new HonorariumMappingRule("INFO", "INFORME", MappingRuleType.Contains, 3, usuario));
                _context.HonorariumMappingRules.Add(new HonorariumMappingRule("CONS", "CONSULTA", MappingRuleType.Contains, 4, usuario));
            }

            await _context.SaveChangesAsync();
        }
        private async Task SeedInventorySedesAndMigrateStockAsync()
        {
            _logger.LogInformation("[MIGRATION] Verificando existencia y migrando IDs de Sedes a constantes fijas...");

            var sedesDef = new[]
            {
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal,       Codigo: "PRINCIPAL",       Nombre: "Almacen Principal",    EsPrincipal: true),
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Emergencia,      Codigo: "EMERGENCIA",      Nombre: "Área de Emergencia",             EsPrincipal: false),
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Hospitalizacion, Codigo: "HOSPITALIZACION", Nombre: "Área de Hospitalización",        EsPrincipal: false),
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_UCI,             Codigo: "UCI",             Nombre: "Unidad de Cuidados Intensivos",  EsPrincipal: false),
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Cirugia,         Codigo: "CIRUGIA",         Nombre: "Área de Cirugía y Quirófano",    EsPrincipal: false)
            };

            foreach (var def in sedesDef)
            {
                var existingId = await ObtenerSedeIdPorCodigoAsync(def.Codigo);
                if (existingId == null && def.EsPrincipal)
                {
                    // Fallback para principal por flag
                    var conn = _context.Database.GetDbConnection();
                    bool closeConn = false;
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                        closeConn = true;
                    }
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT `Id` FROM `Sedes` WHERE `EsPrincipal` = 1 LIMIT 1;";
                        var val = await cmd.ExecuteScalarAsync();
                        if (val != null && val != DBNull.Value && val.ToString() is string valStr && !string.IsNullOrEmpty(valStr))
                        {
                            existingId = Guid.Parse(valStr);
                        }
                    }
                    if (closeConn) await conn.CloseAsync();
                }

                if (existingId == null)
                {
                    // Crear nueva
                    var newSede = new Sede(def.Codigo, def.Nombre, def.EsPrincipal);
                    SetSedeId(newSede, def.Id);
                    _context.Sedes.Add(newSede);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"[MIGRATION] Sede creada: {def.Codigo} con ID {def.Id}.");
                }
                else if (existingId.Value != def.Id)
                {
                    // Migrar ID existente
                    await MigrateSedeIdAsync(existingId.Value, def.Id);
                }
            }

            // Migrar Stocks existentes en Insumos si no están registrados en StocksSede
            var principalSede = await _context.Sedes.FirstOrDefaultAsync(s => s.EsPrincipal && s.Activo);
            if (principalSede != null)
            {
                var insumos = await _context.Insumos.Include(i => i.StocksPorSede).ToListAsync();
                foreach (var insumo in insumos)
                {
                    if (!insumo.StocksPorSede.Any(s => s.SedeId == principalSede.Id))
                    {
                        decimal legacyStock = 0;
                        try
                        {
                            var conn = _context.Database.GetDbConnection();
                            bool closeConnection = false;
                            if (conn.State != System.Data.ConnectionState.Open)
                            {
                                await conn.OpenAsync();
                                closeConnection = true;
                            }
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = "SELECT StockActual FROM Insumos WHERE Id = @insumoId;";
                                AddParameter(cmd, "@insumoId", insumo.Id);
                                var val = await cmd.ExecuteScalarAsync();
                                if (val != null && val != DBNull.Value)
                                {
                                    legacyStock = Convert.ToDecimal(val);
                                }
                            }
                            if (closeConnection) await conn.CloseAsync();
                        }
                        catch
                        {
                            legacyStock = 0;
                        }

                        var stockSede = new StockSede(insumo.Id, principalSede.Id, legacyStock);
                        _context.StocksSedes.Add(stockSede);
                        _logger.LogInformation("[MIGRATION] Migrado Stock de Insumo {Codigo}: {Stock} a Sede Principal.", insumo.Codigo, legacyStock);
                    }
                }
            }

            // Migrar Movimientos de Insumos huérfanos sin SedeId
            try
            {
                var conn = _context.Database.GetDbConnection();
                bool closeConnection = false;
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                    closeConnection = true;
                }
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE MovimientosInsumo SET SedeId = @principalSedeId WHERE SedeId IS NULL OR SedeId = '00000000-0000-0000-0000-000000000000';";
                    AddParameter(cmd, "@principalSedeId", SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal);
                    int affected = await cmd.ExecuteNonQueryAsync();
                    if (affected > 0)
                    {
                        _logger.LogInformation("[MIGRATION] Se actualizaron {Count} movimientos huérfanos asignándoles la Sede Principal.", affected);
                    }
                }
                if (closeConnection) await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[MIGRATION] Error al intentar actualizar SedeId en MovimientosInsumo.");
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedAreasClinicasAsync()
        {
            _logger.LogInformation("[SEED] Verificando existencia de quirófanos y áreas clínicas base...");

            var defaultAreas = new (Guid SedeId, string Codigo, string Nombre, bool EsAdmision)[]
            {
                // Sede Cirugía (Quirófanos)
                (SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Cirugia, "QX-1", "Quirófano 1", false),
                (SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Cirugia, "QX-2", "Quirófano 2", false),
                (SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Cirugia, "SALA-PARTOS", "Sala de Partos", false),

                // Sede Hospitalización (Habitaciones)
                (SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Hospitalizacion, "HAB-101", "Habitación 101", false),
                (SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Hospitalizacion, "HAB-102", "Habitación 102", false),

                // Sede Emergencia (Boxes)
                (SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Emergencia, "BOX-1", "Box Emergencia 1", true),
                (SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Emergencia, "BOX-2", "Box Emergencia 2", true),

                // Sede UCI
                (SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_UCI, "UCI-1", "Cama UCI 1", false)
            };

            foreach (var def in defaultAreas)
            {
                var exists = await _context.AreasClinicas
                    .AnyAsync(a => a.SedeId == def.SedeId && a.Codigo == def.Codigo);
                if (!exists)
                {
                    var area = new AreaClinica(def.SedeId, def.Codigo, def.Nombre, def.EsAdmision);
                    _context.AreasClinicas.Add(area);
                    _logger.LogInformation("[SEED] Área/Quirófano creado: {Codigo} - {Nombre}", def.Codigo, def.Nombre);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Guid?> ObtenerSedeIdPorCodigoAsync(string codigo)
        {
            var conn = _context.Database.GetDbConnection();
            bool closeConnection = false;
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
                closeConnection = true;
            }

            Guid? id = null;
            using (var cmd = conn.CreateCommand())
            {
                 cmd.CommandText = "SELECT `Id` FROM `Sedes` WHERE `Codigo` = @codigo LIMIT 1;";
                 AddParameter(cmd, "@codigo", codigo);
                var val = await cmd.ExecuteScalarAsync();
                if (val != null && val != DBNull.Value && val.ToString() is string valStr && !string.IsNullOrEmpty(valStr))
                {
                    id = Guid.Parse(valStr);
                }
            }
            if (closeConnection) await conn.CloseAsync();
            return id;
        }

        private static void SetSedeId(Sede Sede, Guid id)
        {
            var prop = typeof(Sede).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            prop?.SetValue(Sede, id);
        }

        private static void SetAreaClinicaId(AreaClinica area, Guid id)
        {
            var prop = typeof(AreaClinica).GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            prop?.SetValue(area, id);
        }

        private async Task MigrateSedeIdAsync(Guid oldId, Guid newId)
        {
            if (oldId == newId) return;

            var conn = _context.Database.GetDbConnection();
            bool closeConnection = false;
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
                closeConnection = true;
            }

            using (var cmd = conn.CreateCommand())
            {
                if (!_context.Database.IsSqlite())
                {
                    cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0;";
                    await cmd.ExecuteNonQueryAsync();
                }

                AddMigrationIdParameters(cmd, oldId, newId);
                cmd.CommandText = "UPDATE `Sedes` SET `Id` = @newId WHERE `Id` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "UPDATE `StocksSede` SET `SedeId` = @newId WHERE `SedeId` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "UPDATE `MovimientosInsumo` SET `SedeId` = @newId WHERE `SedeId` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "UPDATE `CierresInventario` SET `SedeId` = @newId WHERE `SedeId` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "UPDATE `AreasClinicas` SET `SedeId` = @newId WHERE `SedeId` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "UPDATE `PedidosInterSede` SET `SedeSolicitanteId` = @newId WHERE `SedeSolicitanteId` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "UPDATE `PedidosInterSede` SET `SedeProveedoraId` = @newId WHERE `SedeProveedoraId` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                if (!_context.Database.IsSqlite())
                {
                    cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1;";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            if (closeConnection)
            {
                await conn.CloseAsync();
            }

            _logger.LogInformation($"[MIGRATION] Sede ID migrado de {oldId} a {newId} (incluyendo tablas relacionadas).");
        }

        private async Task MigrateAreaClinicaIdAsync(Guid oldId, Guid newId)
        {
            if (oldId == newId) return;

            var conn = _context.Database.GetDbConnection();
            bool closeConnection = false;
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync();
                closeConnection = true;
            }

            using (var cmd = conn.CreateCommand())
            {
                if (!_context.Database.IsSqlite())
                {
                    cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0;";
                    await cmd.ExecuteNonQueryAsync();
                }

                AddMigrationIdParameters(cmd, oldId, newId);
                cmd.CommandText = "UPDATE `AreasClinicas` SET `Id` = @newId WHERE `Id` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "UPDATE `CitasMedicas` SET `AreaClinicaId` = @newId WHERE `AreaClinicaId` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "UPDATE `CuentasServicios` SET `AreaClinicaId` = @newId WHERE `AreaClinicaId` = @oldId;";
                await cmd.ExecuteNonQueryAsync();

                if (!_context.Database.IsSqlite())
                {
                    cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1;";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            if (closeConnection)
            {
                await conn.CloseAsync();
            }

            _logger.LogInformation($"[MIGRATION] Área Clínica ID migrado de {oldId} a {newId} (incluyendo tablas relacionadas).");
        }

            private static void AddMigrationIdParameters(System.Data.Common.DbCommand command, Guid oldId, Guid newId)
            {
                AddParameter(command, "@oldId", oldId);
                AddParameter(command, "@newId", newId);
            }

            private static void AddParameter(System.Data.Common.DbCommand command, string name, object value)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = value;
                command.Parameters.Add(parameter);
            }

        private async Task SeedInsumosYRecetasTestAsync()
        {
            // Pruebas manuales: No sembrar insumos ni recetas automáticas
            await Task.CompletedTask;
        }

        private async Task SeedTiposServicioAsync()
        {
            try
            {
                var isSqlite = _context.Database.IsSqlite();
                if (isSqlite)
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS TiposServicio (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Nombre TEXT NOT NULL,
                            Codigo TEXT NOT NULL
                        );
                    ");
                }
                else
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `TiposServicio` (
                            `Id` INT NOT NULL AUTO_INCREMENT,
                            `Nombre` VARCHAR(100) NOT NULL,
                            `Codigo` VARCHAR(50) NOT NULL,
                            PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                }

                if (!await _context.TiposServicio.AnyAsync())
                {
                    var tipos = new List<TipoServicio>
                    {
                        new TipoServicio(Core.Domain.Constants.TipoServicioConstants.Medico, "Servicio Médico / Consulta", "MEDICO"),
                        new TipoServicio(Core.Domain.Constants.TipoServicioConstants.Laboratorio, "Examen de Laboratorio", "LAB"),
                        new TipoServicio(Core.Domain.Constants.TipoServicioConstants.RX, "Rayos X / Imagenología", "RX"),
                        new TipoServicio(Core.Domain.Constants.TipoServicioConstants.Tomo, "Tomografía Axial", "TOMO"),
                        new TipoServicio(Core.Domain.Constants.TipoServicioConstants.Insumo, "Insumo / Medicamento", "INSUMO"),
                        new TipoServicio(Core.Domain.Constants.TipoServicioConstants.Informe, "Informe / Lectura Médica", "INFORME")
                    };

                    await _context.TiposServicio.AddRangeAsync(tipos);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("[SYSTEM-DB-INITIALIZER] Se sembraron 6 tipos de servicio en la tabla TiposServicio.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[SYSTEM-DB-INITIALIZER] Error al sembrar la tabla TiposServicio.");
            }
        }

        private async Task EnsureSurgicalTablesAndColumnsAsync()
        {
            try
            {
                var isSqlite = _context.Database.IsSqlite();
                var isMySql = !isSqlite;

                if (isSqlite)
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS RequisitosCirugia (
                            Id TEXT PRIMARY KEY,
                            Nombre TEXT NOT NULL,
                            Descripcion TEXT NULL,
                            EsActivo INTEGER NOT NULL DEFAULT 1
                        );
                        CREATE TABLE IF NOT EXISTS OrdenesCirugia (
                            Id TEXT PRIMARY KEY,
                            CuentaServicioId TEXT NOT NULL,
                            PacienteId TEXT NOT NULL,
                            AreaClinicaId TEXT NULL,
                            SedeQuirofanoId TEXT NULL,
                            AreaClinicaOrigenId TEXT NULL,
                            SedeOrigenId TEXT NULL,
                            DescripcionCirugia TEXT NOT NULL,
                            PrecioBaseUsd NUMERIC NOT NULL,
                            PrecioDerechoSalaUsd NUMERIC NOT NULL,
                            MedicoId TEXT NOT NULL,
                            FechaHoraProgramada TEXT NOT NULL,
                            Estado TEXT NOT NULL,
                            MotivoCancelacion TEXT NULL,
                            FechaCreacion TEXT NOT NULL,
                            UsuarioCreacion TEXT NOT NULL,
                            SalaQuirofano TEXT NOT NULL,
                            ModalidadAnestesia TEXT NOT NULL,
                            EsAlquilado INTEGER NOT NULL DEFAULT 0
                        );
                        CREATE TABLE IF NOT EXISTS CirugiaLogs (
                            Id TEXT PRIMARY KEY,
                            OrdenCirugiaId TEXT NOT NULL,
                            UsuarioId TEXT NOT NULL,
                            Evento TEXT NOT NULL,
                            Detalle TEXT NOT NULL,
                            Timestamp TEXT NOT NULL
                        );
                        CREATE TABLE IF NOT EXISTS CirugiasObservacionesHistorial (
                            Id TEXT PRIMARY KEY,
                            OrdenCirugiaId TEXT NOT NULL,
                            Observacion TEXT NOT NULL,
                            Tipo INTEGER NOT NULL,
                            FechaRegistro TEXT NOT NULL,
                            UsuarioRegistro TEXT NOT NULL,
                            UsuarioRegistroId TEXT NULL
                        );
                        CREATE TABLE IF NOT EXISTS OrdenesCirugiaRequisitos (
                            Id TEXT PRIMARY KEY,
                            OrdenCirugiaId TEXT NOT NULL,
                            RequisitoCirugiaId TEXT NOT NULL,
                            Cumplido INTEGER NOT NULL DEFAULT 0,
                            FechaVerificacion TEXT NULL,
                            VerificadoPor TEXT NULL
                        );
                        CREATE TABLE IF NOT EXISTS CirugiasMedicosHonorarios (
                            Id TEXT PRIMARY KEY,
                            OrdenCirugiaId TEXT NOT NULL,
                            MedicoId TEXT NOT NULL,
                            EspecialidadId TEXT NOT NULL,
                            MontoHonorarioUsd NUMERIC NOT NULL,
                            EsCirujanoPrincipal INTEGER NOT NULL DEFAULT 0
                        );
                        CREATE TABLE IF NOT EXISTS SolicitudesInsumosCirugia (
                            Id TEXT PRIMARY KEY,
                            OrdenCirugiaId TEXT NOT NULL,
                            InsumoId TEXT NOT NULL,
                            CantidadSolicitada NUMERIC NOT NULL,
                            AlmacenOrigenId TEXT NOT NULL,
                            EstadoSolicitud TEXT NOT NULL,
                            FechaSolicitud TEXT NOT NULL,
                            UsuarioSolicitud TEXT NOT NULL,
                            FechaDespacho TEXT NULL,
                            UsuarioDespacho TEXT NULL
                        );
                        CREATE TABLE IF NOT EXISTS TransferenciasReposicionStock (
                            Id TEXT PRIMARY KEY,
                            InsumoId TEXT NOT NULL,
                            SedeOrigenId TEXT NOT NULL,
                            SedeDestinoId TEXT NOT NULL,
                            Cantidad NUMERIC NOT NULL,
                            Fecha TEXT NOT NULL,
                            UsuarioId TEXT NOT NULL,
                            Observacion TEXT NULL
                        );
                    ");
                }
                else
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `RequisitosCirugia` (
                            `Id` CHAR(36) NOT NULL,
                            `Nombre` VARCHAR(250) NOT NULL,
                            `Descripcion` TEXT NULL,
                            `EsActivo` TINYINT(1) NOT NULL DEFAULT 1,
                            PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `OrdenesCirugia` (
                            `Id` CHAR(36) NOT NULL,
                            `CuentaServicioId` CHAR(36) NOT NULL,
                            `PacienteId` CHAR(36) NOT NULL,
                            `AreaClinicaId` CHAR(36) NULL,
                            `SedeQuirofanoId` CHAR(36) NULL,
                            `AreaClinicaOrigenId` CHAR(36) NULL,
                            `SedeOrigenId` CHAR(36) NULL,
                            `DescripcionCirugia` VARCHAR(500) NOT NULL,
                            `PrecioBaseUsd` DECIMAL(18,2) NOT NULL,
                            `PrecioDerechoSalaUsd` DECIMAL(18,2) NOT NULL,
                            `MedicoId` CHAR(36) NOT NULL,
                            `FechaHoraProgramada` DATETIME NOT NULL,
                            `Estado` VARCHAR(50) NOT NULL,
                            `MotivoCancelacion` VARCHAR(500) NULL,
                            `FechaCreacion` DATETIME NOT NULL,
                            `UsuarioCreacion` VARCHAR(100) NOT NULL,
                            `SalaQuirofano` VARCHAR(100) NOT NULL,
                            `ModalidadAnestesia` VARCHAR(100) NOT NULL,
                            `EsAlquilado` TINYINT(1) NOT NULL DEFAULT 0,
                            PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `CirugiaLogs` (
                            `Id` CHAR(36) NOT NULL,
                            `OrdenCirugiaId` CHAR(36) NOT NULL,
                            `UsuarioId` VARCHAR(100) NOT NULL,
                            `Evento` VARCHAR(100) NOT NULL,
                            `Detalle` TEXT NOT NULL,
                            `Timestamp` DATETIME NOT NULL,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_CirugiaLogs_Orden` (`OrdenCirugiaId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `CirugiasObservacionesHistorial` (
                            `Id` CHAR(36) NOT NULL,
                            `OrdenCirugiaId` CHAR(36) NOT NULL,
                            `Observacion` TEXT NOT NULL,
                            `Tipo` INT NOT NULL,
                            `FechaRegistro` DATETIME NOT NULL,
                            `UsuarioRegistro` VARCHAR(100) NOT NULL,
                            `UsuarioRegistroId` VARCHAR(100) NULL,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_CirugiaObs_Orden` (`OrdenCirugiaId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `OrdenesCirugiaRequisitos` (
                            `Id` CHAR(36) NOT NULL,
                            `OrdenCirugiaId` CHAR(36) NOT NULL,
                            `RequisitoCirugiaId` CHAR(36) NOT NULL,
                            `Cumplido` TINYINT(1) NOT NULL DEFAULT 0,
                            `FechaVerificacion` DATETIME NULL,
                            `VerificadoPor` VARCHAR(100) NULL,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_OrdenReq_Orden` (`OrdenCirugiaId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `CirugiasMedicosHonorarios` (
                            `Id` CHAR(36) NOT NULL,
                            `OrdenCirugiaId` CHAR(36) NOT NULL,
                            `MedicoId` CHAR(36) NOT NULL,
                            `EspecialidadId` CHAR(36) NOT NULL,
                            `MontoHonorarioUsd` DECIMAL(18,2) NOT NULL,
                            `EsCirujanoPrincipal` TINYINT(1) NOT NULL DEFAULT 0,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_CirugiaMed_Orden` (`OrdenCirugiaId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `SolicitudesInsumosCirugia` (
                            `Id` CHAR(36) NOT NULL,
                            `OrdenCirugiaId` CHAR(36) NOT NULL,
                            `InsumoId` CHAR(36) NOT NULL,
                            `CantidadSolicitada` DECIMAL(18,4) NOT NULL,
                            `AlmacenOrigenId` CHAR(36) NOT NULL,
                            `EstadoSolicitud` VARCHAR(50) NOT NULL,
                            `FechaSolicitud` DATETIME NOT NULL,
                            `UsuarioSolicitud` VARCHAR(100) NOT NULL,
                            `FechaDespacho` DATETIME NULL,
                            `UsuarioDespacho` VARCHAR(100) NULL,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_SolIns_Orden` (`OrdenCirugiaId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `TransferenciasReposicionStock` (
                            `Id` CHAR(36) NOT NULL,
                            `InsumoId` CHAR(36) NOT NULL,
                            `SedeOrigenId` CHAR(36) NOT NULL,
                            `SedeDestinoId` CHAR(36) NOT NULL,
                            `Cantidad` DECIMAL(18,4) NOT NULL,
                            `Fecha` DATETIME NOT NULL,
                            `UsuarioId` VARCHAR(100) NOT NULL,
                            `Observacion` TEXT NULL,
                            PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                }

                // Verificación y adición de columnas dinámicas en OrdenesCirugia si la tabla ya existía
                var conn = _context.Database.GetDbConnection();
                bool closeConn = false;
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                    closeConn = true;
                }

                bool hasAreaClinicaOrigenId = false;
                bool hasSedeOrigenId = false;

                using (var cmd = conn.CreateCommand())
                {
                    if (isSqlite)
                    {
                        cmd.CommandText = "PRAGMA table_info(OrdenesCirugia);";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var col = reader["name"]?.ToString() ?? string.Empty;
                                if (col.Equals("AreaClinicaOrigenId", StringComparison.OrdinalIgnoreCase)) hasAreaClinicaOrigenId = true;
                                if (col.Equals("SedeOrigenId", StringComparison.OrdinalIgnoreCase)) hasSedeOrigenId = true;
                            }
                        }
                    }
                    else
                    {
                        cmd.CommandText = "SHOW COLUMNS FROM `OrdenesCirugia` WHERE Field IN ('AreaClinicaOrigenId', 'SedeOrigenId');";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var col = reader["Field"]?.ToString() ?? string.Empty;
                                if (col.Equals("AreaClinicaOrigenId", StringComparison.OrdinalIgnoreCase)) hasAreaClinicaOrigenId = true;
                                if (col.Equals("SedeOrigenId", StringComparison.OrdinalIgnoreCase)) hasSedeOrigenId = true;
                            }
                        }
                    }
                }

                if (!hasAreaClinicaOrigenId)
                {
                    await _context.Database.ExecuteSqlRawAsync(isSqlite
                        ? "ALTER TABLE `OrdenesCirugia` ADD COLUMN `AreaClinicaOrigenId` TEXT NULL;"
                        : "ALTER TABLE `OrdenesCirugia` ADD COLUMN `AreaClinicaOrigenId` CHAR(36) NULL;");
                }

                if (!hasSedeOrigenId)
                {
                    await _context.Database.ExecuteSqlRawAsync(isSqlite
                        ? "ALTER TABLE `OrdenesCirugia` ADD COLUMN `SedeOrigenId` TEXT NULL;"
                        : "ALTER TABLE `OrdenesCirugia` ADD COLUMN `SedeOrigenId` CHAR(36) NULL;");
                }

                if (closeConn)
                {
                    await conn.CloseAsync();
                }

                _logger.LogInformation("[SYSTEM-DB-INITIALIZER] Tablas y columnas quirúrgicas auto-sanadas exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[SYSTEM-DB-INITIALIZER] No se pudieron verificar/crear las tablas quirúrgicas.");
            }
        }

        private async Task EnsureProductionCompatibilitySchemaAsync()
        {
            try
            {
                var isSqlite = _context.Database.IsSqlite();

                // 1. Tablas auxiliares de Auditoría, Garantías y Compras
                if (isSqlite)
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS GarantiasItems (
                            Id TEXT PRIMARY KEY,
                            CuentaPorCobrarId TEXT NOT NULL,
                            Descripcion TEXT NOT NULL,
                            ValorEstimado NUMERIC NOT NULL,
                            FechaRegistro TEXT NOT NULL
                        );
                        CREATE TABLE IF NOT EXISTS DocumentLogs (
                            Id TEXT PRIMARY KEY,
                            DocumentType TEXT NOT NULL,
                            ReferenceId TEXT NOT NULL,
                            Action TEXT NOT NULL,
                            UserId TEXT NOT NULL,
                            UserName TEXT NOT NULL,
                            Timestamp TEXT NOT NULL,
                            Details TEXT NULL
                        );
                        CREATE TABLE IF NOT EXISTS Proveedores (
                            Id TEXT PRIMARY KEY,
                            RIF TEXT NOT NULL,
                            RazonSocial TEXT NOT NULL,
                            Direccion TEXT NULL,
                            Telefono TEXT NULL,
                            Activo INTEGER NOT NULL DEFAULT 1,
                            FechaRegistro TEXT NOT NULL
                        );
                        CREATE TABLE IF NOT EXISTS OrdenesCompraInventario (
                            Id TEXT PRIMARY KEY,
                            NumeroFactura TEXT NOT NULL,
                            ProveedorId TEXT NULL,
                            ProveedorNombre TEXT NOT NULL,
                            FechaEmision TEXT NOT NULL,
                            MontoTotalUSD NUMERIC NOT NULL,
                            MontoTotalBs NUMERIC NOT NULL,
                            TotalAbonadoUSD NUMERIC NOT NULL,
                            SaldoPendienteUSD NUMERIC NOT NULL,
                            Estado TEXT NOT NULL,
                            Observaciones TEXT NULL
                        );
                        CREATE TABLE IF NOT EXISTS PagosProveedores (
                            Id TEXT PRIMARY KEY,
                            OrdenCompraId TEXT NOT NULL,
                            FechaPago TEXT NOT NULL,
                            MontoAbonadoUSD NUMERIC NOT NULL,
                            TasaCambio NUMERIC NOT NULL,
                            MontoAbonadoBs NUMERIC NOT NULL,
                            MetodoPago TEXT NOT NULL,
                            Referencia TEXT NULL,
                            UsuarioId TEXT NULL,
                            Observaciones TEXT NULL
                        );
                    ");
                }
                else
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE IF NOT EXISTS `GarantiasItems` (
                            `Id` CHAR(36) NOT NULL,
                            `CuentaPorCobrarId` CHAR(36) NOT NULL,
                            `Descripcion` VARCHAR(500) NOT NULL,
                            `ValorEstimado` DECIMAL(18,2) NOT NULL,
                            `FechaRegistro` DATETIME NOT NULL,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_GarantiasItems_CxC` (`CuentaPorCobrarId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `DocumentLogs` (
                            `Id` CHAR(36) NOT NULL,
                            `DocumentType` VARCHAR(100) NOT NULL,
                            `ReferenceId` VARCHAR(100) NOT NULL,
                            `Action` VARCHAR(50) NOT NULL,
                            `UserId` VARCHAR(100) NOT NULL,
                            `UserName` VARCHAR(100) NOT NULL,
                            `Timestamp` DATETIME NOT NULL,
                            `Details` TEXT NULL,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_DocumentLogs_Ref` (`ReferenceId`),
                            INDEX `IX_DocumentLogs_Time` (`Timestamp`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `Proveedores` (
                            `Id` CHAR(36) NOT NULL,
                            `RIF` VARCHAR(50) NOT NULL,
                            `RazonSocial` VARCHAR(250) NOT NULL,
                            `Direccion` VARCHAR(500) NULL,
                            `Telefono` VARCHAR(50) NULL,
                            `Activo` TINYINT(1) NOT NULL DEFAULT 1,
                            `FechaRegistro` DATETIME NOT NULL,
                            PRIMARY KEY (`Id`),
                            UNIQUE KEY `IX_Proveedores_RIF` (`RIF`),
                            INDEX `IX_Proveedores_RazonSocial` (`RazonSocial`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `OrdenesCompraInventario` (
                            `Id` CHAR(36) NOT NULL,
                            `NumeroFactura` VARCHAR(100) NOT NULL,
                            `ProveedorId` CHAR(36) NULL,
                            `ProveedorNombre` VARCHAR(250) NOT NULL,
                            `FechaEmision` DATETIME NOT NULL,
                            `MontoTotalUSD` DECIMAL(18,2) NOT NULL,
                            `MontoTotalBs` DECIMAL(18,2) NOT NULL,
                            `TotalAbonadoUSD` DECIMAL(18,2) NOT NULL,
                            `SaldoPendienteUSD` DECIMAL(18,2) NOT NULL,
                            `Estado` VARCHAR(50) NOT NULL,
                            `Observaciones` VARCHAR(1000) NULL,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_OrdenesCompra_Numero` (`NumeroFactura`),
                            INDEX `IX_OrdenesCompra_Proveedor` (`ProveedorNombre`),
                            INDEX `IX_OrdenesCompra_Estado` (`Estado`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

                        CREATE TABLE IF NOT EXISTS `PagosProveedores` (
                            `Id` CHAR(36) NOT NULL,
                            `OrdenCompraId` CHAR(36) NOT NULL,
                            `FechaPago` DATETIME NOT NULL,
                            `MontoAbonadoUSD` DECIMAL(18,2) NOT NULL,
                            `TasaCambio` DECIMAL(18,2) NOT NULL,
                            `MontoAbonadoBs` DECIMAL(18,2) NOT NULL,
                            `MetodoPago` VARCHAR(50) NOT NULL,
                            `Referencia` VARCHAR(100) NULL,
                            `UsuarioId` VARCHAR(100) NULL,
                            `Observaciones` VARCHAR(1000) NULL,
                            PRIMARY KEY (`Id`),
                            INDEX `IX_PagosProveedores_Orden` (`OrdenCompraId`),
                            INDEX `IX_PagosProveedores_Fecha` (`FechaPago`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                }

                // 2. Columnas dinámicas en tablas existentes
                var conn = _context.Database.GetDbConnection();
                bool closeConn = false;
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                    closeConn = true;
                }

                // Insumos -> OcultoEnTraslados
                bool hasOcultoEnTraslados = false;
                using (var cmd = conn.CreateCommand())
                {
                    if (isSqlite)
                    {
                        cmd.CommandText = "PRAGMA table_info(Insumos);";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (string.Equals(reader["name"]?.ToString(), "OcultoEnTraslados", StringComparison.OrdinalIgnoreCase))
                                    hasOcultoEnTraslados = true;
                            }
                        }
                    }
                    else
                    {
                        cmd.CommandText = "SHOW COLUMNS FROM `Insumos` LIKE 'OcultoEnTraslados';";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) hasOcultoEnTraslados = true;
                        }
                    }
                }
                if (!hasOcultoEnTraslados)
                {
                    await _context.Database.ExecuteSqlRawAsync(isSqlite
                        ? "ALTER TABLE `Insumos` ADD COLUMN `OcultoEnTraslados` INTEGER NOT NULL DEFAULT 0;"
                        : "ALTER TABLE `Insumos` ADD COLUMN `OcultoEnTraslados` TINYINT(1) NOT NULL DEFAULT 0;");
                }

                // InsumosCirugiasPacientes -> OrdenCirugiaId
                bool hasOrdenCirugiaIdInInsumosCirugia = false;
                using (var cmd = conn.CreateCommand())
                {
                    if (isSqlite)
                    {
                        cmd.CommandText = "PRAGMA table_info(InsumosCirugiasPacientes);";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (string.Equals(reader["name"]?.ToString(), "OrdenCirugiaId", StringComparison.OrdinalIgnoreCase))
                                    hasOrdenCirugiaIdInInsumosCirugia = true;
                            }
                        }
                    }
                    else
                    {
                        cmd.CommandText = "SHOW COLUMNS FROM `InsumosCirugiasPacientes` LIKE 'OrdenCirugiaId';";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) hasOrdenCirugiaIdInInsumosCirugia = true;
                        }
                    }
                }
                if (!hasOrdenCirugiaIdInInsumosCirugia)
                {
                    await _context.Database.ExecuteSqlRawAsync(isSqlite
                        ? "ALTER TABLE `InsumosCirugiasPacientes` ADD COLUMN `OrdenCirugiaId` TEXT NULL;"
                        : "ALTER TABLE `InsumosCirugiasPacientes` ADD COLUMN `OrdenCirugiaId` CHAR(36) NULL;");
                }

                // CuentasPorCobrar -> CompromisoGenerado, GarantiaGenerada, IsAudited, etc.
                var cxcCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = conn.CreateCommand())
                {
                    if (isSqlite)
                    {
                        cmd.CommandText = "PRAGMA table_info(CuentasPorCobrar);";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                cxcCols.Add(reader["name"]?.ToString() ?? string.Empty);
                        }
                    }
                    else
                    {
                        cmd.CommandText = "SHOW COLUMNS FROM `CuentasPorCobrar`;";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                cxcCols.Add(reader["Field"]?.ToString() ?? string.Empty);
                        }
                    }
                }

                if (!cxcCols.Contains("CompromisoGenerado"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `CompromisoGenerado` INTEGER NOT NULL DEFAULT 0;" : "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `CompromisoGenerado` TINYINT(1) NOT NULL DEFAULT 0;");
                if (!cxcCols.Contains("GarantiaGenerada"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `GarantiaGenerada` INTEGER NOT NULL DEFAULT 0;" : "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `GarantiaGenerada` TINYINT(1) NOT NULL DEFAULT 0;");
                if (!cxcCols.Contains("IsAudited"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `IsAudited` INTEGER NOT NULL DEFAULT 0;" : "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `IsAudited` TINYINT(1) NOT NULL DEFAULT 0;");
                if (!cxcCols.Contains("UsuarioAuditoria"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `UsuarioAuditoria` TEXT NULL;" : "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `UsuarioAuditoria` VARCHAR(100) NULL;");
                if (!cxcCols.Contains("FechaAuditoria"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `FechaAuditoria` TEXT NULL;" : "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `FechaAuditoria` DATETIME NULL;");
                if (!cxcCols.Contains("QuienAutorizo"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `QuienAutorizo` TEXT NULL;" : "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `QuienAutorizo` VARCHAR(150) NULL;");
                if (!cxcCols.Contains("DoctorProcedimiento"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `DoctorProcedimiento` TEXT NULL;" : "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `DoctorProcedimiento` VARCHAR(150) NULL;");
                if (!cxcCols.Contains("InformacionAdicional"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `InformacionAdicional` TEXT NULL;" : "ALTER TABLE `CuentasPorCobrar` ADD COLUMN `InformacionAdicional` TEXT NULL;");

                // Medicos -> Activo, Telefono, IntervaloTurnoMinutos
                var medCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = conn.CreateCommand())
                {
                    if (isSqlite)
                    {
                        cmd.CommandText = "PRAGMA table_info(Medicos);";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                medCols.Add(reader["name"]?.ToString() ?? string.Empty);
                        }
                    }
                    else
                    {
                        cmd.CommandText = "SHOW COLUMNS FROM `Medicos`;";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                medCols.Add(reader["Field"]?.ToString() ?? string.Empty);
                        }
                    }
                }

                if (!medCols.Contains("Activo"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `Medicos` ADD COLUMN `Activo` INTEGER NOT NULL DEFAULT 1;" : "ALTER TABLE `Medicos` ADD COLUMN `Activo` TINYINT(1) NOT NULL DEFAULT 1;");
                if (!medCols.Contains("Telefono"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `Medicos` ADD COLUMN `Telefono` TEXT NULL;" : "ALTER TABLE `Medicos` ADD COLUMN `Telefono` VARCHAR(50) NULL;");
                if (!medCols.Contains("IntervaloTurnoMinutos"))
                    await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `Medicos` ADD COLUMN `IntervaloTurnoMinutos` INTEGER NOT NULL DEFAULT 20;" : "ALTER TABLE `Medicos` ADD COLUMN `IntervaloTurnoMinutos` INT NOT NULL DEFAULT 20;");

                // Especialidades -> Activo
                bool hasEspActivo = false;
                using (var cmd = conn.CreateCommand())
                {
                    if (isSqlite)
                    {
                        cmd.CommandText = "PRAGMA table_info(Especialidades);";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (string.Equals(reader["name"]?.ToString(), "Activo", StringComparison.OrdinalIgnoreCase))
                                    hasEspActivo = true;
                            }
                        }
                    }
                    else
                    {
                        cmd.CommandText = "SHOW COLUMNS FROM `Especialidades` LIKE 'Activo';";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) hasEspActivo = true;
                        }
                    }
                }
                if (!hasEspActivo)
                {
                    await _context.Database.ExecuteSqlRawAsync(isSqlite
                        ? "ALTER TABLE `Especialidades` ADD COLUMN `Activo` INTEGER NOT NULL DEFAULT 1;"
                        : "ALTER TABLE `Especialidades` ADD COLUMN `Activo` TINYINT(1) NOT NULL DEFAULT 1;");
                }

                if (closeConn)
                {
                    await conn.CloseAsync();
                }

                _logger.LogInformation("[SYSTEM-DB-INITIALIZER] Esquema de compatibilidad de producción auto-sanado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[SYSTEM-DB-INITIALIZER] Error durante la auto-sanación de compatibilidad de producción.");
            }
        }
    }
}
