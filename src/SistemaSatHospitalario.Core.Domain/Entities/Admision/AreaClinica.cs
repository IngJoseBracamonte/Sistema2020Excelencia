using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class AreaClinica
    {
        public Guid Id { get; private set; }
        public Guid SedeId { get; private set; }
        public virtual Sede Sede { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }

        // Strong-typed state and custom admission rates properties
        public EstadoUbicacion Estado { get; private set; }
        public bool EsAreaAdmision { get; private set; }
        public Guid? ServicioTarifaBaseId { get; private set; }
        public virtual ServicioClinico? ServicioTarifaBase { get; private set; }

        private AreaClinica() { }

        public AreaClinica(Guid sedeId, string codigo, string nombre, bool esAreaAdmision = false, Guid? servicioTarifaBaseId = null)
        {
            Id = Guid.NewGuid();
            SedeId = sedeId;
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Activo = true;
            Estado = EstadoUbicacion.Disponible;
            EsAreaAdmision = esAreaAdmision;
            ServicioTarifaBaseId = servicioTarifaBaseId;
        }

        public void Update(string codigo, string nombre, bool esAreaAdmision = false, Guid? servicioTarifaBaseId = null)
        {
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            EsAreaAdmision = esAreaAdmision;
            ServicioTarifaBaseId = servicioTarifaBaseId;
        }

        public void SetEstado(bool activo)
        {
            Activo = activo;
        }

        // Encapsulated state transitions
        public void MarcarComoOcupada()
        {
            Estado = EstadoUbicacion.Ocupada;
        }

        public void Liberar()
        {
            Estado = EstadoUbicacion.Disponible;
        }

        public void MarcarEnRetencionQuirurgica()
        {
            Estado = EstadoUbicacion.RetencionQuirurgica;
        }

        public void AsignarServicioTarifa(ServicioClinico? servicio)
        {
            ServicioTarifaBase = servicio;
            ServicioTarifaBaseId = servicio?.Id;
        }
    }
}
