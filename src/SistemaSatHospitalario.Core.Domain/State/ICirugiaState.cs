using System;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.State
{
    /// <summary>
    /// Contrato del Patrón State para el ciclo de vida de una cirugía.
    /// Recibe Guid? directamente de ICurrentUserService.UserId y valida que el usuario esté autenticado.
    /// </summary>
    public interface ICirugiaState
    {
        string NombreEstado { get; }
        void IniciarEspera(OrdenCirugia cirugia, Guid? usuarioId);
        void IniciarCirugia(OrdenCirugia cirugia, Guid? usuarioId);
        void FinalizarCirugia(OrdenCirugia cirugia, Guid? usuarioId);
        void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, Guid? usuarioId);
        void Cancelar(OrdenCirugia cirugia, string motivo, Guid? usuarioId);
    }

    public abstract class BaseCirugiaState : ICirugiaState
    {
        public abstract string NombreEstado { get; }

        /// <summary>
        /// Valida que el Guid? proveniente de ICurrentUserService sea válido y pertenezca a un usuario autenticado.
        /// </summary>
        protected static Guid ValidarUsuarioAutenticado(Guid? usuarioId)
        {
            if (!usuarioId.HasValue || usuarioId.Value == Guid.Empty)
            {
                throw new InvalidOperationException("Operación quirúrgica denegada: se requiere un usuario autenticado en el sistema.");
            }

            return usuarioId.Value;
        }

        public virtual void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, Guid? usuarioId)
        {
            var id = ValidarUsuarioAutenticado(usuarioId);

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo u observación es obligatorio para reprogramar una cirugía.", nameof(motivo));

            cirugia.ActualizarFechaYMotivoReprogramacion(nuevaFecha, motivo, id);
        }

        public virtual void Cancelar(OrdenCirugia cirugia, string motivo, Guid? usuarioId)
        {
            var id = ValidarUsuarioAutenticado(usuarioId);

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo de cancelación es obligatorio.", nameof(motivo));

            cirugia.SetEstadoInternal(EstadoCirugiaConstants.Cancelada, new CanceladaState());
            cirugia.MotivoCancelacionInternal(motivo);

            var idStr = id.ToString();
            cirugia.AgregarLog(idStr, CirugiaEventoConstants.TransicionEstado, $"Cirugía cancelada desde {NombreEstado}. Motivo: {motivo}");
            cirugia.AgregarHistorialObservacion($"Cirugía cancelada desde {NombreEstado}. Motivo: {motivo}", TipoObservacionCirugiaConstants.Cancelacion, id);
        }

        public virtual void IniciarEspera(OrdenCirugia cirugia, Guid? usuarioId)
        {
            throw new InvalidOperationException($"No se puede ingresar a sala de espera desde el estado actual ({NombreEstado}). Solo permitido desde Programada.");
        }

        public virtual void IniciarCirugia(OrdenCirugia cirugia, Guid? usuarioId)
        {
            throw new InvalidOperationException($"No se puede iniciar la cirugía desde el estado actual ({NombreEstado}). Solo permitido desde Programada o En Espera.");
        }

        public virtual void FinalizarCirugia(OrdenCirugia cirugia, Guid? usuarioId)
        {
            throw new InvalidOperationException($"Solo se puede completar / finalizar una cirugía que está En Proceso / En Cirugía.");
        }
    }

    public class ProgramadaState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.Programada;

        public override void IniciarEspera(OrdenCirugia cirugia, Guid? usuarioId)
        {
            var id = ValidarUsuarioAutenticado(usuarioId);
            var idStr = id.ToString();

            cirugia.SetEstadoInternal(EstadoCirugiaConstants.EnEspera, new EnEsperaState());
            cirugia.AgregarLog(idStr, CirugiaEventoConstants.TransicionEstado, "Paso de Programada a EnEspera");
            cirugia.AgregarHistorialObservacion("Paciente ingresado a sala de espera pre-quirúrgica.", TipoObservacionCirugiaConstants.ObservacionMedica,  id);
        }

        public override void IniciarCirugia(OrdenCirugia cirugia, Guid? usuarioId)
        {
            var id = ValidarUsuarioAutenticado(usuarioId);
            var idStr = id.ToString();

            cirugia.SetEstadoInternal(EstadoCirugiaConstants.EnCirugia, new EnCirugiaState());
            cirugia.AgregarLog(idStr, CirugiaEventoConstants.TransicionEstado, "Paso de Programada a EnCirugia");
            cirugia.AgregarHistorialObservacion("Cirugía iniciada directamente desde Programada.", TipoObservacionCirugiaConstants.ObservacionMedica,id);
        }
    }

    public class EnEsperaState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.EnEspera;

        public override void IniciarCirugia(OrdenCirugia cirugia, Guid? usuarioId)
        {
            var id = ValidarUsuarioAutenticado(usuarioId);
            var idStr = id.ToString();

            cirugia.SetEstadoInternal(EstadoCirugiaConstants.EnCirugia, new EnCirugiaState());
            cirugia.AgregarLog(idStr, CirugiaEventoConstants.TransicionEstado, "Paso de EnEspera a EnCirugia");
            cirugia.AgregarHistorialObservacion("Paciente trasladado a pabellón e inicio de procedimiento quirúrgico.", TipoObservacionCirugiaConstants.ObservacionMedica, id);
        }
    }

    public class EnCirugiaState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.EnCirugia;

        public override void FinalizarCirugia(OrdenCirugia cirugia, Guid? usuarioId)
        {
            var id = ValidarUsuarioAutenticado(usuarioId);
            var idStr = id.ToString();

            cirugia.SetEstadoInternal(EstadoCirugiaConstants.Finalizado, new FinalizadoState());
            cirugia.AgregarLog(idStr, CirugiaEventoConstants.TransicionEstado, "Paso de EnCirugia a Finalizado");
            cirugia.AgregarHistorialObservacion("Procedimiento quirúrgico finalizado exitosamente.", TipoObservacionCirugiaConstants.ObservacionMedica, id);
        }

        public override void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, Guid? usuarioId)
        {
            throw new InvalidOperationException("No se puede reprogramar una cirugía que se encuentra actualmente en procedimiento (EnCirugia).");
        }
    }

    public class FinalizadoState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.Finalizado;

        public override void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, Guid? usuarioId)
        {
            throw new InvalidOperationException("No se puede reprogramar una cirugía que ya ha sido finalizada.");
        }

        public override void Cancelar(OrdenCirugia cirugia, string motivo, Guid? usuarioId)
        {
            throw new InvalidOperationException("No se puede cancelar una cirugía ya completada / finalizada.");
        }
    }

    public class CanceladaState : BaseCirugiaState
    {
        public override string NombreEstado => EstadoCirugiaConstants.Cancelada;

        public override void Reprogramar(OrdenCirugia cirugia, DateTime nuevaFecha, string motivo, Guid? usuarioId)
        {
            throw new InvalidOperationException("No se puede reprogramar una cirugía que ya fue cancelada.");
        }

        public override void Cancelar(OrdenCirugia cirugia, string motivo, Guid? usuarioId)
        {
            throw new InvalidOperationException("La cirugía ya se encuentra cancelada.");
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