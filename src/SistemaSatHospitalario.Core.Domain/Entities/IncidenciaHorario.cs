using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public class IncidenciaHorario
    {
        public Guid Id { get; private set; }
        public Guid MedicoId { get; private set; }
        public DateTime Inicio { get; private set; }
        public DateTime Fin { get; private set; }
        public TipoComentarioHorario Tipo { get; private set; }
        public string Descripcion { get; private set; }
        public Guid CreadoPor { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        private IncidenciaHorario() { }

        public IncidenciaHorario(Guid medicoId, DateTime inicio, DateTime fin, TipoComentarioHorario tipo, string descripcion, Guid creadoPor)
        {
            if (inicio >= fin) throw new ArgumentException("La fecha de inicio debe ser anterior a la de fin.");
            
            Id = Guid.NewGuid();
            MedicoId = medicoId;
            Inicio = inicio;
            Fin = fin;
            Tipo = tipo;
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            CreadoPor = creadoPor;
            FechaCreacion = DateTime.UtcNow;
        }

        public bool SolapaCon(DateTime horaAComparar)
        {
            return horaAComparar >= Inicio && horaAComparar <= Fin;
        }
    }

    public enum TipoComentarioHorario
    {
        Informativo = 1,
        Incidencia = 2
    }
}
