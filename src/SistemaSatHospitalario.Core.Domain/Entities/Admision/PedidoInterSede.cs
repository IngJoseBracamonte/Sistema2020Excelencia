using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class PedidoInterSede
    {
        public Guid Id { get; private set; }
        public string Correlativo { get; private set; } // Formato: PED-YYYY-XXXX
        public Guid SedeSolicitanteId { get; private set; }
        public virtual Sede SedeSolicitante { get; private set; }
        public Guid SedeProveedoraId { get; private set; }
        public virtual Sede SedeProveedora { get; private set; }
        public EstadoPedidoInterSede Estado { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaDespacho { get; private set; }
        public DateTime? FechaRecepcion { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioCreadorId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioCreadorId. Columna legacy pendiente de DROP.")]
        public string UsuarioCreador { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que creó el pedido.</summary>
        public Guid? UsuarioCreadorId { get; private set; }
        public string Observaciones { get; private set; }

        public virtual ICollection<PedidoInterSedeDetalle> Detalles { get; private set; } = new List<PedidoInterSedeDetalle>();

        private PedidoInterSede() { }

        public PedidoInterSede(string correlativo, Guid sedeSolicitanteId, Guid sedeProveedoraId, string usuarioCreador, string observaciones)
        {
            if (sedeSolicitanteId == sedeProveedoraId)
            {
                throw new InvalidOperationException("La sede solicitante no puede ser la misma que la sede proveedora.");
            }

            Id = Guid.NewGuid();
            Correlativo = correlativo ?? throw new ArgumentNullException(nameof(correlativo));
            SedeSolicitanteId = sedeSolicitanteId;
            SedeProveedoraId = sedeProveedoraId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioCreador = usuarioCreador ?? throw new ArgumentNullException(nameof(usuarioCreador));
#pragma warning restore CS0618
            // 3FN: poblar la FK si el texto es un GUID válido
            UsuarioCreadorId = Guid.TryParse(usuarioCreador, out var parsed) ? parsed : (Guid?)null;
            Observaciones = observaciones ?? string.Empty;
            Estado = EstadoPedidoInterSede.Solicitado;
            FechaCreacion = DateTime.UtcNow;
        }

        public void CambiarEstado(EstadoPedidoInterSede nuevoEstado)
        {
            Estado = nuevoEstado;
            if (nuevoEstado == EstadoPedidoInterSede.Despachado)
            {
                FechaDespacho = DateTime.UtcNow;
            }
            else if (nuevoEstado == EstadoPedidoInterSede.Recibido)
            {
                FechaRecepcion = DateTime.UtcNow;
            }
        }

        public void SetObservaciones(string observaciones)
        {
            Observaciones = observaciones ?? string.Empty;
        }

        public void AgregarDetalle(PedidoInterSedeDetalle detalle)
        {
            Detalles.Add(detalle);
        }
    }
}
