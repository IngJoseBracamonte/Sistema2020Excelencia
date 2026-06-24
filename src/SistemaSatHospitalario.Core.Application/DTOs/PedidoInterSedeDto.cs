using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs
{
    public class PedidoInterSedeDto
    {
        public Guid Id { get; set; }
        public string Correlativo { get; set; }
        public Guid SedeSolicitanteId { get; set; }
        public string SedeSolicitanteNombre { get; set; }
        public Guid SedeProveedoraId { get; set; }
        public string SedeProveedoraNombre { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaDespacho { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string UsuarioCreador { get; set; }
        public string Observaciones { get; set; }
        public List<PedidoInterSedeDetalleDto> Detalles { get; set; } = new List<PedidoInterSedeDetalleDto>();
    }

    public class PedidoInterSedeDetalleDto
    {
        public Guid Id { get; set; }
        public Guid InsumoId { get; set; }
        public string InsumoNombre { get; set; }
        public string InsumoCodigo { get; set; }
        public decimal CantidadSolicitada { get; set; }
        public decimal CantidadDespachada { get; set; }
        public decimal CantidadRecibida { get; set; }
    }
}
