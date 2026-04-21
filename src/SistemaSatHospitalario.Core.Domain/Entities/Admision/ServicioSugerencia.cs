using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ServicioSugerencia
    {
        public Guid Id { get; private set; }
        public Guid ServicioOrigenId { get; private set; }
        public Guid ServicioSugeridoId { get; private set; }

        public virtual ServicioClinico ServicioOrigen { get; private set; }
        public virtual ServicioClinico ServicioSugerido { get; private set; }

        private ServicioSugerencia() { } // EF Core

        public ServicioSugerencia(Guid servicioOrigenId, Guid servicioSugeridoId)
        {
            Id = Guid.NewGuid();
            ServicioOrigenId = servicioOrigenId;
            ServicioSugeridoId = servicioSugeridoId;
        }
    }
}
