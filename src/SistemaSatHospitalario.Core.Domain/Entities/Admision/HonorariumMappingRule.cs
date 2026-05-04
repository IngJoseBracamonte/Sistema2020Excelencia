using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public enum MappingRuleType
    {
        Contains = 1,
        StartsWith = 2,
        Equals = 3,
        Regex = 4
    }

    public class HonorariumMappingRule
    {
        public Guid Id { get; private set; }
        public string Pattern { get; private set; }
        public string Category { get; private set; }
        public MappingRuleType MatchType { get; private set; }
        public int Priority { get; private set; }
        public bool IsActive { get; private set; }
        public string UsuarioCreo { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        protected HonorariumMappingRule() { }

        public HonorariumMappingRule(string pattern, string category, MappingRuleType matchType, int priority, string usuario)
        {
            Id = Guid.NewGuid();
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            MatchType = matchType;
            Priority = priority;
            IsActive = true;
            UsuarioCreo = usuario;
            FechaCreacion = DateTime.UtcNow;
        }

        public void Update(string pattern, string category, MappingRuleType matchType, int priority, bool isActive)
        {
            Pattern = pattern;
            Category = category;
            MatchType = matchType;
            Priority = priority;
            IsActive = isActive;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
