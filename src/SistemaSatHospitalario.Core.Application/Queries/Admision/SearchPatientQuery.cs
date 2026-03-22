using System.Collections.Generic;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class SearchPatientQuery : IRequest<List<PatientDto>>
    {
        public string SearchTerm { get; set; }
    }
}
