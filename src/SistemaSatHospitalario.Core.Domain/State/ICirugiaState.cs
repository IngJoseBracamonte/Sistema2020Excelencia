using System;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Domain.State
{
    /// <summary>
    /// Contrato del Patrón State para encapsular la lógica de transiciones de ciclo de vida de una cirugía.
    /// </summary>
    public interface ICirugiaState
    {
        string NombreEstado { get; }
        void IniciarEspera(OrdenCirugia cirugia, string usuarioId);
        void IniciarCirugia(OrdenCirugia cirugia, string usuarioId);
        void FinalizarCirugia(OrdenCirugia cirugia, string usuarioId);
        void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, string usuarioId);
        void Cancelar(OrdenCirugia cirugia, string motivo, string usuarioId);
    }

    public abstract class BaseCirugiaState : ICirugiaState
    {
        public abstract string NombreEstado { get; }

        public virtual void IniciarEspera(OrdenCirugia cirugia, string usuarioId)
        {
            throw new InvalidOperationException($"Solo se puede ingresar a sala de espera en estado Pendiente de Ejecución / Programada.");
        }

        public virtual void IniciarCirugia(OrdenCirugia cirugia, string usuarioId)
        {
            throw new InvalidOperationException($"Solo se puede iniciar una cirugía en estado Pendiente de Ejecución / Programada o En Espera.");
        }

        public virtual void FinalizarCirugia(OrdenCirugia cirugia, string usuarioId)
        {
            throw new InvalidOperationException($"Solo se puede completar / finalizar una cirugía que está En Proceso / En Cirugía.");
        }

        public virtual void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo u observación es obligatorio para reprogramar una cirugía.", nameof(motivo));

            cirugia.ActualizarFechaYMotivoReprogramacion(nuevaFecha, motivo, usuarioId);
        }

        public virtual void Cancelar(OrdenCirugia cirugia, string motivo, string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo de cancelación es obligatorio.", nameof(motivo));

            cirugia.SetEstadoInternal(EstadoCirugiaConstants.Cancelada, new CanceladaState());
            cirugia.MotivoCancelacionInternal(motivo);
            cirugia.AgregarLog(usuarioId, CirugiaEventoConstants.TransicionEstado, $"Cirugía cancelada desde {NombreEstado}. Motivo: {motivo}");
            cirugia.AgregarHistorialObservacion($"Cirugía cancelada desde {NombreEstado}. Motivo: {motivo}", "Cancelacion", usuarioId);
        }
    }

    public class ProgramadaState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.Programada;

        public override void IniciarEspera(OrdenCirugia cirugia, string usuarioId)
        {
            cirugia.SetEstadoInternal(EstadoCirugiaConstants.EnEspera, new EnEsperaState());
            cirugia.AgregarLog(usuarioId, CirugiaEventoConstants.TransicionEstado, $"Paso de Programada a EnEspera");
            cirugia.AgregarHistorialObservacion("Paciente ingresado a sala de espera pre-quirúrgica.", "CambioEstado", usuarioId);
        }

        public override void IniciarCirugia(OrdenCirugia cirugia, string usuarioId)
        {
            cirugia.SetEstadoInternal(EstadoCirugiaConstants.EnCirugia, new EnCirugiaState());
            cirugia.AgregarLog(usuarioId, CirugiaEventoConstants.TransicionEstado, $"Paso de Programada a EnCirugia");
            cirugia.AgregarHistorialObservacion("Cirugía iniciada directamente desde Programada.", "CambioEstado", usuarioId);
        }
    }

    public class EnEsperaState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.EnEspera;

        public override void IniciarCirugia(OrdenCirugia cirugia, string usuarioId)
        {
            cirugia.SetEstadoInternal(EstadoCirugiaConstants.EnCirugia, new EnCirugiaState());
            cirugia.AgregarLog(usuarioId, CirugiaEventoConstants.TransicionEstado, $"Paso de EnEspera a EnCirugia");
            cirugia.AgregarHistorialObservacion("Paciente trasladado a pabellón e inicio de cirugía.", "CambioEstado", usuarioId);
        }
    }

    public class EnCirugiaState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.EnCirugia;

        public override void FinalizarCirugia(OrdenCirugia cirugia, string usuarioId)
        {
            cirugia.SetEstadoInternal(EstadoCirugiaConstants.Finalizado, new FinalizadoState());
            cirugia.AgregarLog(usuarioId, CirugiaEventoConstants.TransicionEstado, $"Paso de EnCirugia a Finalizado");
            cirugia.AgregarHistorialObservacion("Cirugía finalizada exitosamente.", "CambioEstado", usuarioId);
        }

        public override void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, string usuarioId)
        {
            throw new InvalidOperationException("No se puede reprogramar una cirugía que se encuentra actualmente en procedimiento (EnCirugia).");
        }
    }

    public class FinalizadoState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.Finalizado;

        public override void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, string usuarioId)
        {
            throw new InvalidOperationException("No se puede reprogramar una cirugía que ya ha sido finalizada.");
        }

        public override void Cancelar(OrdenCirugia cirugia, string motivo, string usuarioId)
        {
            throw new InvalidOperationException("No se puede cancelar una cirugía ya completada / finalizada.");
        }
    }

    public class CanceladaState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.Cancelada;

        public override void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, string usuarioId)
        {
            throw new InvalidOperationException("No se puede reprogramar una cirugía cancelada.");
        }

        public override void Cancelar(OrdenCirugia cirugia, string motivo, string usuarioId)
        {
            throw new InvalidOperationException("La cirugía ya está cancelada.");
        }
    }

    public static class CirugiaStateFactory
    {
        public static ICirugiaState GetState(string estado)
        {
            var e = (estado ?? string.Empty).Trim();
            if (e.Equals(EstadoCirugiaConstants.Programada, StringComparison.OrdinalIgnoreCase) ||
                e.Equals(EstadoCirugiaConstants.PendienteEjecucion, StringComparison.OrdinalIgnoreCase))
            {
                return new ProgramadaState();
            }

            if (e.Equals(EstadoCirugiaConstants.EnEspera, StringComparison.OrdinalIgnoreCase))
            {
                return new EnEsperaState();
            }

            if (e.Equals(EstadoCirugiaConstants.EnCirugia, StringComparison.OrdinalIgnoreCase) ||
                e.Equals(EstadoCirugiaConstants.EnProceso, StringComparison.OrdinalIgnoreCase))
            {
                return new EnCirugiaState();
            }

            if (e.Equals(EstadoCirugiaConstants.Finalizado, StringComparison.OrdinalIgnoreCase) ||
                e.Equals(EstadoCirugiaConstants.Completada, StringComparison.OrdinalIgnoreCase))
            {
                return new FinalizadoState();
            }

            if (e.Equals(EstadoCirugiaConstants.Cancelada, StringComparison.OrdinalIgnoreCase))
            {
                return new CanceladaState();
            }

            return new ProgramadaState();
        }
    }
}
