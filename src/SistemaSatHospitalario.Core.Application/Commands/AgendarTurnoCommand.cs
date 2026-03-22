using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class AgendarTurnoCommand : IRequest<Guid>
    {
        public Guid MedicoId { get; set; }
        // Se cambió PacienteId a int para paridad con Legacy
        public int PacienteId { get; set; }
        public Guid CuentaServicioId { get; set; }
        public DateTime FechaHoraToma { get; set; }
    }
}
