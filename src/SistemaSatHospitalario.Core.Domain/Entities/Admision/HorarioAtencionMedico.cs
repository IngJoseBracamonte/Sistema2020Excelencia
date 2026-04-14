using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HorarioAtencionMedico
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        
        /// <summary>
        /// 0 = Domingo, 1 = Lunes, ..., 6 = Sabado
        /// </summary>
        public int DiaSemana { get; private set; }
        
        public TimeSpan HoraInicio { get; private set; }
        public TimeSpan HoraFin { get; private set; }

        private HorarioAtencionMedico() { }

        public HorarioAtencionMedico(Guid medicoId, int diaSemana, TimeSpan inicio, TimeSpan fin)
        {
            if (diaSemana < 0 || diaSemana > 6) throw new ArgumentException("Día de la semana inválido.");
            if (inicio >= fin) throw new ArgumentException("La hora de inicio debe ser anterior a la de fin.");

            Id = Guid.NewGuid();
            MedicoId = medicoId;
            DiaSemana = diaSemana;
            HoraInicio = inicio;
            HoraFin = fin;
        }

        public void Update(int diaSemana, TimeSpan inicio, TimeSpan fin)
        {
            DiaSemana = diaSemana;
            HoraInicio = inicio;
            HoraFin = fin;
        }
    }
}
