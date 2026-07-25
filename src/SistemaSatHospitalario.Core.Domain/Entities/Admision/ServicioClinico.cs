using System;
using System.Collections.Generic;
using SistemaSatHospitalario.Core.Domain.Constants;
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
        public string? HonorariumCategory { get; set; } // Clasificación explícita para honorarios
        public bool Activo { get; set; }
        public string? UnidadMedida { get; set; }
        public bool PermiteFraccionamiento { get; set; }
        public bool RequiereInventario { get; set; } = true;
        public Guid? EspecialidadId { get; set; }
        public virtual Especialidad? Especialidad { get; set; }

        // Vínculo relacional a Servicio de Informe Médico
        public Guid? ServicioInformeId { get; set; }
        public virtual ServicioClinico? ServicioInforme { get; set; }
        public bool EsServicioInforme { get; set; } = false;

        // Auditoría Soft Delete
        public string? DesactivadoPorUsuarioId { get; set; }
        public DateTime? FechaDesactivacion { get; set; }

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

        public void Desactivar(string usuarioId)
        {
            Activo = false;
            DesactivadoPorUsuarioId = usuarioId;
            FechaDesactivacion = DateTime.UtcNow;
        }

        public void Desactivar() => Desactivar("Sistema");

        public void ActualizarPrecio(decimal nuevoPrecio) => PrecioBase = nuevoPrecio;

        public void ValidarInvariantes()
        {
            if (TipoServicioId == TipoServicioConstants.Informe || EsServicioInforme)
            {
                if (PrecioBase < HonorarioBase)
                {
                    throw new InvalidOperationException($"El precio base del informe (${PrecioBase:F2}) no puede ser inferior al honorario asignado al médico (${HonorarioBase:F2}).");
                }
            }
        }

        private static int MapearTipoServicioAId(string tipoServicio)
        {
            if (string.IsNullOrWhiteSpace(tipoServicio)) return TipoServicioConstants.Insumo;
            var t = tipoServicio.ToUpperInvariant();
            if (t == "INFORME" || t == "INFORME MEDICO" || t == "INFORME MÉDICO") return TipoServicioConstants.Informe;
            if (t == "LABORATORIO" || t == "LAB") return TipoServicioConstants.Laboratorio;
            if (t == "RX") return TipoServicioConstants.RX;
            if (t == "TOMO" || t == "TOMOGRAFIA" || t == "TOMOGRAFÍA") return TipoServicioConstants.Tomo;
            if (t == "MEDICO" || t == "MEDICA" || t == "MÉDICO" || t == "MÉDICA" || t == "CONSULTA") return TipoServicioConstants.Medico;
            return TipoServicioConstants.Insumo;
        }
    }
}
