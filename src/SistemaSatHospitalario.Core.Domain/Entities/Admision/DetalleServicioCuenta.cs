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
        public decimal Honorario { get; private set; }
        public decimal Cantidad { get; private set; }

        /// <summary>
        /// LEGACY (3FN): texto del tipo de servicio. Fuente de verdad: <see cref="TipoServicioId"/>
        /// y la navegación <see cref="TipoServicioNav"/>. Se mantiene mapeado como alias de
        /// compatibilidad hasta el DROP de columna (delta posterior a validación en producción).
        /// Código nuevo debe usar TipoServicioId / TipoServicioNav.Nombre.
        /// </summary>
        [Obsolete("Usar TipoServicioId / TipoServicioNav. Columna legacy pendiente de DROP.")]
        public string TipoServicio { get; private set; } // Medico, RX, Laboratorio, Insumo, Informe
        public int TipoServicioId { get; private set; }
        public string UsuarioCarga { get; private set; }
        public string? UsuarioCargaId { get; private set; }
        public DateTime FechaCarga { get; private set; }
        public string? LegacyMappingId { get; private set; }
        public Guid? MedicoResponsableId { get; private set; }
        public string? CategoriaHonorario { get; private set; }
        public Guid? AreaClinicaId { get; private set; }

        // Vínculo relacional Padre-Hijo (Informe vinculado a Estudio Base)
        public Guid? DetallePadreId { get; private set; }
        public virtual DetalleServicioCuenta? DetallePadre { get; private set; }

        public bool IncluidoEnTarifaBase { get; private set; }
        public decimal PrecioCatalogoHistorico { get; private set; }
        public virtual CuentaServicios CuentaServicio { get; private set; }
        public virtual AreaClinica? AreaClinica { get; private set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(TipoServicioId))]
        public virtual TipoServicio? TipoServicioNav { get; private set; }
        public virtual System.Collections.Generic.ICollection<DetalleServicioMedicoResponsable> MedicosResponsables { get; private set; } = new System.Collections.Generic.List<DetalleServicioMedicoResponsable>();

        public void AgregarMedicoResponsable(Guid medicoId, string rol, decimal montoHonorario)
        {
            MedicosResponsables.Add(new DetalleServicioMedicoResponsable(Id, medicoId, rol, montoHonorario));
        }

        public decimal ObtenerSubtotal() => IncluidoEnTarifaBase ? 0.00m : (Precio * Cantidad);

        public void MarcarComoIncluidoEnTarifaBase()
        {
            if (IncluidoEnTarifaBase) return;
            PrecioCatalogoHistorico = Precio;
            Precio = 0.00m;
            IncluidoEnTarifaBase = true;
        }

        public void RemoverDeTarifaBase(decimal precioRestaurado)
        {
            Precio = precioRestaurado;
            PrecioCatalogoHistorico = 0.00m;
            IncluidoEnTarifaBase = false;
        }

        public bool Realizado { get; private set; }
        public DateTime? FechaRealizacion { get; private set; }
        public string? UsuarioTecnico { get; private set; }

        protected DetalleServicioCuenta() { }

        public DetalleServicioCuenta(Guid cuentaServicioId, Guid servicioId, string descripcion, decimal precio, decimal honorario, decimal cantidad, string tipoServicio, string usuarioCarga, string? legacyMappingId = null, Guid? areaClinicaId = null, Guid? detallePadreId = null, int? tipoServicioId = null, string? usuarioCargaId = null)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            ServicioId = servicioId;
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            Precio = precio;
            Honorario = honorario;
            Cantidad = cantidad;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            TipoServicio = tipoServicio ?? string.Empty;
#pragma warning restore CS0618
            TipoServicioId = tipoServicioId ?? Constants.TipoServicioConstants.Insumo;
            UsuarioCarga = usuarioCarga ?? throw new ArgumentNullException(nameof(usuarioCarga));
            UsuarioCargaId = usuarioCargaId;
            LegacyMappingId = legacyMappingId;
            FechaCarga = DateTime.UtcNow;
            Realizado = false;
            AreaClinicaId = areaClinicaId;
            DetallePadreId = detallePadreId;
        }

        public void AsignarDetallePadre(Guid detallePadreId)
        {
            DetallePadreId = detallePadreId;
        }

        public void AsignarAreaClinica(Guid areaClinicaId)
        {
            AreaClinicaId = areaClinicaId;
        }

        public void MarcarRealizado(string usuario)
        {
            if (Realizado) return;
            
            Realizado = true;
            FechaRealizacion = DateTime.UtcNow;
            UsuarioTecnico = usuario;
        }

        public void AsignarMedicoResponsable(Guid medicoId, string categoria, decimal? honorario = null)
        {
            MedicoResponsableId = medicoId;
            CategoriaHonorario = categoria;
            if (honorario.HasValue)
            {
                Honorario = honorario.Value;
            }
        }

        public void LimpiarMedicoResponsable()
        {
            MedicoResponsableId = null;
            Honorario = 0.00m;
        }

        public void ModificarPreciosAdministrativos(decimal nuevoPrecio, decimal nuevoHonorario)
        {
            Precio = nuevoPrecio;
            Honorario = nuevoHonorario;
        }

        public void ModificarCantidadAdministrativa(decimal nuevaCantidad)
        {
            Cantidad = nuevaCantidad;
        }
    }
}
