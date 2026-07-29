using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CompromisoPago
    {
        public Guid Id { get; private set; }
        public Guid CuentaPorCobrarId { get; private set; }
        public bool Omitido { get; private set; }
        public string? Observacion { get; private set; }
        public string UsuarioCreacion { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        public virtual CuentaPorCobrar CuentaPorCobrar { get; private set; } = null!;

        protected CompromisoPago() 
        {
            UsuarioCreacion = string.Empty;
        }

        public CompromisoPago(Guid cuentaPorCobrarId, string usuarioCreacion, bool omitido = false, string? observacion = null)
        {
            Id = Guid.NewGuid();
            CuentaPorCobrarId = cuentaPorCobrarId;
            UsuarioCreacion = usuarioCreacion ?? throw new ArgumentNullException(nameof(usuarioCreacion));
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
    }
}
