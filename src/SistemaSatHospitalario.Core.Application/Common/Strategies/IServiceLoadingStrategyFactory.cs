using System;
using System.Collections.Generic;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Common.Strategies
{
    public interface IServiceLoadingStrategyFactory
    {
        IServiceLoadingStrategy GetStrategy(string tipoServicio, ServicioClinico? baseService);
    }

    public class ServiceLoadingStrategyFactory : IServiceLoadingStrategyFactory
    {
        private readonly IEnumerable<IServiceLoadingStrategy> _strategies;

        public ServiceLoadingStrategyFactory(IEnumerable<IServiceLoadingStrategy> strategies)
        {
            _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
        }

        public IServiceLoadingStrategy GetStrategy(string tipoServicio, ServicioClinico? baseService)
        {
            var strategy = _strategies.FirstOrDefault(s => !(s is FallbackLoadingStrategy) && s.CanHandle(tipoServicio, baseService))
                           ?? _strategies.FirstOrDefault(s => s is FallbackLoadingStrategy);
            if (strategy == null)
            {
                throw new InvalidOperationException($"No se encontró una estrategia de carga para el servicio de tipo '{tipoServicio}'.");
            }
            return strategy;
        }
    }
}
