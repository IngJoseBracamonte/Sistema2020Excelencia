        using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class PrecioServicioConvenio
    {
        public Guid Id { get; private set; }
        public Guid ServicioClinicoId { get; private set; }
        // Se cambió de Guid a int para sincronización con Legacy
        public int SeguroConvenioId { get; private set; }
        public decimal PrecioDiferencial { get; private set; }

        public virtual ServicioClinico Servicio { get; private set; }
        public virtual SeguroConvenio Convenio { get; private set; }

        private PrecioServicioConvenio() { }

        public PrecioServicioConvenio(Guid servicioId, int convenioId, decimal precio)
        {
            Id = Guid.NewGuid();
            ServicioClinicoId = servicioId;
            SeguroConvenioId = convenioId;
            PrecioDiferencial = precio;
        }
    }
}
