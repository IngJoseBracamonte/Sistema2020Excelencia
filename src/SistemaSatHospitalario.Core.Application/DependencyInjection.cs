using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SistemaSatHospitalario.Core.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registra MediatR y todos los Handlers en el ensamblado actual
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
