using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class InsumoPrincipioActivo
    {
        public Guid Id { get; private set; }
        public Guid InsumoId { get; private set; }
        public virtual Insumo Insumo { get; private set; }
        public Guid PrincipioActivoId { get; private set; }
        public virtual PrincipioActivo PrincipioActivo { get; private set; }
        public string Concentracion { get; private set; } // Ej: "500mg", "4mg/5ml"

        private InsumoPrincipioActivo() { }

        public InsumoPrincipioActivo(Guid insumoId, Guid principioActivoId, string concentracion)
        {
            Id = Guid.NewGuid();
            InsumoId = insumoId;
            PrincipioActivoId = principioActivoId;
            Concentracion = concentracion?.Trim() ?? string.Empty;
        }

        public InsumoPrincipioActivo(Insumo insumo, PrincipioActivo principioActivo, string concentracion)
        {
            Id = Guid.NewGuid();
            InsumoId = insumo?.Id ?? Guid.Empty;
            Insumo = insumo!;
            PrincipioActivoId = principioActivo?.Id ?? Guid.Empty;
            PrincipioActivo = principioActivo!;
            Concentracion = concentracion?.Trim() ?? string.Empty;
        }

        public void ActualizarConcentracion(string concentracion)
        {
            Concentracion = concentracion?.Trim() ?? string.Empty;
        }
    }
}
