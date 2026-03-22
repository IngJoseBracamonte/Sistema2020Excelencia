using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreatePatientCommand : IRequest<PatientDto>
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
    }
}
