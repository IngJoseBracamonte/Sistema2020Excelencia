using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Common.Services
{
    public interface IHonorariumMapperService
    {
        Task<string> MapToCategoryAsync(string tipoServicio);
        void InvalidateCache();
    }

    public class HonorariumMapperService : IHonorariumMapperService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "HonorariumMappingRules";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public HonorariumMapperService(IApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<string> MapToCategoryAsync(string tipoServicio)
        {
            if (string.IsNullOrWhiteSpace(tipoServicio)) return HonorarioConstants.CategoriaOtros;

            var rules = await GetRulesAsync();
            var normalizedType = tipoServicio.ToUpperInvariant();

            foreach (var rule in rules.OrderBy(r => r.Priority))
            {
                bool isMatch = rule.MatchType switch
                {
                    MappingRuleType.Contains => normalizedType.Contains(rule.Pattern.ToUpperInvariant()),
                    MappingRuleType.StartsWith => normalizedType.StartsWith(rule.Pattern.ToUpperInvariant()),
                    MappingRuleType.Equals => normalizedType.Equals(rule.Pattern.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase),
                    _ => false
                };

                if (isMatch)
                {
                    DiagnosticsConfig.HonorariumMappingHits.Add(1, new KeyValuePair<string, object?>("category", rule.Category));
                    return rule.Category;
                }
            }

            DiagnosticsConfig.HonorariumMappingMisses.Add(1);
            return HonorarioConstants.CategoriaOtros;
        }

        public void InvalidateCache()
        {
            _cache.Remove(CacheKey);
        }

        private async Task<List<HonorariumMappingRule>> GetRulesAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out List<HonorariumMappingRule> rules))
            {
                rules = await _context.HonorariumMappingRules
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Priority)
                    .ToListAsync();

                _cache.Set(CacheKey, rules, CacheDuration);
            }
            return rules;
        }
    }
}
