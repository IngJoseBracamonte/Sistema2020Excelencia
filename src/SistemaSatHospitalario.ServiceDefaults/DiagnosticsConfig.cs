using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Microsoft.Extensions.Hosting;

public static class DiagnosticsConfig
{
    public const string ServiceName = "SistemaSatHospitalario";
    public static readonly ActivitySource ActivitySource = new(ServiceName);
    public static readonly Meter Meter = new(ServiceName);
    
    // Ejemplo de un contador de métricas personalizado
    public static readonly Counter<long> LoginCounter = Meter.CreateCounter<long>(
        "auth.login_attempts", 
        unit: "{attempts}", 
        description: "Número de intentos de inicio de sesión");

    public static readonly Counter<long> HonorariumMappingHits = Meter.CreateCounter<long>(
        "honorarium.mapping.hits",
        unit: "{hits}",
        description: "Número de mapeos de honorarios exitosos");

    public static readonly Counter<long> HonorariumMappingMisses = Meter.CreateCounter<long>(
        "honorarium.mapping.misses",
        unit: "{misses}",
        description: "Número de fallos en el mapeo de honorarios (sin regla)");
}
