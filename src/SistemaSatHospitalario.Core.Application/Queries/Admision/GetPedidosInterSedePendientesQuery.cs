using System;
using System.Collections.Generic;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetPedidosInterSedePendientesQuery : IRequest<List<PedidoInterSedeDto>>
    {
    }
}
