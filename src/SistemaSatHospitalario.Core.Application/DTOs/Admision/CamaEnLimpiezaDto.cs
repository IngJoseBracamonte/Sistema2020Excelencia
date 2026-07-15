using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class CamaEnLimpiezaDto
    {
        public Guid HistorialId { get; set; }
        public Guid CamaId { get; set; }
        public string CamaCodigo { get; set; }
        public string CamaNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public string UsuarioInicio { get; set; }
        public string Observaciones { get; set; }
    }
}
