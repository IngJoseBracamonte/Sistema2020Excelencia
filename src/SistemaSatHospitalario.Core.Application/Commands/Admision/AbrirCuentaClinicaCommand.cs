using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AbrirCuentaClinicaCommand : IRequest<Guid>
    {
        public Guid PacienteId { get; set; }
        public string TipoIngreso { get; set; } = string.Empty; // Particular, Seguro, Hospitalizacion, UCI, Emergencia
        public int? ConvenioId { get; set; }
        public string UsuarioCarga { get; set; } = string.Empty;
        public Guid? MedicoId { get; set; }
        public Guid? AreaClinicaId { get; set; }
        public bool PermitirBypassExcepcionMedica { get; set; }
    }
}
