using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class AgendarTurnoCommand : IRequest<Guid>
    {
        public Guid MedicoId { get; set; }
        public Guid PacienteId { get; set; }
        public DateTime FechaHoraToma { get; set; }
        public bool IgnorarIncidencia { get; set; }
        public Guid? IncidenciaIgnoradaId { get; set; }
    }
}
