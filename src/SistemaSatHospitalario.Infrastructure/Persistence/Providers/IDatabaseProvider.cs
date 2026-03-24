using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Providers
{
    public interface IDatabaseProvider
    {
        string Name { get; }
        void ConfigureIdentityContext(IServiceCollection services, IConfiguration configuration);
        void ConfigureApplicationContext(IServiceCollection services, IConfiguration configuration);
        void ConfigureLegacyContext(IServiceCollection services, IConfiguration configuration);
    }
}
