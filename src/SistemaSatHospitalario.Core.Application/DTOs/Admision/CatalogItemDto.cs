using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CatalogItemDto
    {
        public string Id { get; set; } // Puede ser Guid o IdPersona/IdPerfil string
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; } // CONSULTA, RX, LABORATORIO, etc.
        public bool EsLegacy { get; set; }
    }
}
