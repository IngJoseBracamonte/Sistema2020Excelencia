namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class SeguroConvenioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Rtn { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool Activo { get; set; }
    }
}
