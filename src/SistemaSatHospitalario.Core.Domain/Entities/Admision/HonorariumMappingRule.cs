using System;
using SistemaSatHospitalario.Core.Domain.Enums; // ✅ Importa el Enum desde su namespace correspondiente

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HonorariumMappingRule
    {
        public Guid Id { get; private set; }
        public string Pattern { get; private set; } = string.Empty;
        public string Category { get; private set; } = string.Empty;
        public MappingRuleType MappingRuleType { get; private set; }
        public int Priority { get; private set; }
        public bool IsActive { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioCreoId { get; private set; }
        public string? UsuarioCreo { get; private set; } // Alias legacy opcional
        public DateTime FechaCreacion { get; private set; }

        protected HonorariumMappingRule() { }

        public HonorariumMappingRule(
            string pattern, 
            string category, 
            MappingRuleType mappingRuleType, 
            int priority, 
            Guid? usuarioCreoId = null,
            string? usuarioCreo = null)
        {
            Id = Guid.NewGuid();
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            MappingRuleType = mappingRuleType;
            Priority = priority;
            IsActive = true;
            
            // 3FN: Asignar ID directo o intentar parsear string si vino el alias
            UsuarioCreoId = usuarioCreoId ?? (Guid.TryParse(usuarioCreo, out var parsed) ? parsed : (Guid?)null);
            UsuarioCreo = usuarioCreo;
            FechaCreacion = DateTime.UtcNow;
        }

        public void Update(string pattern, string category, MappingRuleType mappingRuleType, int priority, bool isActive)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            MappingRuleType = mappingRuleType;
            Priority = priority;
            IsActive = isActive;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }

}