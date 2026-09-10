using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class PedidoInterSede
    {
        public Guid Id { get; private set; }
        public string Correlativo { get; private set; } = string.Empty; // Formato: PED-YYYY-XXXX
        public Guid SedeSolicitanteId { get; private set; }
        public virtual Sede SedeSolicitante { get; private set; } = null!;
        public Guid SedeProveedoraId { get; private set; }
        public virtual Sede SedeProveedora { get; private set; } = null!;
        public EstadoPedidoInterSedeConstants Estado { get; private set; }
        
        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaDespacho { get; private set; }
        public DateTime? FechaRecepcion { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioCreadorId { get; private set; }
        public string? UsuarioCreador { get; private set; } // Alias legacy opcional
        public string Observaciones { get; private set; } = string.Empty;

        // Colección de renglones del pedido (1:N)
        public virtual ICollection<PedidoInterSedeDetalle> Detalles { get; private set; } = new List<PedidoInterSedeDetalle>();

        protected PedidoInterSede() { }

        public PedidoInterSede(
            string correlativo, 
            Guid sedeSolicitanteId, 
            Guid sedeProveedoraId, 
            Guid? usuarioCreadorId = null,
            string? usuarioCreador = null, 
            string? observaciones = null)
        {
            if (sedeSolicitanteId == sedeProveedoraId)
            {
                throw new InvalidOperationException("La sede solicitante no puede ser la misma que la sede proveedora.");
            }

            Id = Guid.NewGuid();
            Correlativo = correlativo ?? throw new ArgumentNullException(nameof(correlativo));
            SedeSolicitanteId = sedeSolicitanteId;
            SedeProveedoraId = sedeProveedoraId;
            
            // 3FN: Asignar ID directo o intentar parsear string si vino el alias
            UsuarioCreadorId = usuarioCreadorId ?? (Guid.TryParse(usuarioCreador, out var parsed) ? parsed : (Guid?)null);
            UsuarioCreador = usuarioCreador;

            Observaciones = observaciones ?? string.Empty;
            Estado = EstadoPedidoInterSedeConstants.Solicitado;
            FechaCreacion = DateTime.UtcNow;
        }

        public void CambiarEstado(EstadoPedidoInterSedeConstants nuevoEstado)
        {
            Estado = nuevoEstado;
            if (nuevoEstado == EstadoPedidoInterSedeConstants.Despachado)
            {
                FechaDespacho = DateTime.UtcNow;
            }
            else if (nuevoEstado == EstadoPedidoInterSedeConstants.Recibido)
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
            ArgumentNullException.ThrowIfNull(detalle);
            Detalles.Add(detalle);
        }
    }
}