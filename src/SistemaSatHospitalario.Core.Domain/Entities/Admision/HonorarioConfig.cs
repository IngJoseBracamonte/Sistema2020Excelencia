using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HonorarioConfig
    {
        public Guid Id { get; private set; }
        public string CategoriaServicio { get; private set; } // RX, INFORME, CITOLOGIA, BIOPSIA, CONSULTA
        public Guid? MedicoDefaultId { get; private set; }
        public virtual Medico MedicoDefault { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioConfiguroId"/> (FK lógica a Usuarios, PK Guid).
        /// Se mantiene mapeado como alias de compatibilidad hasta el DROP de columna.
        /// </summary>
        [Obsolete("Usar UsuarioConfiguroId. Columna legacy pendiente de DROP.")]
        public string UsuarioConfiguro { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que configuró.</summary>
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
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioConfiguro = usuario;
#pragma warning restore CS0618
            // 3FN: si no se pasa la FK explícita, intentar parsear el texto como GUID
            UsuarioConfiguroId = usuarioId ?? (Guid.TryParse(usuario, out var parsed) ? parsed : (Guid?)null);
        }
    }
}
