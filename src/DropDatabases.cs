using System;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Legacy;

string connSystem = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
string connIdentity = "server=localhost;database=SatHospitalarioIdentity;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
string connLegacy = "server=localhost;database=sistema2020;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None;Allow User Variables=True";

Console.WriteLine("--- ELIMINANDO BASES DE DATOS PARA RESET TOTAL ---");

try {
    var optionsIdentity = new DbContextOptionsBuilder<SatHospitalarioIdentityDbContext>().UseMySql(connIdentity, ServerVersion.AutoDetect(connIdentity)).Options;
    using var contextIdentity = new SatHospitalarioIdentityDbContext(optionsIdentity);
    Console.WriteLine("Eliminando Identity...");
    contextIdentity.Database.EnsureDeleted();
    
    var optionsSystem = new DbContextOptionsBuilder<SatHospitalarioDbContext>().UseMySql(connSystem, ServerVersion.AutoDetect(connSystem)).Options;
    using var contextSystem = new SatHospitalarioDbContext(optionsSystem);
    Console.WriteLine("Eliminando Application...");
    contextSystem.Database.EnsureDeleted();
    
    var optionsLegacy = new DbContextOptionsBuilder<Sistema2020LegacyDbContext>().UseMySql(connLegacy, ServerVersion.AutoDetect(connLegacy)).Options;
    using var contextLegacy = new Sistema2020LegacyDbContext(optionsLegacy);
    Console.WriteLine("Eliminando Legacy...");
    contextLegacy.Database.EnsureDeleted();

    Console.WriteLine("¡Bases de datos eliminadas con éxito!");
} catch (Exception ex) {
    Console.WriteLine($"ERROR: {ex.Message}");
}
