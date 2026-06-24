using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CierreInventario
    {
        public Guid Id { get; private set; }
        public Guid SedeId { get; private set; }
        public virtual Sede Sede { get; private set; }
        public DateTime FechaCierre { get; private set; }
        public string Usuario { get; private set; }
        public string Observaciones { get; private set; }

        public virtual ICollection<CierreInventarioDetalle> Detalles { get; private set; } = new List<CierreInventarioDetalle>();

        protected CierreInventario() { }

        public CierreInventario(Guid sedeId, string usuario, string observaciones)
        {
            Id = Guid.NewGuid();
            SedeId = sedeId;
            FechaCierre = DateTime.UtcNow;
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            Observaciones = observaciones ?? string.Empty;
        }

        public void AgregarDetalle(CierreInventarioDetalle detalle)
        {
            if (detalle == null) throw new ArgumentNullException(nameof(detalle));
            Detalles.Add(detalle);
        }
    }
}
