using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace SistemaSatHospitalario.Infrastructure.Common.Helpers
{
    /// <summary>
    /// Senior Utility for handling MySQL infrastructure quirks.
    /// Addresses case-sensitivity issues in database names across different OS/Clouds.
    /// Also reads legacy server/port configuration from App.config dynamically.
    /// </summary>
    public static class ConnectionStringHelper
    {
        /// <summary>
        /// Traverses directories upwards from BaseDirectory to find App.config and extracts Server/PuertoServer.
        /// </summary>
        public static (string? Server, string? Port) GetServerSettingsFromAppConfig()
        {
            try
            {
                string? configPath = null;
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var currentDir = new DirectoryInfo(baseDir);

                while (currentDir != null)
                {
                    var file = currentDir.GetFiles("App.config", SearchOption.TopDirectoryOnly)
                        .Concat(currentDir.GetFiles("app.config", SearchOption.TopDirectoryOnly))
                        .Concat(currentDir.GetFiles("Laboratorio.exe.config", SearchOption.TopDirectoryOnly))
                        .FirstOrDefault();

                    if (file != null)
                    {
                        configPath = file.FullName;
                        break;
                    }

                    // Check if Laboratorio folder exists in current directory and has App.config
                    var labDir = currentDir.GetDirectories("Laboratorio", SearchOption.TopDirectoryOnly).FirstOrDefault();
                    if (labDir != null)
                    {
                        var labFile = labDir.GetFiles("App.config", SearchOption.TopDirectoryOnly).FirstOrDefault();
                        if (labFile != null)
                        {
                            configPath = labFile.FullName;
                            break;
                        }
                    }

                    currentDir = currentDir.Parent;
                }

                if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
                {
                    return (null, null);
                }

                var doc = new XmlDocument();
                doc.Load(configPath);
                var addNodes = doc.SelectNodes("//connectionStrings/add");
                string? server = null;
                string? port = null;

                if (addNodes != null)
                {
                    foreach (XmlNode node in addNodes)
                    {
                        var nameAttr = node.Attributes?["name"]?.Value;
                        var connStrAttr = node.Attributes?["connectionString"]?.Value;
                        
                        if (string.Equals(nameAttr, "Server", StringComparison.OrdinalIgnoreCase))
                        {
                            server = connStrAttr;
                        }
                        else if (string.Equals(nameAttr, "PuertoServer", StringComparison.OrdinalIgnoreCase))
                        {
                            port = connStrAttr;
                        }
                    }
                }

                return (server, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONFIG-ERROR] GetServerSettingsFromAppConfig failed: {ex.Message}");
                return (null, null);
            }
        }

        /// <summary>
        /// Resolves and overrides server/port in connection string with settings from App.config.
        /// </summary>
        public static string ResolveConnectionStringWithAppConfig(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return connectionString;

            try
            {
                var (server, port) = GetServerSettingsFromAppConfig();
                if (string.IsNullOrEmpty(server))
                {
                    return connectionString;
                }

                var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
                bool updated = false;

                if (builder.Server != server)
                {
                    builder.Server = server;
                    updated = true;
                }

                if (!string.IsNullOrEmpty(port) && uint.TryParse(port, out var portVal) && builder.Port != portVal)
                {
                    builder.Port = portVal;
                    updated = true;
                }

                if (updated)
                {
                    return builder.ConnectionString;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONFIG-ERROR] ResolveConnectionStringWithAppConfig failed: {ex.Message}");
            }

            return connectionString;
        }

        /// <summary>
        /// Normalizes the database name in a MySQL connection string.
        /// By default, it forces lowercase to ensure compatibility with case-sensitive environments (Docker/Linux).
        /// Set forceLowercase to false for legacy systems that require specific casing (e.g. Sistema2020).
        /// </summary>
        public static string NormalizeMySqlConnectionString(string connectionString, bool forceLowercase = false)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return connectionString;
            
            try 
            {
                var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
                var originalDb = builder.Database;
                
                if (!string.IsNullOrEmpty(originalDb))
                {
                    // [MICRO-CICLO 31] Normalized strategy:
                    // Force lowercase for internal systems by default, but allow case-preservation for legacy externos.
                    var targetDb = forceLowercase ? originalDb.ToLowerInvariant() : originalDb;
                    
                    if (originalDb != targetDb)
                    {
                        builder.Database = targetDb;
                        connectionString = builder.ConnectionString;
                    }
                }
            }
            catch 
            {
                // Silent fallback to avoid breaking startup if the string is malformed
            }
            
            return connectionString;
        }

        /// <summary>
        /// Detects if the connection is pointing to a cloud managed database (like Aiven)
        /// and enforces secure defaults (SSL, Public Key Retrieval).
        /// </summary>
        public static string EnhanceForCloud(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return connectionString;

            try
            {
                var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
                var host = builder.Server?.ToLowerInvariant() ?? "";

                // Aiven Detection Strategy
                if (host.Contains("aivencloud.com"))
                {
                    // Enforced secure defaults for Managed Cloud Databases
                    if (builder.SslMode == MySqlConnector.MySqlSslMode.None)
                    {
                        builder.SslMode = MySqlConnector.MySqlSslMode.Required;
                    }

                    builder.AllowPublicKeyRetrieval = true;
                    builder.AllowUserVariables = true; // Essential for migrations and complex queries
                    
                    return builder.ConnectionString;
                }
            }
            catch
            {
                // Fallback to original if parsing fails
            }

            return connectionString;
        }
    }
}
