using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using Dapper;

namespace LegacyManualTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=sistema2020;Uid=root;Pwd=Labordono1818;Connection Timeout=20;Allow User Variables=True;AllowPublicKeyRetrieval=True;SslMode=None";
            
            Console.WriteLine("[DIRECT-SYNC] Iniciando proceso manual para Paciente 90, Perfil 20.");

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                // 1. Crear Orden
                var now = DateTime.Now;
                var sqlOrden = @"INSERT INTO ordenes (IdPersona, Fecha, HoraIngreso, PrecioF, IDConvenio, EstadoDeOrden, Usuario, NumeroDia) 
                                 VALUES (@IdPersona, @Fecha, @HoraIngreso, @PrecioF, @IDConvenio, @EstadoDeOrden, @Usuario, @NumeroDia);
                                 SELECT LAST_INSERT_ID();";
                
                var orderId = await connection.ExecuteScalarAsync<int>(sqlOrden, new {
                    IdPersona = 90,
                    Fecha = now,
                    HoraIngreso = now.ToString("HH:mm:ss"),
                    PrecioF = 21332.25,
                    IDConvenio = 1,
                    EstadoDeOrden = 1,
                    Usuario = 1,
                    NumeroDia = 1 // Simplified for test
                }, transaction);

                Console.WriteLine($"[DIRECT-SYNC] Orden creada exitosamente. ID: {orderId}");

                // 2. Insertar Perfil Facturado
                var sqlPerfil = @"INSERT INTO perfilesfacturados (IdOrden, IdPerfil, PrecioTotal) 
                                  VALUES (@IdOrden, @IdPerfil, @PrecioTotal)";
                await connection.ExecuteAsync(sqlPerfil, new {
                    IdOrden = orderId,
                    IdPerfil = 20,
                    PrecioTotal = 21332.25
                }, transaction);

                // 3. Buscar Análisis asociados al Perfil 20
                var sqlAnalisis = "SELECT IdAnalisis, IDOrganizador FROM perfilesanalisis WHERE IdPerfil = 20";
                var items = (await connection.QueryAsync(sqlAnalisis, null, transaction)).ToList();
                Console.WriteLine($"[DIRECT-SYNC] Encontrados {items.Count} análisis para el Perfil 20.");

                // 4. Insertar Resultados
                var sqlResult = @"INSERT INTO resultadospaciente (IdPaciente, IdOrden, IdAnalisis, IDOrganizador, IdConvenio, EstadoDeResultado, FechaIngreso, HoraIngreso) 
                                  VALUES (@IdPaciente, @IdOrden, @IdAnalisis, @IDOrganizador, @IdConvenio, @EstadoDeResultado, @FechaIngreso, @HoraIngreso)";
                
                foreach (var item in items)
                {
                    await connection.ExecuteAsync(sqlResult, new {
                        IdPaciente = 90,
                        IdOrden = orderId,
                        IdAnalisis = item.IdAnalisis,
                        IDOrganizador = item.IDOrganizador,
                        IdConvenio = 1,
                        EstadoDeResultado = 1,
                        FechaIngreso = now,
                        HoraIngreso = now.ToString("HH:mm:ss")
                    }, transaction);
                    Console.WriteLine($"[DIRECT-SYNC] -> Resultado insertado para Análisis ID: {item.IdAnalisis}");
                }

                await transaction.CommitAsync();
                Console.WriteLine($"[DIRECT-SYNC] ÉXITO TOTAL. Proceso completado para Orden {orderId}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[DIRECT-SYNC] ERROR: {ex.Message}");
            }
        }
    }
}
