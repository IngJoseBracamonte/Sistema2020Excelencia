using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var services = new ServiceCollection();
        string connectionString = "Server=localhost;Port=3306;Database=sistema2020;Uid=root;Pwd=Labordono1818;Allow User Variables=True";
        
        services.AddDbContext<Sistema2020LegacyDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Sistema2020LegacyDbContext>();

        Console.WriteLine("--- ÚLTIMAS 5 ÓRDENES EN SISTEMA2020 ---");
        var orders = await context.Orden
            .OrderByDescending(o => o.IdOrden)
            .Take(5)
            .ToListAsync();

        foreach (var o in orders)
        {
            Console.WriteLine($"ID: {o.IdOrden}, Persona: {o.IdPersona}, Fecha: {o.Fecha:yyyy-MM-dd}, Hora: {o.HoraIngreso}");
        }

        if (!orders.Any()) Console.WriteLine("No se encontraron órdenes.");
    }
}
