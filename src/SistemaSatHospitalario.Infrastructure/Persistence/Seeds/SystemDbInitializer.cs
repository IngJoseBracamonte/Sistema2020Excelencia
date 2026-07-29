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

                    // 3. Verificar/crear columnas IsDeleted y FechaInactivacion en Insumos
                    bool hasIsDeleted = false;
                    bool hasFechaInactivacion = false;

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
                                }
                            }
                        }
                        else
                        {
                            cmd.CommandText = "SHOW COLUMNS FROM `Insumos` WHERE Field IN ('IsDeleted', 'FechaInactivacion');";
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var field = reader["Field"]?.ToString() ?? string.Empty;
                                    if (field.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase)) hasIsDeleted = true;
                                    if (field.Equals("FechaInactivacion", StringComparison.OrdinalIgnoreCase)) hasFechaInactivacion = true;
                                }
                            }
                        }
                    }

                    if (!hasIsDeleted)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `Insumos` ADD COLUMN `IsDeleted` INTEGER NOT NULL DEFAULT 0;" : "ALTER TABLE `Insumos` ADD COLUMN `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0;");
                    if (!hasFechaInactivacion)
                        await _context.Database.ExecuteSqlRawAsync(isSqlite ? "ALTER TABLE `Insumos` ADD COLUMN `FechaInactivacion` TEXT NULL;" : "ALTER TABLE `Insumos` ADD COLUMN `FechaInactivacion` DATETIME NULL;");

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

                _logger.LogInformation("Poblando System Database con datos de prueba...");

                await SeedEspecialidadesAsync();
                await SeedServiciosClinicosAsync();
                await SeedMedicosAsync();
                await SeedServiciosSugerenciasAsync();
                await SeedHonorariosMedicosServiciosAsync();
                await SeedPacientesAsync();
                await SeedCajaDiariaAsync();
                await SeedConfiguracionAsync();
                await SeedTasaCambioAsync();
                await SeedConveniosAsync();
                
                // Senior Maintenance Pattern: Asegurar integridad de fechas de recaudación
                await FixOrphanPaymentDatesAsync();
                await SeedMonedasAsync();
                await SeedMetodosPagoAsync();
                await SeedInventorySedesAndMigrateStockAsync();
                await SeedAreasClinicasAsync();
                await SeedInsumosYRecetasTestAsync();
 
                _logger.LogInformation("System Database Inicializada Correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inicializando System Database.");
                throw;
            }
        }

        private async Task SeedConveniosAsync()
        {
            if (await _context.SegurosConvenios.AnyAsync()) return;

            _logger.LogInformation("Sembrando convenios de seguros por defecto...");

            var convenios = new List<SeguroConvenio>
            {
                new SeguroConvenio("PDVSA", "RTN-5", "Av. Principal PDVSA", "0212-1234567", "pdvsa@test.com"),
                new SeguroConvenio("Seguros Caracas", "RTN-CARACAS", "Centro Seguros Caracas", "0212-7654321", "caracas@test.com"),
                new SeguroConvenio("Mercantil Seguros", "RTN-MERCANTIL", "Torre Mercantil Seguros", "0212-9999999", "mercantil@test.com"),
                new SeguroConvenio("Sanitas", "RTN-SANITAS", "Las Mercedes", "0212-8888888", "sanitas@test.com")
            };

            _context.SegurosConvenios.AddRange(convenios);
            await _context.SaveChangesAsync();
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
            if (!await _context.PacientesAdmision.AnyAsync())
            {
                _logger.LogInformation("Sincronizando pacientes desde Sistema Legacy (Concatenación V11.6)...");
                
                try 
                {
                    // Obtener una muestra de pacientes recientes del legacy para poblar el baseline
                    var legacyPatients = await _legacyRepository.SearchPatientsLimitedAsync("A", default); // Buscar genérica para traer iniciales
                    
                    if (legacyPatients != null && legacyPatients.Any())
                    {
                        foreach (var lp in legacyPatients.Take(100)) // Limitamos a 100 para el seed inicial
                        {
                            var fullName = $"{lp.Nombre} {lp.Apellidos}".Trim();
                            var mainPhone = !string.IsNullOrEmpty(lp.Celular) ? lp.Celular : lp.Telefono;
                            
                            var nativePatient = new PacienteAdmision(lp.Cedula, fullName, mainPhone ?? "", lp.IdPersona);
                            _context.PacientesAdmision.Add(nativePatient);
                        }
                        
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Sincronización inicial completada con éxito.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo completar la sincronización legacy durante el seed. Se usarán datos estáticos de respaldo.");
                    
                    _context.PacientesAdmision.AddRange(
                        new PacienteAdmision("0999999999", "John Doe (Backup)", "0987654321", 1),
                        new PacienteAdmision("0888888888", "Jane Smith (Backup)", "0912345678", 2)
                    );
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task SeedCajaDiariaAsync()
        {
            // Evitar duplicar cajas si el sistema se reinicia (Check por NombreUsuario 'admin')
            if (!await _context.CajasDiarias.AnyAsync(c => c.NombreUsuario == "admin"))
            {
                _context.CajasDiarias.Add(
                    new CajaDiaria(50.00m, 1500.00m, "1", "admin")
                );
                await _context.SaveChangesAsync();
            }
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
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_UCI,             Codigo: "UCI",             Nombre: "Unidad de Cuidados Intensivos",  EsPrincipal: false)
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
                                cmd.CommandText = $"SELECT StockActual FROM Insumos WHERE Id = '{insumo.Id}';";
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
                    cmd.CommandText = $"UPDATE MovimientosInsumo SET SedeId = '{SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal}' WHERE SedeId IS NULL OR SedeId = '00000000-0000-0000-0000-000000000000';";
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
            _logger.LogInformation("[MIGRATION] Verificando existencia de áreas clínicas y migrando sus IDs a constantes fijas...");

            var areasDef = new[]
            {
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.AreaId_Emergencia,      SedeId: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Emergencia,      Codigo: "EMERGENCIA",      Nombre: "Área de Emergencia"),
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.AreaId_Hospitalizacion, SedeId: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Hospitalizacion, Codigo: "HOSPITALIZACION", Nombre: "Área de Hospitalización"),
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.AreaId_UCI,             SedeId: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_UCI,             Codigo: "UCI",             Nombre: "Unidad de Cuidados Intensivos"),
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.AreaId_Farmacia,        SedeId: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal,       Codigo: "FARMACIA",        Nombre: "Farmacia"),
                (Id: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.AreaId_Laboratorio,     SedeId: SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal,       Codigo: "LABORATORIO",     Nombre: "Laboratorio")
            };

            // Senior Maintenance: Limpiar áreas clínicas no fijas o sobrantes de pruebas anteriores
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
                    var idsFijos = string.Join(",", areasDef.Select(a => $"'{a.Id}'"));
                    
                    cmd.CommandText = $"SELECT `Id` FROM `AreasClinicas` WHERE `Id` NOT IN ({idsFijos});";
                    var idsToDelete = new List<string>();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var valStr = reader[0]?.ToString();
                            if (!string.IsNullOrEmpty(valStr) && Guid.TryParse(valStr, out var parsedGuid))
                            {
                                idsToDelete.Add($"'{parsedGuid}'");
                            }
                        }
                    }

                    if (idsToDelete.Any())
                    {
                        var idsStr = string.Join(",", idsToDelete);

                        cmd.CommandText = $"UPDATE `CuentasServicios` SET `AreaClinicaId` = NULL WHERE `AreaClinicaId` IN ({idsStr});";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = $"UPDATE `CitasMedicas` SET `AreaClinicaId` = NULL WHERE `AreaClinicaId` IN ({idsStr});";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = $"UPDATE `DetallesServicioCuenta` SET `AreaClinicaId` = NULL WHERE `AreaClinicaId` IN ({idsStr});";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = $"DELETE FROM `HistorialesLimpiezasCamas` WHERE `CamaId` IN ({idsStr});";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = $"DELETE FROM `AreasClinicas` WHERE `Id` IN ({idsStr});";
                        int deleted = await cmd.ExecuteNonQueryAsync();
                        _logger.LogInformation($"[MIGRATION] Se eliminaron {deleted} áreas clínicas de prueba/sobrantes.");
                    }
                }
                if (closeConnection) await conn.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[MIGRATION] Error al intentar limpiar áreas clínicas de prueba.");
            }

            foreach (var def in areasDef)
            {
                var conn = _context.Database.GetDbConnection();
                bool closeConnection = false;
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    await conn.OpenAsync();
                    closeConnection = true;
                }

                Guid? existingId = null;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT `Id` FROM `AreasClinicas` WHERE `Codigo` = '{def.Codigo}' LIMIT 1;";
                    var val = await cmd.ExecuteScalarAsync();
                    if (val != null && val != DBNull.Value && val.ToString() is string valStr && !string.IsNullOrEmpty(valStr))
                    {
                        existingId = Guid.Parse(valStr);
                    }
                }
                if (closeConnection) await conn.CloseAsync();

                if (existingId == null)
                {
                    var newArea = new AreaClinica(def.SedeId, def.Codigo, def.Nombre);
                    SetAreaClinicaId(newArea, def.Id);
                    _context.AreasClinicas.Add(newArea);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"[MIGRATION] Área Clínica creada: {def.Codigo} con ID {def.Id}.");
                }
                else if (existingId.Value != def.Id)
                {
                    await MigrateAreaClinicaIdAsync(existingId.Value, def.Id);
                }
            }
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
                cmd.CommandText = $"SELECT `Id` FROM `Sedes` WHERE `Codigo` = '{codigo}' LIMIT 1;";
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

                cmd.CommandText = $"UPDATE `Sedes` SET `Id` = '{newId}' WHERE `Id` = '{oldId}';";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = $"UPDATE `StocksSede` SET `SedeId` = '{newId}' WHERE `SedeId` = '{oldId}';";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = $"UPDATE `MovimientosInsumo` SET `SedeId` = '{newId}' WHERE `SedeId` = '{oldId}';";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = $"UPDATE `CierresInventario` SET `SedeId` = '{newId}' WHERE `SedeId` = '{oldId}';";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = $"UPDATE `AreasClinicas` SET `SedeId` = '{newId}' WHERE `SedeId` = '{oldId}';";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = $"UPDATE `PedidosInterSede` SET `SedeSolicitanteId` = '{newId}' WHERE `SedeSolicitanteId` = '{oldId}';";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = $"UPDATE `PedidosInterSede` SET `SedeProveedoraId` = '{newId}' WHERE `SedeProveedoraId` = '{oldId}';";
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

                cmd.CommandText = $"UPDATE `AreasClinicas` SET `Id` = '{newId}' WHERE `Id` = '{oldId}';";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = $"UPDATE `CitasMedicas` SET `AreaClinicaId` = '{newId}' WHERE `AreaClinicaId` = '{oldId}';";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = $"UPDATE `CuentasServicios` SET `AreaClinicaId` = '{newId}' WHERE `AreaClinicaId` = '{oldId}';";
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

        private async Task SeedInsumosYRecetasTestAsync()
        {
            _logger.LogInformation("Sembrando Catálogo Extendido de Medicamentos e Insumos con Múltiples Unidades de Medida...");

            var med01 = await _context.ServiciosClinicos.FirstOrDefaultAsync(s => s.Codigo == "MED-01");

            // Definición de catálogo realista con diferentes unidades de medida
            var catalogoReal = new List<(string Codigo, string Nombre, UnidadMedida Unidad, decimal Costo, bool Fraccionable, string Categoria, string PA, string Concentracion)>
            {
                ("INS-01", "Ibuprofeno 600mg (Medicamento)", UnidadMedida.UNIDAD, 1.50m, false, "Medicamento", "Ibuprofeno", "600mg"),
                ("INS-02", "Dexametasona 4mg/2ml (Ampolla 2ml)", UnidadMedida.ML, 2.80m, true, "Medicamento", "Dexametasona", "4mg/2ml"),
                ("INS-03", "Omeprazol 40mg (Vial Liofilizado)", UnidadMedida.UNIDAD, 4.50m, false, "Medicamento", "Omeprazol", "40mg"),
                ("INS-04", "Salbutamol Jarabe 2mg/5ml (Frasco 120ml)", UnidadMedida.ML, 3.20m, true, "Medicamento", "Salbutamol", "2mg/5ml"),
                ("INS-05", "Sulfadiazina de Plata 1% (Tubo 30g)", UnidadMedida.G, 5.10m, true, "Medicamento", "Sulfadiazina de Plata", "1%"),
                ("INS-06", "Solución Fisiológica 0.9% (Bolsa 500ml)", UnidadMedida.ML, 1.80m, false, "Material Medico", "Cloruro de Sodio", "0.9%"),
                ("INS-07", "Losartán Potásico 50mg (Comprimidos)", UnidadMedida.UNIDAD, 0.75m, false, "Medicamento", "Losartán Potásico", "50mg"),
                ("INS-08", "Guantes Quirúrgicos Estériles Talla 7.5 (Caja)", UnidadMedida.UNIDAD, 12.00m, false, "Material Medico", "Látex", "7.5")
            };

            foreach (var item in catalogoReal)
            {
                var insumo = await _context.Insumos.Include(i => i.PrincipiosActivos).FirstOrDefaultAsync(i => i.Codigo == item.Codigo);
                if (insumo == null)
                {
                    insumo = new Insumo(item.Codigo, item.Nombre, 100.00m, item.Unidad, item.Costo, item.Fraccionable, item.Categoria);
                    _context.Insumos.Add(insumo);
                    await _context.SaveChangesAsync();
                }

                // Crear y vincular Principio Activo si aplica
                if (!string.IsNullOrEmpty(item.PA))
                {
                    var pa = await _context.PrincipiosActivos.FirstOrDefaultAsync(p => p.Nombre == item.PA);
                    if (pa == null)
                    {
                        pa = new PrincipioActivo(item.PA);
                        _context.PrincipiosActivos.Add(pa);
                        await _context.SaveChangesAsync();
                    }

                    if (!insumo.PrincipiosActivos.Any(r => r.PrincipioActivoId == pa.Id))
                    {
                        insumo.AgregarPrincipioActivo(pa, item.Concentracion);
                        await _context.SaveChangesAsync();
                    }
                }

                // Asegurar Stock Sede Principal (Almacén Central)
                var stockPrincipal = await _context.StocksSedes.FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal);
                if (stockPrincipal == null)
                {
                    _context.StocksSedes.Add(new StockSede(insumo.Id, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal, 250.00m));
                }
                else if (stockPrincipal.StockActual < 20.00m)
                {
                    stockPrincipal.EstablecerStockCierre(250.00m);
                }

                // Asegurar Stock UCI
                var stockUci = await _context.StocksSedes.FirstOrDefaultAsync(s => s.InsumoId == insumo.Id && s.SedeId == SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_UCI);
                if (stockUci == null)
                {
                    _context.StocksSedes.Add(new StockSede(insumo.Id, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_UCI, 20.00m));
                }

                // Receta para MED-01 si aplica
                if (med01 != null && item.Codigo == "INS-01")
                {
                    var receta = await _context.ServiciosInsumoRecetas.FirstOrDefaultAsync(r => r.ServicioClinicoId == med01.Id && r.InsumoId == insumo.Id);
                    if (receta == null)
                    {
                        _context.ServiciosInsumoRecetas.Add(new ServicioInsumoReceta(med01.Id, med01.Codigo, insumo.Id, 1.00m, item.Unidad));
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
