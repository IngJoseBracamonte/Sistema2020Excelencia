using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

namespace SistemaSatHospitalario.WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration, string policyName)
        {
            var allowedOriginsStr = configuration["AllowedOrigins"] ?? "https://localhost:4200,http://localhost:4200";
            var allowedOrigins = allowedOriginsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var deployMode = configuration["DeploymentSettings:Mode"] ?? "Local";

            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    if (deployMode.Equals("Cloud", StringComparison.OrdinalIgnoreCase))
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else
                    {
                        policy.SetIsOriginAllowed(origin => true)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddFixedWindowLimiter("fixed", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 100;
                    opt.QueueLimit = 2;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomIdentityAndJwt(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            var jwtSecret = configuration["JwtConfig:Secret"] ?? "DefaultSecretKey_MustBeChangedInProduction_1234567890123456";
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !isDevelopment;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtConfig:Issuer"] ?? "SistemaSatHospitalarioAPI",
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtConfig:Audience"] ?? "SistemaSatHospitalario_PWA",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "unique_name",
                    RoleClaimType = "role",
                    RequireExpirationTime = true
                };
            });

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddCustomForwardedHeaders(this IServiceCollection services)
        {
            services.Configure<Microsoft.AspNetCore.HttpOverrides.ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                                         Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
                
                // Limpiar redes conocidas para confiar en los encabezados del proxy de la nube (Render/Azure)
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            return services;
        }

        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                    .AddDbContextCheck<SatHospitalarioDbContext>("Database");
            return services;
        }

        public static IServiceCollection AddCustomCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnection = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                    options.InstanceName = "SatHospitalario_";
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }
            return services;
        }
    }
}
