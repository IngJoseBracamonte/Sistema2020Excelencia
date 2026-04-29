using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class CachedLegacyLabRepository : ILegacyLabRepository
    {
        private readonly ILegacyLabRepository _innerRepository;
        private readonly IMemoryCache _cache;
        
        // Tiempos de expiración (Micro-Ciclo 29)
        private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromHours(1);
        private static readonly TimeSpan LongCacheDuration = TimeSpan.FromHours(4);
        private static readonly TimeSpan ShortCacheDuration = TimeSpan.FromMinutes(10);

        public CachedLegacyLabRepository(ILegacyLabRepository innerRepository, IMemoryCache cache)
        {
            _innerRepository = innerRepository;
            _cache = cache;
        }

        public async Task<int> GenerarOrdenLaboratorioAsync(
            OrdenLegacy orden, 
            List<PerfilesFacturadosLegacy> perfilesAFacturar, 
            List<ResultadosPacienteLegacy> resultados, 
            CancellationToken cancellationToken)
        {
            // Operación de escritura: No se cachea, se delega directamente
            return await _innerRepository.GenerarOrdenLaboratorioAsync(orden, perfilesAFacturar, resultados, cancellationToken);
        }

        public async Task<DatosPersonalesLegacy?> GetPatientByCedulaAsync(string cedula, CancellationToken cancellationToken)
        {
            var cacheKey = $"Patient_{cedula}";
            
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = DefaultCacheDuration;
                entry.SlidingExpiration = TimeSpan.FromMinutes(20);
                return await _innerRepository.GetPatientByCedulaAsync(cedula, cancellationToken);
            });
        }

        public async Task<DatosPersonalesLegacy?> GetPatientByIdAsync(string legacyId, CancellationToken cancellationToken)
        {
            var cacheKey = $"PatientId_{legacyId}";
            
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = DefaultCacheDuration;
                entry.SlidingExpiration = TimeSpan.FromMinutes(20);
                return await _innerRepository.GetPatientByIdAsync(legacyId, cancellationToken);
            });
        }

        public async Task<List<DatosPersonalesLegacy>> SearchPatientsLimitedAsync(string term, CancellationToken cancellationToken)
        {
            var cacheKey = $"PatientSearch_{term.ToLower().Trim()}";
            
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = ShortCacheDuration;
                return await _innerRepository.SearchPatientsLimitedAsync(term, cancellationToken);
            }) ?? new List<DatosPersonalesLegacy>();
        }

        public async Task<List<PerfilLegacy>> GetAvailableProfilesAsync(CancellationToken cancellationToken)
        {
            const string cacheKey = "AvailableProfiles_vF";
            
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = LongCacheDuration;
                return await _innerRepository.GetAvailableProfilesAsync(cancellationToken);
            }) ?? new List<PerfilLegacy>();
        }

        public async Task<int> CreatePatientLegacyAsync(DatosPersonalesLegacy patient, CancellationToken cancellationToken)
        {
            // Escritura: Delegamos e invalidamos la búsqueda por cédula si existe
            var id = await _innerRepository.CreatePatientLegacyAsync(patient, cancellationToken);
            var cacheKey = $"Patient_{patient.Cedula}";
            _cache.Remove(cacheKey);
            return id;
        }

        public async Task<List<int>> GetLegacyAgreementsIdsAsync(CancellationToken cancellationToken)
        {
            const string cacheKey = "LegacyAgreementIds";
            
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = LongCacheDuration;
                return await _innerRepository.GetLegacyAgreementsIdsAsync(cancellationToken);
            }) ?? new List<int>();
        }

        public async Task<int?> GetMuestraStatusAsync(int legacyOrderId, CancellationToken cancellationToken)
        {
            var cacheKey = $"MuestraStatus_{legacyOrderId}";
            
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = ShortCacheDuration;
                return await _innerRepository.GetMuestraStatusAsync(legacyOrderId, cancellationToken);
            });
        }
    }
}
