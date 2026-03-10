using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class DetalleServicioCuenta
    {
        public Guid Id { get; private set; }
        public Guid CuentaServicioId { get; private set; }
        public Guid ServicioId { get; private set; }
        public string Descripcion { get; private set; }
        public decimal Precio { get; private set; }
        public int Cantidad { get; private set; }
        public string TipoServicio { get; private set; } // Medico, RX, Laboratorio, Insumo
        public string UsuarioCarga { get; private set; }
        public DateTime FechaCarga { get; private set; }

        protected DetalleServicioCuenta() { }

        internal DetalleServicioCuenta(Guid cuentaServicioId, Guid servicioId, string descripcion, decimal precio, int cantidad, string tipoServicio, string usuarioCarga)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            ServicioId = servicioId;
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            Precio = precio;
            Cantidad = cantidad;
            TipoServicio = tipoServicio ?? throw new ArgumentNullException(nameof(tipoServicio));
            UsuarioCarga = usuarioCarga ?? throw new ArgumentNullException(nameof(usuarioCarga));
            FechaCarga = DateTime.UtcNow;
        }
    }
}
