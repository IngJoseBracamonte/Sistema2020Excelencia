using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Representa un médico participante en un equipo quirúrgico con su honorario individual asignado.
    /// Normalizado en 3FN: el rol/tipo del médico está tipado por EspecialidadId (FK a Especialidad)
    /// y el flag booleano EsCirujanoPrincipal. Cero cadenas de texto libres.
    /// </summary>
    public class CirugiaMedicoHonorario
    {
        public Guid Id { get; private set; }
        public Guid OrdenCirugiaId { get; private set; }
        public Guid MedicoId { get; private set; }
        public Guid EspecialidadId { get; private set; }
        public decimal MontoHonorarioUsd { get; private set; }
        public bool EsCirujanoPrincipal { get; private set; }

        // Navegación
        public virtual OrdenCirugia OrdenCirugia { get; private set; }
        public virtual Medico Medico { get; private set; }
        public virtual Especialidad Especialidad { get; private set; }

        protected CirugiaMedicoHonorario() { }

        public CirugiaMedicoHonorario(
            Guid ordenCirugiaId,
            Guid medicoId,
            Guid especialidadId,
            decimal montoHonorarioUsd,
            bool esCirujanoPrincipal = false)
        {
            if (ordenCirugiaId == Guid.Empty)
                throw new ArgumentException("La orden de cirugía es obligatoria.", nameof(ordenCirugiaId));
            if (medicoId == Guid.Empty)
                throw new ArgumentException("El médico es obligatorio.", nameof(medicoId));
            if (especialidadId == Guid.Empty)
                throw new ArgumentException("La especialidad es obligatoria.", nameof(especialidadId));
            if (montoHonorarioUsd < 0)
                throw new ArgumentException("El honorario no puede ser negativo.", nameof(montoHonorarioUsd));

            Id = Guid.NewGuid();
            OrdenCirugiaId = ordenCirugiaId;
            MedicoId = medicoId;
            EspecialidadId = especialidadId;
            MontoHonorarioUsd = montoHonorarioUsd;
            EsCirujanoPrincipal = esCirujanoPrincipal;
        }

        public void ActualizarHonorario(decimal nuevoMontoUsd, Guid? nuevaEspecialidadId = null)
        {
            if (nuevoMontoUsd < 0)
                throw new ArgumentException("El honorario no puede ser negativo.", nameof(nuevoMontoUsd));

            MontoHonorarioUsd = nuevoMontoUsd;
            if (nuevaEspecialidadId.HasValue && nuevaEspecialidadId.Value != Guid.Empty)
            {
                EspecialidadId = nuevaEspecialidadId.Value;
            }
        }
    }
}
