using System;

namespace SistemaSatHospitalario.Infrastructure.Common.Helpers
{
    /// <summary>
    /// Senior Utility for handling MySQL infrastructure quirks.
    /// Addresses case-sensitivity issues in database names across different OS/Clouds.
    /// </summary>
    public static class ConnectionStringHelper
    {
        /// <summary>
        /// Normalizes the database name in a MySQL connection string to lowercase.
        /// This ensures compatibility with case-sensitive environments (Docker/Linux).
        /// </summary>
        public static string NormalizeMySqlConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return connectionString;
            
            try 
            {
                var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
                var originalDb = builder.Database;
                
                // [MICRO-CICLO 31] Auto-Normalization strategy:
                // We force lowercase as confirmed by the database tool view (sistema2020, sathospitalario).
                if (!string.IsNullOrEmpty(originalDb))
                {
                    var normalizedDb = originalDb.ToLowerInvariant();
                    
                    if (originalDb != normalizedDb)
                    {
                        builder.Database = normalizedDb;
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
