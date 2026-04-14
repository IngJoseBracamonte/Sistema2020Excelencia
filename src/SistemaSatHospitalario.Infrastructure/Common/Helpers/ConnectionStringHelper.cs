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
                        return builder.ConnectionString;
                    }
                }
            }
            catch 
            {
                // Silent fallback to avoid breaking startup if the string is malformed
            }
            
            return connectionString;
        }
    }
}
