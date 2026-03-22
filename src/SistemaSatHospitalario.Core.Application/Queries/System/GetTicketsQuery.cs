using MediatR;
using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Domain.Entities;

namespace SistemaSatHospitalario.Core.Application.Queries.System
{
    public class GetTicketsQuery : IRequest<List<ErrorTicket>>
    {
        public bool? Resueltos { get; set; }
    }
}
