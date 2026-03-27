using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SistemaSatHospitalario.Core.Application;
using SistemaSatHospitalario.Infrastructure;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using SistemaSatHospitalario.Infrastructure.Identity.Seeds;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;
using Microsoft.AspNetCore.Identity;
using SistemaSatHospitalario.Infrastructure.Identity.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.WebAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire ServiceDefaults for OpenTelemetry and HealthChecks
builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Permitir cualquier origen en Desarrollo para Aspire
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Application Layer Services (MediatR, FluentValidation)
builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddScoped<SistemaSatHospitalario.Core.Application.Common.Interfaces.ITasaNotificationService, SistemaSatHospitalario.WebAPI.Infrastructure.TasaNotificationService>();

// Add JWT Authentication
var jwtSecret = builder.Configuration["JwtConfig:Secret"];
var key = Encoding.ASCII.GetBytes(jwtSecret!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtConfig:Audience"]
    };
});
builder.Services.AddAuthorization();

// Add Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ----- Database Initializer -----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Iniciando secuencia de inicialización de Base de Datos...");
        var initializers = services.GetServices<IDatabaseInitializer>();
        
        foreach (var initializer in initializers)
        {
            logger.LogInformation("Ejecutando inicializador: {InitializerType}", initializer.GetType().Name);
            await initializer.InitializeAsync();
            logger.LogInformation("Finalizado: {InitializerType}", initializer.GetType().Name);
        }
        logger.LogInformation("Secuencia de inicialización completada exitosamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "ERROR CRÍTICO durante la inicialización de bases de datos.");
    }
}
// --------------------------------

app.UseRouting();
app.UseCors("AngularPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SistemaSatHospitalario.WebAPI.Hubs.DashboardHub>("/hub/dashboard");
app.MapHub<SistemaSatHospitalario.WebAPI.Hubs.TasaHub>("/hub/tasa");

app.Run();
