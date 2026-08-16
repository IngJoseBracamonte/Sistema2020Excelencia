using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Representa un requerimiento ad-hoc de insumo o medicamento urgente emitido desde quirófano hacia almacén/farmacia central.
    /// </summary>
    public class SolicitudInsumoCirugia
    {
        public Guid Id { get; private set; }
        public Guid OrdenCirugiaId { get; private set; }
        public Guid InsumoId { get; private set; }
        public decimal CantidadSolicitada { get; private set; }
        public Guid AlmacenOrigenId { get; private set; }
        public string EstadoSolicitud { get; private set; } // Pendiente, Despachado, Rechazado
        public DateTime FechaSolicitud { get; private set; }
        public string UsuarioSolicitud { get; private set; }
        public DateTime? FechaDespacho { get; private set; }
        public string? UsuarioDespacho { get; private set; }
        public string? Observaciones { get; private set; }

        // Navegación
        public virtual OrdenCirugia OrdenCirugia { get; private set; }
        public virtual Insumo Insumo { get; private set; }
        public virtual Sede AlmacenOrigen { get; private set; }

        protected SolicitudInsumoCirugia() { }

        public SolicitudInsumoCirugia(
            Guid ordenCirugiaId,
            Guid insumoId,
            decimal cantidadSolicitada,
            Guid almacenOrigenId,
            string usuarioSolicitud,
            string? observaciones = null)
        {
            if (ordenCirugiaId == Guid.Empty)
                throw new ArgumentException("La orden de cirugía es obligatoria.", nameof(ordenCirugiaId));
            if (insumoId == Guid.Empty)
                throw new ArgumentException("El insumo es obligatorio.", nameof(insumoId));
            if (cantidadSolicitada <= 0)
                throw new ArgumentException("La cantidad solicitada debe ser mayor a cero.", nameof(cantidadSolicitada));
            if (almacenOrigenId == Guid.Empty)
                throw new ArgumentException("El almacén de origen es obligatorio.", nameof(almacenOrigenId));
            if (string.IsNullOrWhiteSpace(usuarioSolicitud))
                throw new ArgumentException("El usuario solicitante es obligatorio.", nameof(usuarioSolicitud));

            Id = Guid.NewGuid();
            OrdenCirugiaId = ordenCirugiaId;
            InsumoId = insumoId;
            CantidadSolicitada = cantidadSolicitada;
            AlmacenOrigenId = almacenOrigenId;
            EstadoSolicitud = EstadoSolicitudInsumoConstants.Pendiente;
            FechaSolicitud = DateTime.UtcNow;
            UsuarioSolicitud = usuarioSolicitud.Trim();
            Observaciones = observaciones?.Trim();
        }

        public void Despachar(string usuarioDespacho)
        {
            if (EstadoSolicitud != EstadoSolicitudInsumoConstants.Pendiente)
                throw new InvalidOperationException($"La solicitud no se encuentra pendiente (Estado actual: {EstadoSolicitud}).");

            EstadoSolicitud = EstadoSolicitudInsumoConstants.Despachado;
            FechaDespacho = DateTime.UtcNow;
            UsuarioDespacho = usuarioDespacho.Trim();
        }

        public void Rechazar(string usuarioDespacho, string motivo)
        {
            if (EstadoSolicitud != EstadoSolicitudInsumoConstants.Pendiente)
                throw new InvalidOperationException($"La solicitud no se encuentra pendiente (Estado actual: {EstadoSolicitud}).");

            EstadoSolicitud = EstadoSolicitudInsumoConstants.Rechazado;
            FechaDespacho = DateTime.UtcNow;
            UsuarioDespacho = usuarioDespacho.Trim();
            Observaciones = string.IsNullOrWhiteSpace(Observaciones) ? motivo : $"{Observaciones} | Rechazo: {motivo}";
        }
    }

    public static class EstadoSolicitudInsumoConstants
    {
        public const string Pendiente = "Pendiente";
        public const string Despachado = "Despachado";
        public const string Rechazado = "Rechazado";
    }
}
