using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HistorialLimpiezaCama
    {
        public Guid Id { get; private set; }
        public Guid CamaId { get; private set; }
        public virtual AreaClinica Cama { get; private set; }
        public DateTime FechaInicio { get; private set; }
        public DateTime? FechaFin { get; private set; }
        public string UsuarioInicio { get; private set; }
        public string? UsuarioFin { get; private set; }
        public string? Observaciones { get; private set; }

        protected HistorialLimpiezaCama() { }

        public HistorialLimpiezaCama(Guid camaId, DateTime fechaInicio, string usuarioInicio, string? observaciones = null)
        {
            Id = Guid.NewGuid();
            CamaId = camaId;
            FechaInicio = fechaInicio;
            UsuarioInicio = usuarioInicio ?? throw new ArgumentNullException(nameof(usuarioInicio));
            Observaciones = observaciones;
        }

        public void FinalizarLimpieza(DateTime fechaFin, string usuarioFin)
        {
            if (FechaFin.HasValue)
            {
                throw new InvalidOperationException("La limpieza de esta cama ya ha sido finalizada.");
            }
            if (fechaFin < FechaInicio)
            {
                throw new InvalidOperationException("La fecha de fin no puede ser anterior a la fecha de inicio.");
            }

            FechaFin = fechaFin;
            UsuarioFin = usuarioFin ?? throw new ArgumentNullException(nameof(usuarioFin));
        }
    }
}
