using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;

internal static class AppHostConfiguration
{
    public static void ConfigureApiEnvironment(
        IResourceBuilder<IResourceWithEnvironment> resource,
        IResourceBuilder<IResourceWithEndpoints> frontendResource,
        IConfiguration configuration,
        string dbProviderName,
        bool useDocker)
    {
        SetConnectionString(resource, configuration, "mysql-legacy-query", "ConnectionStrings__LegacyConnection", useDocker);

        resource.WithEnvironment("DatabaseProvider", dbProviderName);

        SetConnectionString(resource, configuration, "mysql-system-query", "ConnectionStrings__mysql-system", useDocker);
        SetConnectionString(resource, configuration, "mysql-identity-query", "ConnectionStrings__mysql-identity", useDocker);
        SetEnvironmentValue(resource, configuration, "jwt-secret", "JwtConfig__Secret");
        SetEnvironmentValue(resource, configuration, "smtp-user", "EmailSettings__SmtpUser");
        SetEnvironmentValue(resource, configuration, "smtp-pass", "EmailSettings__SmtpPass");

        resource.WithEnvironment("JwtConfig__Issuer", configuration["JwtConfig:Issuer"] ?? "SistemaSatHospitalarioAPI")
            .WithEnvironment("JwtConfig__Audience", configuration["JwtConfig:Audience"] ?? "SistemaSatHospitalario_PWA")
            .WithEnvironment("AllowedOrigins", $"{frontendResource.GetEndpoint("http")},https://sathospital.netlify.app,http://localhost:4200,http://127.0.0.1:4200,http://0.0.0.0:4200,http://localhost:80,http://localhost");
    }

    private static void SetConnectionString(
        IResourceBuilder<IResourceWithEnvironment> resource,
        IConfiguration configuration,
        string configurationKey,
        string environmentKey,
        bool useDocker)
    {
        var value = NormalizeMySqlConnectionString(GetConfiguredValue(configuration, configurationKey), useDocker);
        if (!string.IsNullOrEmpty(value))
        {
            resource.WithEnvironment(environmentKey, value);
        }
    }

    private static void SetEnvironmentValue(
        IResourceBuilder<IResourceWithEnvironment> resource,
        IConfiguration configuration,
        string configurationKey,
        string environmentKey)
    {
        var value = GetConfiguredValue(configuration, configurationKey);
        if (!string.IsNullOrEmpty(value))
        {
            resource.WithEnvironment(environmentKey, value);
        }
    }

    private static string GetConfiguredValue(IConfiguration configuration, string key)
    {
        return configuration[key] ?? configuration[$"Parameters:{key}"] ?? string.Empty;
    }

    private static string NormalizeMySqlConnectionString(string connectionString, bool useDocker)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return string.Empty;
        }

        var processed = connectionString;
        if (connectionString.Contains("localhost") || connectionString.Contains("127.0.0.1"))
        {
            var host = useDocker ? "host.docker.internal" : "127.0.0.1";
            processed = processed.Replace("localhost", host).Replace("127.0.0.1", host);
        }

        if (!processed.Contains("AllowPublicKeyRetrieval", StringComparison.OrdinalIgnoreCase))
        {
            processed += (processed.Contains("?") || processed.Contains(";") ? ";" : "") + "AllowPublicKeyRetrieval=True";
        }

        if (!processed.Contains("SslMode", StringComparison.OrdinalIgnoreCase))
        {
            processed += ";SslMode=None";
        }

        if (!processed.Contains("Allow User Variables", StringComparison.OrdinalIgnoreCase))
        {
            processed += ";Allow User Variables=True";
        }

        return processed;
    }
}