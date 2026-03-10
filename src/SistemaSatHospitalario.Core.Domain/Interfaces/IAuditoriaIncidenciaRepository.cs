using System;
using System.Threading;
using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Domain.Entities;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IAuditoriaIncidenciaRepository
    {
        Task RegistrarAsync(RegistroAuditoriaIncidencia registro, CancellationToken cancellationToken);
    }
}
