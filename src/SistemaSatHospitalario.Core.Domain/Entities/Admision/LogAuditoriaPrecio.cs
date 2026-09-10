using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class LogAuditoriaPrecio
    {
        public Guid Id { get; private set; }
        public Guid DetalleServicioId { get; private set; }
        public string DescripcionServicio { get; private set; } = string.Empty;
        
        // Registro de Cambios de Precio y Honorarios
        public decimal PrecioOriginal { get; private set; }
        public decimal PrecioModificado { get; private set; }
        public decimal HonorarioAnterior { get; private set; }
        public decimal NuevoHonorario { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioOperadorId { get; private set; }
        public string? UsuarioOperador { get; private set; } // Alias legacy opcional

        public Guid? AutorizadoPorId { get; private set; }
        public string? AutorizadoPor { get; private set; } // Alias legacy opcional

        public DateTime FechaModificacion { get; private set; }

        public virtual DetalleServicioCuenta DetalleServicio { get; private set; } = null!;

        protected LogAuditoriaPrecio() { }

        public LogAuditoriaPrecio(
            Guid detalleServicioId, 
            string descripcionServicio, 
            decimal precioOriginal, 
            decimal precioModificado, 
            decimal honorarioAnterior,
            decimal nuevoHonorario,
            Guid? usuarioOperadorId = null,
            string? usuarioOperador = null,
            Guid? autorizadoPorId = null,
            string? autorizadoPor = null)
        {
            Id = Guid.NewGuid();
            DetalleServicioId = detalleServicioId;
            DescripcionServicio = descripcionServicio ?? throw new ArgumentNullException(nameof(descripcionServicio));
            PrecioOriginal = precioOriginal;
            PrecioModificado = precioModificado;
            HonorarioAnterior = honorarioAnterior;
            NuevoHonorario = nuevoHonorario;
            
            // 3FN: Asignar ID directo o intentar parsear string si vino el alias
            UsuarioOperadorId = usuarioOperadorId ?? (Guid.TryParse(usuarioOperador, out var parsedOp) ? parsedOp : (Guid?)null);
            UsuarioOperador = usuarioOperador;

            AutorizadoPorId = autorizadoPorId ?? (Guid.TryParse(autorizadoPor, out var parsedAut) ? parsedAut : (Guid?)null);
            AutorizadoPor = autorizadoPor;

            FechaModificacion = DateTime.UtcNow;
        }
    }
}