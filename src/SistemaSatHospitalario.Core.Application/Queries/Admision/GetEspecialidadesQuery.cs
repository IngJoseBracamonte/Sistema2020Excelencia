using System.Collections.Generic;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetEspecialidadesQuery : IRequest<List<EspecialidadDto>>
    {
    }
}
