using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class LogAuditoriaPrecio
    {
        public Guid Id { get; private set; }
        public Guid DetalleServicioId { get; private set; }
        public string DescripcionServicio { get; private set; }
        public decimal PrecioOriginal { get; private set; }
        public decimal PrecioModificado { get; private set; }
        public decimal HonorarioAnterior { get; private set; }
        public decimal NuevoHonorario { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre de usuario en texto plano. Fuente de verdad:
        /// <see cref="UsuarioOperadorId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar UsuarioOperadorId. Columna legacy pendiente de DROP.")]
        public string UsuarioOperador { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del operador que modificó el precio.</summary>
        public Guid? UsuarioOperadorId { get; private set; }

        /// <summary>
        /// LEGACY (3FN): nombre del autorizador en texto plano. Fuente de verdad:
        /// <see cref="AutorizadoPorId"/> (FK lógica a Usuarios, PK Guid). Alias hasta el DROP.
        /// </summary>
        [Obsolete("Usar AutorizadoPorId. Columna legacy pendiente de DROP.")]
        public string AutorizadoPor { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que autorizó el cambio.</summary>
        public Guid? AutorizadoPorId { get; private set; }
        public DateTime FechaModificacion { get; private set; }

        public virtual DetalleServicioCuenta DetalleServicio { get; private set; } = null!;

        protected LogAuditoriaPrecio() { }

        public LogAuditoriaPrecio(
            Guid detalleServicioId, 
            string descripcionServicio, 
            decimal precioOriginal, 
            decimal precioModificado, 
            decimal honorarioAnterior,
            decimal nuevoHonorary,
            string usuarioOperador, 
            string autorizadoPor)
        {
            Id = Guid.NewGuid();
            DetalleServicioId = detalleServicioId;
            DescripcionServicio = descripcionServicio ?? throw new ArgumentNullException(nameof(descripcionServicio));
            PrecioOriginal = precioOriginal;
            PrecioModificado = precioModificado;
            HonorarioAnterior = honorarioAnterior;
            NuevoHonorario = nuevoHonorary;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioOperador = usuarioOperador ?? throw new ArgumentNullException(nameof(usuarioOperador));
            AutorizadoPor = autorizadoPor ?? throw new ArgumentNullException(nameof(autorizadoPor));
#pragma warning restore CS0618
            // 3FN: poblar las FKs si los textos son GUIDs válidos
            UsuarioOperadorId = Guid.TryParse(usuarioOperador, out var parsedOp) ? parsedOp : (Guid?)null;
            AutorizadoPorId = Guid.TryParse(autorizadoPor, out var parsedAut) ? parsedAut : (Guid?)null;
            FechaModificacion = DateTime.UtcNow;
        }
    }
}
