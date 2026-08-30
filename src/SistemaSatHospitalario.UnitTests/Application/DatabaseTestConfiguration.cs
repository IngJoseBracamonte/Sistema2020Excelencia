namespace SistemaSatHospitalario.UnitTests.Application
{
    internal static class DatabaseTestConfiguration
    {
        internal static string GetRequiredDatabasePassword() =>
            Environment.GetEnvironmentVariable("MYSQL_PASSWORD")
            ?? throw new InvalidOperationException("MYSQL_PASSWORD debe configurarse para ejecutar pruebas de base de datos directa.");
    }
}
