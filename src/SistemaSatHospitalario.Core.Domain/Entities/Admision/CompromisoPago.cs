using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CompromisoPago
    {
        public Guid Id { get; private set; }
        public Guid CuentaPorCobrarId { get; private set; }
        public bool Omitido { get; private set; }
        public string? Observacion { get; private set; }

        /// <summary>
        /// LEGACY (3FN): username en texto plano. Fuente de verdad: <see cref="UsuarioCreacionId"/>
        /// (FK lógica a Usuarios, PK Guid). Se mantiene mapeado como alias de compatibilidad
        /// hasta el DROP de columna (delta posterior a validación en producción).
        /// </summary>
        [Obsolete("Usar UsuarioCreacionId. Columna legacy pendiente de DROP.")]
        public string UsuarioCreacion { get; private set; }

        /// <summary>FK lógica a Usuarios (Identity, PK Guid) del usuario que registró el compromiso.</summary>
        public Guid? UsuarioCreacionId { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        /// <summary>3FN: FK al catálogo MotivosAutorizacion (para omisiones autorizadas).</summary>
        public int? MotivoAutorizacionId { get; private set; }

        public virtual CuentaPorCobrar CuentaPorCobrar { get; private set; } = null!;
        public virtual MotivoAutorizacion? MotivoAutorizacion { get; private set; }

        protected CompromisoPago()
        {
#pragma warning disable CS0618
            UsuarioCreacion = string.Empty;
#pragma warning restore CS0618
        }

        public CompromisoPago(Guid cuentaPorCobrarId, string usuarioCreacion, bool omitido = false, string? observacion = null, Guid? usuarioCreacionId = null)
        {
            Id = Guid.NewGuid();
            CuentaPorCobrarId = cuentaPorCobrarId;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UsuarioCreacion = usuarioCreacion ?? throw new ArgumentNullException(nameof(usuarioCreacion));
#pragma warning restore CS0618
            // 3FN: si no se pasa la FK explícita, intentar parsear el texto como GUID
            UsuarioCreacionId = usuarioCreacionId ?? (Guid.TryParse(usuarioCreacion, out var parsed) ? parsed : (Guid?)null);
            Omitido = omitido;
            Observacion = observacion;
            FechaCreacion = DateTime.UtcNow;
        }

        public void Omitir(string observacion)
        {
            if (string.IsNullOrWhiteSpace(observacion))
            {
                throw new ArgumentException("La observación es obligatoria al omitir el compromiso de pago.", nameof(observacion));
            }
            Omitido = true;
            Observacion = observacion;
        }

        /// <summary>3FN: omisión con motivo catalogado (fuente de verdad relacional).</summary>
        public void OmitirConMotivo(int motivoAutorizacionId, string? observacionAdicional = null)
        {
            if (motivoAutorizacionId <= 0)
            {
                throw new ArgumentException("El motivo de autorización es obligatorio al omitir el compromiso de pago.", nameof(motivoAutorizacionId));
            }
            Omitido = true;
            MotivoAutorizacionId = motivoAutorizacionId;
            if (!string.IsNullOrWhiteSpace(observacionAdicional))
            {
                Observacion = observacionAdicional;
            }
        }
    }
}
