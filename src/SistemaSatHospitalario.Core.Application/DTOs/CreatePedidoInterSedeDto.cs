using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs
{
    public class CreatePedidoInterSedeDto
    {
        public Guid SedeSolicitanteId { get; set; }
        public Guid SedeProveedoraId { get; set; }
        public string Observaciones { get; set; }
        public List<PedidoInterSedeLineaDto> Lineas { get; set; } = new List<PedidoInterSedeLineaDto>();
    }

    public class PedidoInterSedeLineaDto
    {
        public Guid InsumoId { get; set; }
        public decimal CantidadSolicitada { get; set; }
    }
}
