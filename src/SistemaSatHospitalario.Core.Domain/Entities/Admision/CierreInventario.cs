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

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioId. Columna legacy pendiente de DROP.")]
        public string Usuario { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que cerró el inventario.</summary>
        public Guid? UsuarioId { get; private set; }
        public string Observaciones { get; private set; }

        public virtual ICollection<CierreInventarioDetalle> Detalles { get; private set; } = new List<CierreInventarioDetalle>();

        protected CierreInventario() { }

        public CierreInventario(Guid sedeId, string usuario, string observaciones)
        {
            Id = Guid.NewGuid();
            SedeId = sedeId;
            FechaCierre = DateTime.UtcNow;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioId = Guid.TryParse(usuario, out var parsed) ? parsed : (Guid?)null;
            Observaciones = observaciones ?? string.Empty;
        }

        public void AgregarDetalle(CierreInventarioDetalle detalle)
        {
            if (detalle == null) throw new ArgumentNullException(nameof(detalle));
            Detalles.Add(detalle);
        }
    }
}
