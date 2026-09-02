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

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioCreoId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioCreoId. Columna legacy pendiente de DROP.")]
        public string UsuarioCreo { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que creó la regla.</summary>
        public Guid? UsuarioCreoId { get; private set; }
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
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioCreo = usuario;
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioCreoId = Guid.TryParse(usuario, out var parsed) ? parsed : (Guid?)null;
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
