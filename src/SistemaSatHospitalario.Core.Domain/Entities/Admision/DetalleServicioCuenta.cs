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
        public string TipoServicio { get; private set; } // Medico, RX, Laboratorio, Insumo
        public string UsuarioCarga { get; private set; }
        public DateTime FechaCarga { get; private set; }
        public string? LegacyMappingId { get; private set; }
        public Guid? MedicoResponsableId { get; private set; }
        public string? CategoriaHonorario { get; private set; }
        public Guid? AreaClinicaId { get; private set; }
        public virtual CuentaServicios CuentaServicio { get; private set; }
        public virtual AreaClinica? AreaClinica { get; private set; }
        public virtual System.Collections.Generic.ICollection<DetalleServicioMedicoResponsable> MedicosResponsables { get; private set; } = new System.Collections.Generic.List<DetalleServicioMedicoResponsable>();

        public void AgregarMedicoResponsable(Guid medicoId, string rol, decimal montoHonorario)
        {
            MedicosResponsables.Add(new DetalleServicioMedicoResponsable(Id, medicoId, rol, montoHonorario));
        }


        // [PHASE-6] Technical Validation Fields (Senior Traceability)
        public bool Realizado { get; private set; }
        public DateTime? FechaRealizacion { get; private set; }
        public string? UsuarioTecnico { get; private set; }

        protected DetalleServicioCuenta() { }

        public DetalleServicioCuenta(Guid cuentaServicioId, Guid servicioId, string descripcion, decimal precio, decimal honorario, decimal cantidad, string tipoServicio, string usuarioCarga, string? legacyMappingId = null, Guid? areaClinicaId = null)
        {
            Id = Guid.NewGuid();
            CuentaServicioId = cuentaServicioId;
            ServicioId = servicioId;
            Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
            Precio = precio;
            Honorario = honorario;
            Cantidad = cantidad;
            TipoServicio = tipoServicio ?? throw new ArgumentNullException(nameof(tipoServicio));
            UsuarioCarga = usuarioCarga ?? throw new ArgumentNullException(nameof(usuarioCarga));
            LegacyMappingId = legacyMappingId; // V12.1 Fix: Assigning mapping ID for legacy sync
            FechaCarga = DateTime.UtcNow;
            Realizado = false;
            AreaClinicaId = areaClinicaId;
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

