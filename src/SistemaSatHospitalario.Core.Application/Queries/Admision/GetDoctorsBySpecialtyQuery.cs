using System.Collections.Generic;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetDoctorsBySpecialtyQuery : IRequest<List<DoctorDto>>
    {
        public string Especialidad { get; set; }

        public GetDoctorsBySpecialtyQuery(string especialidad)
        {
            Especialidad = especialidad;
        }
    }
}
