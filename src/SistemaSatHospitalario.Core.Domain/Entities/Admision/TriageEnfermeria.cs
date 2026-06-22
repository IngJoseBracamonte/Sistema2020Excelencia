using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class TriageEnfermeria
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public string MotivoConsulta { get; private set; }
        public string TensionArterial { get; private set; }
        public int FrecuenciaCardiaca { get; private set; }
        public int FrecuenciaRespiratoria { get; private set; }
        public decimal Temperatura { get; private set; }
        public int SaturacionO2 { get; private set; }
        public int? GlicemiaCapilar { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public string UsuarioRegistro { get; private set; }
        public string? DescripcionRapida { get; private set; }
        public string? DescripcionDetallada { get; private set; }

        public virtual CuentaServicios CuentaServicio { get; private set; }

        protected TriageEnfermeria() { }

        public TriageEnfermeria(
            Guid cuentaServicioId, 
            string motivoConsulta, 
            string tensionArterial, 
            int frecuenciaCardiaca, 
            int frecuenciaRespiratoria, 
            decimal temperatura, 
            int saturacionO2, 
            int? glicemiaCapilar, 
            string usuarioRegistro,
            string? descripcionRapida = null,
            string? descripcionDetallada = null)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            MotivoConsulta = motivoConsulta ?? throw new ArgumentNullException(nameof(motivoConsulta));
            TensionArterial = tensionArterial ?? throw new ArgumentNullException(nameof(tensionArterial));
            FrecuenciaCardiaca = frecuenciaCardiaca;
            FrecuenciaRespiratoria = frecuenciaRespiratoria;
            Temperatura = temperatura;
            SaturacionO2 = saturacionO2;
            GlicemiaCapilar = glicemiaCapilar;
            FechaRegistro = DateTime.UtcNow;
            UsuarioRegistro = usuarioRegistro ?? throw new ArgumentNullException(nameof(usuarioRegistro));
            DescripcionRapida = descripcionRapida;
            DescripcionDetallada = descripcionDetallada;
        }

        public void ActualizarDatos(
            string motivoConsulta, 
            string tensionArterial, 
            int frecuenciaCardiaca, 
            int frecuenciaRespiratoria, 
            decimal temperatura, 
            int saturacionO2, 
            int? glicemiaCapilar, 
            string usuarioRegistro,
            string? descripcionRapida = null,
            string? descripcionDetallada = null)
        {
            MotivoConsulta = motivoConsulta ?? throw new ArgumentNullException(nameof(motivoConsulta));
            TensionArterial = tensionArterial ?? throw new ArgumentNullException(nameof(tensionArterial));
            FrecuenciaCardiaca = frecuenciaCardiaca;
            FrecuenciaRespiratoria = frecuenciaRespiratoria;
            Temperatura = temperatura;
            SaturacionO2 = saturacionO2;
            GlicemiaCapilar = glicemiaCapilar;
            UsuarioRegistro = usuarioRegistro ?? throw new ArgumentNullException(nameof(usuarioRegistro));
            DescripcionRapida = descripcionRapida;
            DescripcionDetallada = descripcionDetallada;
        }

        public void AsignarDescripcionEstado(string? rapida, string? detallada)
        {
            DescripcionRapida = rapida;
            DescripcionDetallada = detallada;
        }
    }
}
