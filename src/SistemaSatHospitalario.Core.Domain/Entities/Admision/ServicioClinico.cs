using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ServicioClinico
    {
        public Guid Id { get; private set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioBase { get; set; }
        public decimal HonorarioBase { get; set; }
        public string TipoServicio { get; set; } // Legacy compatibility
        public string? LegacyMappingId { get; set; }
        public ServiceCategory Category { get; set; } 
        public bool Activo { get; set; }
        public Guid? EspecialidadId { get; set; }
        public virtual Especialidad? Especialidad { get; set; }
        public virtual ICollection<ServicioSugerencia> Sugerencias { get; private set; } = new List<ServicioSugerencia>();

        private ServicioClinico() { }

        public ServicioClinico(string codigo, string descripcion, decimal precioBase, string tipoServicio, string? legacyMappingId = null)
        {
            Id = Guid.NewGuid();
            Codigo = codigo;
            Descripcion = descripcion;
            PrecioBase = precioBase;
            TipoServicio = tipoServicio;
            Activo = true;
        }

        public void SetEspecialidad(Guid especialidadId)
        {
            EspecialidadId = especialidadId;
        }

        public void Desactivar() => Activo = false;
        public void ActualizarPrecio(decimal nuevoPrecio) => PrecioBase = nuevoPrecio;
    }
}
