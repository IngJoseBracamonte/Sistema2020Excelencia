namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class SeguroConvenioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PorcentajeCobertura { get; set; }
        public bool Activo { get; set; }
    }
}
