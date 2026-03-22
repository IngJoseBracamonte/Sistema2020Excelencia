using System.Threading.Tasks;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Seeds
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }
}
