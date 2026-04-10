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
        public string UsuarioOperador { get; private set; }
        public string AutorizadoPor { get; private set; }
        public DateTime FechaModificacion { get; private set; }

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
            UsuarioOperador = usuarioOperador ?? throw new ArgumentNullException(nameof(usuarioOperador));
            AutorizadoPor = autorizadoPor ?? throw new ArgumentNullException(nameof(autorizadoPor));
            FechaModificacion = DateTime.UtcNow;
        }
    }
}
