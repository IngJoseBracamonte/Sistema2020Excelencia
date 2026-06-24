using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class DetalleServicioMedicoResponsable
    {
        public Guid Id { get; private set; }
        public Guid DetalleServicioCuentaId { get; private set; }
        public virtual DetalleServicioCuenta DetalleServicioCuenta { get; private set; }
        public Guid MedicoId { get; private set; }
        public virtual Medico Medico { get; private set; }
        public string Rol { get; private set; } // Cirujano Principal, Ayudante, Anestesiólogo, etc.
        public decimal MontoHonorario { get; private set; }

        private DetalleServicioMedicoResponsable() { }

        public DetalleServicioMedicoResponsable(Guid detalleServicioCuentaId, Guid medicoId, string rol, decimal montoHonorario)
        {
            Id = Guid.NewGuid();
            DetalleServicioCuentaId = detalleServicioCuentaId;
            MedicoId = medicoId;
            Rol = rol ?? throw new ArgumentNullException(nameof(rol));
            MontoHonorario = montoHonorario;
        }
    }
}
