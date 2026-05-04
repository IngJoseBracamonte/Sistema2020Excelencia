using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HonorarioConfig
    {
        public Guid Id { get; private set; }
        public string CategoriaServicio { get; private set; } // RX, INFORME, CITOLOGIA, BIOPSIA, CONSULTA
        public Guid? MedicoDefaultId { get; private set; }
        public virtual Medico MedicoDefault { get; private set; }
        public string UsuarioConfiguro { get; private set; }
        public DateTime FechaConfiguracion { get; private set; }
        public string? NotasConfig { get; private set; }

        protected HonorarioConfig() { }

        public HonorarioConfig(string categoriaServicio, string usuario)
        {
            Id = Guid.NewGuid();
            CategoriaServicio = categoriaServicio ?? throw new ArgumentNullException(nameof(categoriaServicio));
            UsuarioConfiguro = usuario;
            FechaConfiguracion = DateTime.UtcNow;
        }

        public void AsignarMedicoDefault(Guid medicoId, string usuario, string? notas = null)
        {
            MedicoDefaultId = medicoId;
            UsuarioConfiguro = usuario;
            FechaConfiguracion = DateTime.UtcNow;
            NotasConfig = notas;
        }

        public void LimpiarMedicoDefault(string usuario)
        {
            MedicoDefaultId = null;
            UsuarioConfiguro = usuario;
            FechaConfiguracion = DateTime.UtcNow;
            NotasConfig = "Limpiado por " + usuario;
        }
    }
}
