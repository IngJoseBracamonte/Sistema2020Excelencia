using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CompromisoPago
    {
        public Guid Id { get; private set; }
        public Guid CuentaPorCobrarId { get; private set; }
        public bool Omitido { get; private set; }
        public string? Observacion { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioCreacionId { get; private set; }
        public string? UsuarioCreacion { get; private set; } // Alias legacy opcional
        public DateTime FechaCreacion { get; private set; }

        // Catálogo de Motivos de Autorización (para omisiones autorizadas)
        public int? MotivoAutorizacionId { get; private set; }

        public virtual CuentaPorCobrar CuentaPorCobrar { get; private set; } = null!;
        public virtual MotivoAutorizacion? MotivoAutorizacion { get; private set; }

        protected CompromisoPago() { }

        public CompromisoPago(
            Guid cuentaPorCobrarId, 
            Guid? usuarioCreacionId = null, 
            string? usuarioCreacion = null, 
            bool omitido = false, 
            string? observacion = null)
        {
            Id = Guid.NewGuid();
            CuentaPorCobrarId = cuentaPorCobrarId;
            UsuarioCreacionId = usuarioCreacionId;
            UsuarioCreacion = usuarioCreacion;
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