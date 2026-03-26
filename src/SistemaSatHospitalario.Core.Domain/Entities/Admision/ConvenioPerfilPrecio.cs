using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ConvenioPerfilPrecio
    {
        public Guid Id { get; private set; }
        
        // Relación con SeguroConvenio (ID legacy int)
        public int SeguroConvenioId { get; private set; }
        
        // Relación con PerfilLegacy (ID legacy int)
        public int PerfilId { get; private set; }
        
        // Precios personalizados
        public decimal PrecioHNL { get; private set; }
        public decimal PrecioUSD { get; private set; }
        
        public bool Activo { get; private set; }
        public DateTime UltimaActualizacion { get; private set; }

        public virtual SeguroConvenio Convenio { get; private set; }

        private ConvenioPerfilPrecio() { }

        public ConvenioPerfilPrecio(int convenioId, int perfilId, decimal precioHnl, decimal precioUsd)
        {
            Id = Guid.NewGuid();
            SeguroConvenioId = convenioId;
            PerfilId = perfilId;
            PrecioHNL = precioHnl;
            PrecioUSD = precioUsd;
            Activo = true;
            UltimaActualizacion = DateTime.UtcNow;
        }

        public void ActualizarPrecio(decimal nuevoHnl, decimal nuevoUsd)
        {
            PrecioHNL = nuevoHnl;
            PrecioUSD = nuevoUsd;
            UltimaActualizacion = DateTime.UtcNow;
        }

        public void SetActivo(bool activo) => Activo = activo;
    }
}
