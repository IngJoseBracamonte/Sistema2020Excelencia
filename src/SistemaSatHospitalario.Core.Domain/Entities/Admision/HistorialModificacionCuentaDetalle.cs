using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HistorialModificacionCuentaDetalle
    {
        public Guid Id { get; private set; }
        public Guid HistorialModificacionCuentaId { get; private set; }
        public Guid DetalleServicioId { get; private set; }
        public decimal PrecioAnterior { get; private set; }
        public decimal PrecioNuevo { get; private set; }
        public decimal HonorarioAnterior { get; private set; }
        public decimal HonorarioNuevo { get; private set; }
        public decimal CantidadAnterior { get; private set; }
        public decimal CantidadNueva { get; private set; }

        public virtual HistorialModificacionCuenta HistorialModificacionCuenta { get; private set; } = null!;
        public virtual DetalleServicioCuenta DetalleServicio { get; private set; } = null!;

        private HistorialModificacionCuentaDetalle() { }

        public HistorialModificacionCuentaDetalle(Guid historialModificacionCuentaId, Guid detalleServicioId, decimal precioAnterior, decimal precioNuevo, decimal honorarioAnterior, decimal honorarioNuevo, decimal cantidadAnterior, decimal cantidadNueva)
        {
            if (historialModificacionCuentaId == Guid.Empty) throw new ArgumentException("El historial es requerido.", nameof(historialModificacionCuentaId));
            if (detalleServicioId == Guid.Empty) throw new ArgumentException("El detalle de servicio es requerido.", nameof(detalleServicioId));

            Id = Guid.NewGuid();
            HistorialModificacionCuentaId = historialModificacionCuentaId;
            DetalleServicioId = detalleServicioId;
            PrecioAnterior = precioAnterior;
            PrecioNuevo = precioNuevo;
            HonorarioAnterior = honorarioAnterior;
            HonorarioNuevo = honorarioNuevo;
            CantidadAnterior = cantidadAnterior;
            CantidadNueva = cantidadNueva;
        }
    }
}