using System;
using System.Collections.Generic;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetUnifiedCatalogQuery : IRequest<List<CatalogItemDto>>
    {
        // Se cambió de Guid? a int? para sincronización con Legacy
        public int? ConvenioId { get; set; }
    }
}
