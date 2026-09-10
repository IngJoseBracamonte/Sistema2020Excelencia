using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HonorarioConfig
    {
        public Guid Id { get; private set; }
        public string CategoriaServicio { get; private set; } // RX, INFORME, CITOLOGIA, BIOPSIA, CONSULTA
        public Guid? MedicoDefaultId { get; private set; }
        public virtual Medico MedicoDefault { get; private set; }
        public Guid? UsuarioConfiguroId { get; private set; }
        public DateTime FechaConfiguracion { get; private set; }
        public string? NotasConfig { get; private set; }

        protected HonorarioConfig() { }

        public HonorarioConfig(string categoriaServicio, string usuario, Guid? usuarioId = null)
        {
            Id = Guid.NewGuid();
            CategoriaServicio = categoriaServicio ?? throw new ArgumentNullException(nameof(categoriaServicio));
            SetUsuarioConfiguro(usuario, usuarioId);
            FechaConfiguracion = DateTime.UtcNow;
        }

        public void AsignarMedicoDefault(Guid medicoId, string usuario, string? notas = null, Guid? usuarioId = null)
        {
            MedicoDefaultId = medicoId;
            SetUsuarioConfiguro(usuario, usuarioId);
            FechaConfiguracion = DateTime.UtcNow;
            NotasConfig = notas;
        }

        public void LimpiarMedicoDefault(string usuario, Guid? usuarioId = null)
        {
            MedicoDefaultId = null;
            SetUsuarioConfiguro(usuario, usuarioId);
            FechaConfiguracion = DateTime.UtcNow;
            NotasConfig = "Limpiado por " + usuario;
        }

        private void SetUsuarioConfiguro(string usuario, Guid? usuarioId)
        {
            UsuarioConfiguroId = usuarioId ?? (Guid.TryParse(usuario, out var parsed) ? parsed : (Guid?)null);
        }
    }
}
