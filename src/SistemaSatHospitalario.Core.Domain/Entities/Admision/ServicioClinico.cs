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
        public int TipoServicioId { get; set; }
        public string? LegacyMappingId { get; set; }
        public ServiceCategory Category { get; set; } 
        public string? HonorariumCategory { get; set; } // Nuevo: Clasificación explícita para honorarios
        public bool Activo { get; set; }
        public string? UnidadMedida { get; set; }
        public bool PermiteFraccionamiento { get; set; }
        public bool RequiereInventario { get; set; } = true;
        public Guid? EspecialidadId { get; set; }
        public virtual Especialidad? Especialidad { get; set; }
        public virtual ICollection<ServicioSugerencia> Sugerencias { get; private set; } = new List<ServicioSugerencia>();
        public virtual ICollection<HonorarioMedicoServicio> HonorariosMedicos { get; private set; } = new List<HonorarioMedicoServicio>();

        private ServicioClinico() { }

        public ServicioClinico(string codigo, string descripcion, decimal precioBase, string tipoServicio, string? legacyMappingId = null)
        {
            Id = Guid.NewGuid();
            Codigo = codigo;
            Descripcion = descripcion;
            PrecioBase = precioBase;
            TipoServicio = tipoServicio;
            TipoServicioId = MapearTipoServicioAId(tipoServicio);
            Activo = true;
        }

        public void SetEspecialidad(Guid especialidadId)
        {
            EspecialidadId = especialidadId;
        }

        public void Desactivar() => Activo = false;
        public void ActualizarPrecio(decimal nuevoPrecio) => PrecioBase = nuevoPrecio;

        private static int MapearTipoServicioAId(string tipoServicio)
        {
            if (string.IsNullOrWhiteSpace(tipoServicio)) return Constants.TipoServicioConstants.Insumo;
            var t = tipoServicio.ToUpperInvariant();
            if (t == "LABORATORIO" || t == "LAB") return Constants.TipoServicioConstants.Laboratorio;
            if (t == "RX") return Constants.TipoServicioConstants.RX;
            if (t == "TOMO" || t == "TOMOGRAFIA" || t == "TOMOGRAFÍA") return Constants.TipoServicioConstants.Tomo;
            if (t == "MEDICO" || t == "MEDICA" || t == "MÉDICO" || t == "MÉDICA" || t == "CONSULTA") return Constants.TipoServicioConstants.Medico;
            return Constants.TipoServicioConstants.Insumo;
        }
    }
}
