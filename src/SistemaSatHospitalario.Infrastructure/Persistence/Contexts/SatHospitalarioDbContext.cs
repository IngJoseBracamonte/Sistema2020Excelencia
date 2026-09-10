using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Common;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Common;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Infrastructure.Identity.Models;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Contexts
{
    public class SatHospitalarioDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<CajaDiaria> CajasDiarias { get; set; }
        public DbSet<CajaDeclaracionMetodo> CajasDeclaracionesMetodos { get; set; }
        public DbSet<ReciboFactura> RecibosFactura { get; set; }
        public DbSet<DetallePago> DetallesPago { get; set; }
        public DbSet<SeguroConvenio> SegurosConvenios { get; set; }
        public DbSet<PacienteAdmision> PacientesAdmision { get; set; }
        public DbSet<OrdenDeServicio> OrdenesDeServicio { get; set; }
        public DbSet<OrdenRX> OrdenesRX { get; set; }

        public DbSet<TurnoMedico> TurnosMedicos { get; set; }
        public DbSet<IncidenciaHorario> IncidenciasHorario { get; set; }
        public DbSet<RegistroAuditoriaIncidencia> RegistrosAuditoriaIncidencia { get; set; }

        public DbSet<CuentaServicios> CuentasServicios { get; set; }
        public DbSet<DetalleServicioCuenta> DetallesServicioCuenta { get; set; }
        public DbSet<CitaMedica> CitasMedicas { get; set; }
        public DbSet<EstadoCitaMedica> EstadosCitaMedica { get; set; }
        public DbSet<EstadoCaja> EstadosCaja { get; set; }
        public DbSet<EstadoCuenta> EstadosCuenta { get; set; }
        public DbSet<TipoIngreso> TiposIngreso { get; set; }
        public DbSet<EstadoFiscal> EstadosFiscales { get; set; }
        public DbSet<UnidadMedidaCatalogo> UnidadesMedida { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<TasaCambio> TasaCambio { get; set; }
        public DbSet<ServicioClinico> ServiciosClinicos { get; set; }
        public DbSet<TipoServicio> TiposServicio { get; set; }
        public DbSet<ServicioSugerencia> ServiciosSugerencias { get; set; }
        public DbSet<PrecioServicioConvenio> PreciosServicioConvenio { get; set; }
        public DbSet<CuentaPorCobrar> CuentasPorCobrar { get; set; }
        public DbSet<ReservaTemporal> ReservasTemporales { get; set; }
        public DbSet<BloqueoHorario> BloqueosHorarios { get; set; }
        public DbSet<ErrorTicket> ErrorTickets { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<ConfiguracionGeneral> ConfiguracionGeneral { get; set; }
        public DbSet<ConvenioPerfilPrecio> ConvenioPerfilPrecios { get; set; }
        public DbSet<LogAuditoriaPrecio> AuditLogsPrecios { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<HorarioAtencionMedico> HorariosAtencionMedicos { get; set; }
        public DbSet<OrdenImagen> OrdenesImagenes { get; set; }
        public DbSet<CatalogoMetodoPago> CatalogoMetodosPago { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<DocumentLog> DocumentLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<HonorarioConfig> HonorariosConfig { get; set; }
        public DbSet<LogAsignacionHonorario> LogsAsignacionHonorario { get; set; }
        public DbSet<HonorariumMappingRule> HonorariumMappingRules { get; set; }
        public DbSet<HonorarioMedicoServicio> HonorariosMedicosServicios { get; set; }
        public DbSet<GarantiaItem> GarantiasItems { get; set; }
        public DbSet<CompromisoPago> CompromisosPago { get; set; }
        public DbSet<MotivoAutorizacion> MotivosAutorizacion { get; set; }
        public DbSet<HistorialModificacionCuenta> HistorialModificacionCuentas { get; set; }
        public DbSet<HistorialModificacionCuentaDetalle> HistorialModificacionCuentaDetalles { get; set; }
        public DbSet<TriageEnfermeria> TriagesEnfermeria { get; set; }
        public DbSet<ValoracionFisica> ValoracionesFisicas { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<CategoriaInsumo> CategoriasInsumo { get; set; }
        public DbSet<PrincipioActivo> PrincipiosActivos { get; set; }
        public DbSet<InsumoPrincipioActivo> InsumosPrincipiosActivos { get; set; }
        public DbSet<ServicioInsumoReceta> ServiciosInsumoRecetas { get; set; }
        public DbSet<ConsumoServicioRealizado> ConsumosServiciosRealizados { get; set; }
        public DbSet<MovimientoInsumo> MovimientosInsumo { get; set; }
        public DbSet<CierreInventario> CierresInventario { get; set; }
        public DbSet<CierreInventarioDetalle> CierresInventarioDetalles { get; set; }
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<AreaClinica> AreasClinicas { get; set; }
        public DbSet<ClasificacionArea> ClasificacionesAreas { get; set; }
        public DbSet<StockSede> StocksSedes { get; set; }
        public DbSet<PedidoInterSede> PedidosInterSede { get; set; }
        public DbSet<PedidoInterSedeDetalle> PedidosInterSedeDetalles { get; set; }
        public DbSet<DetalleServicioMedicoResponsable> DetallesServicioMedicosResponsables { get; set; }
        public DbSet<ServicioIncluidoArea> ServiciosIncluidosAreas { get; set; }
        public DbSet<InsumoCirugiaPaciente> InsumosCirugiasPacientes { get; set; }
        public DbSet<OrdenCirugia> OrdenesCirugia { get; set; }
        public DbSet<CirugiaLog> CirugiaLogs { get; set; }
        public DbSet<RequisitoCirugia> RequisitosCirugia { get; set; }
        public DbSet<OrdenCirugiaRequisito> OrdenesCirugiaRequisitos { get; set; }
        public DbSet<CirugiaObservacionHistorial> CirugiasObservacionesHistorial { get; set; }
        public DbSet<CirugiaMedicoHonorario> CirugiasMedicosHonorarios { get; set; }
        public DbSet<SolicitudInsumoCirugia> SolicitudesInsumosCirugia { get; set; }
        public DbSet<TransferenciaReposicionStock> TransferenciasReposicionStock { get; set; }
        public DbSet<OrdenCompraInventario> OrdenesCompraInventario { get; set; }
        public DbSet<PagoProveedor> PagosProveedores { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        public SatHospitalarioDbContext(DbContextOptions<SatHospitalarioDbContext> options) : base(options) { }
        public Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) => Database.BeginTransactionAsync(cancellationToken);

        public override int SaveChanges()
        {
            NormalizeAuditEntries();
            EnforceMovimientoInsumoImmutability();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            NormalizeAuditEntries();
            EnforceMovimientoInsumoImmutability();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void NormalizeAuditEntries()
        {
            foreach (var entry in ChangeTracker.Entries<CirugiaLog>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.State = EntityState.Added;
                }
            }
            foreach (var entry in ChangeTracker.Entries<CirugiaObservacionHistorial>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        private void EnforceMovimientoInsumoImmutability()
        {
            foreach (var entry in ChangeTracker.Entries<MovimientoInsumo>())
            {
                if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    throw new InvalidOperationException("Los movimientos de insumos de inventario son inmutables y no se pueden modificar ni eliminar.");
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Ignore<DomainEvent>();

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).Ignore(nameof(BaseEntity.DomainEvents));
                }
            }

            builder.Entity<CajaDiaria>(entity =>
            {
                entity.ToTable("CajasDiarias");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.MontoInicialDivisa).HasPrecision(18, 2);
                entity.Property(c => c.MontoInicialBs).HasPrecision(18, 2);
                entity.Property(c => c.TotalIngresado).HasPrecision(18, 2);
                entity.Property(c => c.UsuarioIdentityId).HasColumnType("char(36)");
                entity.HasIndex(c => c.UsuarioIdentityId);

                entity.HasOne(c => c.EstadoNav)
                    .WithMany()
                    .HasForeignKey(c => c.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(c => c.EstadoId);

                // Relación inversa explícita con las declaraciones
                entity.HasMany(c => c.DeclaracionesPorMetodo)
                    .WithOne(d => d.CajaDiaria)
                    .HasForeignKey(d => d.CajaDiariaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CajaDeclaracionMetodo>(entity =>
            {
                entity.ToTable("CajaDeclaracionesMetodos");
                entity.HasKey(d => d.Id);

                // Precisión para todas las columnas decimales (evita truncamiento y warnings en MySQL)
                entity.Property(d => d.MontoIngresado).HasPrecision(18, 2);
                entity.Property(d => d.MontoVueltos).HasPrecision(18, 2);
                entity.Property(d => d.MontoEsperadoIngreso).HasPrecision(18, 2);
                entity.Property(d => d.MontoEsperadoVueltos).HasPrecision(18, 2);
                entity.Property(d => d.DiferenciaOriginal).HasPrecision(18, 2);
                entity.Property(d => d.DiferenciaBase).HasPrecision(18, 2);

                // Relación con CajaDiaria (1 a Muchos)
                entity.HasOne(d => d.CajaDiaria)
                    .WithMany(c => c.DeclaracionesPorMetodo)
                    .HasForeignKey(d => d.CajaDiariaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con CatalogoMetodoPago (1 a Muchos)
                entity.HasOne(d => d.MetodoPago)
                    .WithMany()
                    .HasForeignKey(d => d.MetodoPagoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices para optimizar consultas
                entity.HasIndex(d => d.CajaDiariaId);
                entity.HasIndex(d => d.MetodoPagoId);
            });

            builder.Entity<EstadoCaja>(entity =>
            {
                entity.ToTable("EstadosCaja");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Codigo).IsUnique();

                entity.HasData(
                    new EstadoCaja(EstadoCajaConstants.AbiertaId, "ABIERTA", "Abierta"),
                    new EstadoCaja(EstadoCajaConstants.CerradaPorAsistenteId, "CERRADA_POR_ASISTENTE", "Cerrada por Asistente"),
                    new EstadoCaja(EstadoCajaConstants.CerradaId, "CERRADA", "Cerrada")
                );
            });

            builder.Entity<CajaDeclaracionMetodo>(entity =>
            {
                entity.ToTable("CajasDeclaracionesMetodos");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.MontoIngresado).HasPrecision(18, 2);
                entity.Property(c => c.MontoVueltos).HasPrecision(18, 2);
                entity.Property(c => c.MontoEsperadoIngreso).HasPrecision(18, 2);
                entity.Property(c => c.MontoEsperadoVueltos).HasPrecision(18, 2);
                entity.Property(c => c.DiferenciaOriginal).HasPrecision(18, 2);
                entity.Property(c => c.DiferenciaBase).HasPrecision(18, 2);

                entity.HasOne(c => c.CajaDiaria)
                      .WithMany()
                      .HasForeignKey(c => c.CajaDiariaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.MetodoPago)
                      .WithMany()
                      .HasForeignKey(c => c.MetodoPagoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => new { c.CajaDiariaId, c.MetodoPagoId }).IsUnique();
                entity.HasIndex(c => c.MetodoPagoId);
            });

            builder.Entity<ReciboFactura>(entity =>
            {
                entity.ToTable("RecibosFacturas");
                entity.HasKey(r => r.Id);

                // ===============================
                // Propiedades de Texto
                // ===============================
                entity.Property(r => r.NumeroRecibo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.NroControlFiscal)
                    .HasMaxLength(50);

                entity.Property(r => r.NumeroComprobante)
                    .HasMaxLength(50);

                // ===============================
                // Precisión Decimales (Evita warnings y pérdida de precisión)
                // ===============================
                entity.Property(r => r.TasaCambioDia).HasPrecision(18, 4);
                entity.Property(r => r.TotalFacturadoUSD).HasPrecision(18, 2);
                entity.Property(r => r.MontoVueltoUSD).HasPrecision(18, 2);

                // ===============================
                // Configuración de Guids / MySQL
                // ===============================
                entity.Property(r => r.UsuarioEmisionId).HasColumnType("char(36)");
                entity.HasIndex(r => r.UsuarioEmisionId);
                entity.HasIndex(r => r.PacienteId);
                entity.HasIndex(r => r.NumeroRecibo).IsUnique();

                // ===============================
                // Relaciones (Foreign Keys)
                // ===============================
                entity.HasOne(r => r.CajaDiaria)
                    .WithMany()
                    .HasForeignKey(r => r.CajaDiariaId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasIndex(r => r.CajaDiariaId);

                entity.HasOne(r => r.CuentaServicio)
                    .WithMany()
                    .HasForeignKey(r => r.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(r => r.CuentaServicioId);

                entity.HasOne(r => r.EstadoFiscalNav)
                    .WithMany()
                    .HasForeignKey(r => r.EstadoFiscalId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(r => r.EstadoFiscalId);

                // =========================================================================
                // ✅ CRÍTICO: Mapeo de la colección DetallesPago con su campo privado (_detallesPago)
                // =========================================================================
                entity.HasMany(r => r.DetallesPago)
                    .WithOne(d => d.ReciboFactura) // o .WithOne() si DetallePago no tiene navegación inversa
                    .HasForeignKey(d => d.ReciboFacturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Le indica a EF Core que acceda a través del campo privado backing-field _detallesPago
                entity.Navigation(r => r.DetallesPago)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            builder.Entity<EstadoFiscal>(entity =>
            {
                entity.ToTable("EstadosFiscales");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Codigo).IsUnique();

                entity.HasData(
                    new EstadoFiscal(EstadoFiscalConstants.BorradorId, "BORRADOR", "Borrador"),
                    new EstadoFiscal(EstadoFiscalConstants.EmitidaId, "EMITIDA", "Emitida"),
                    new EstadoFiscal(EstadoFiscalConstants.AnuladaId, "ANULADA", "Anulada")
                );
            });

            builder.Entity<DetallePago>(entity =>
             {
                 entity.ToTable("DetallesPago");
                 entity.HasKey(d => d.Id);

                 // ==========================================
                 // Propiedades de Texto y Longitudes
                 // ==========================================
                 entity.Property(d => d.ReferenciaBancaria)
                     .HasMaxLength(100);
                 // ==========================================
                 // Precisión de Decimales
                 // ==========================================
                 entity.Property(d => d.MontoAbonadoMoneda).HasPrecision(18, 2);
                 entity.Property(d => d.EquivalenteAbonadoBase).HasPrecision(18, 2);
                 entity.Property(d => d.TasaCambioAplicada).HasPrecision(18, 4);

                 // ==========================================
                 // Relación con ReciboFactura (Muchos a 1)
                 // ==========================================
                 entity.HasOne(d => d.ReciboFactura)
                     .WithMany(r => r.DetallesPago)
                     .HasForeignKey(d => d.ReciboFacturaId)
                     .OnDelete(DeleteBehavior.Cascade);

                 // ==========================================
                 // Relación con CatalogoMetodoPago (Muchos a 1)
                 // ==========================================
                 entity.HasOne(d => d.MetodoPagoNav)
                     .WithMany()
                     .HasForeignKey(d => d.MetodoPagoId)
                     .OnDelete(DeleteBehavior.Restrict);

                 // ==========================================
                 // Configuración de Guids / MySQL e Índices
                 // ==========================================
                 entity.Property(d => d.UsuarioCargaId).HasColumnType("char(36)");

                 entity.HasIndex(d => d.ReciboFacturaId);
                 entity.HasIndex(d => d.MetodoPagoId);
                 entity.HasIndex(d => d.UsuarioCargaId);
                 entity.HasIndex(d => d.FechaPago);
             });

            builder.Entity<SeguroConvenio>(entity =>
            {
                entity.ToTable("SegurosConvenios");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Rtn).HasMaxLength(50);
                entity.Property(s => s.Direccion).HasMaxLength(500);
                entity.Property(s => s.Telefono).HasMaxLength(50);
                entity.Property(s => s.Email).HasMaxLength(150);
            });

            builder.Entity<HonorarioConfig>(entity =>
             {
                 entity.ToTable("HonorariosConfig");
                 entity.HasKey(h => h.Id);

                 // ==========================================
                 // Propiedades de Texto y Restricciones
                 // ==========================================
                 entity.Property(h => h.CategoriaServicio)
                     .IsRequired()
                     .HasMaxLength(50);

                 entity.Property(h => h.NotasConfig)
                     .HasMaxLength(500);

                 // ==========================================
                 // Relación con Medico (Muchos a 1 Opcional)
                 // ==========================================
                 entity.HasOne(h => h.MedicoDefault)
                     .WithMany()
                     .HasForeignKey(h => h.MedicoDefaultId)
                     .OnDelete(DeleteBehavior.SetNull);

                 // ==========================================
                 // Configuración de Guids / MySQL e Índices
                 // ==========================================
                 entity.Property(h => h.UsuarioConfiguroId)
                     .HasColumnType("char(36)");

                 // Índice único para evitar categorías duplicadas
                 entity.HasIndex(h => h.CategoriaServicio)
                     .IsUnique();

                 // Índices para optimizar búsquedas
                 entity.HasIndex(h => h.MedicoDefaultId);
                 entity.HasIndex(h => h.UsuarioConfiguroId);
             });

            builder.Entity<LogAsignacionHonorario>(entity =>
            {
                entity.ToTable("LogsAsignacionHonorario");
                entity.HasKey(l => l.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(l => l.TipoAccion)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(l => l.NombreServicio)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(l => l.MedicoAnteriorNombre)
                    .HasMaxLength(200);

                entity.Property(l => l.MedicoNuevoNombre)
                    .HasMaxLength(200);

                entity.Property(l => l.Observaciones)
                    .HasMaxLength(1000);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // Relación con el detalle del servicio auditado
                entity.HasOne(l => l.DetalleServicio)
                    .WithMany()
                    .HasForeignKey(l => l.DetalleServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Medico Anterior (opcional)
                entity.HasOne(l => l.MedicoAnterior)
                    .WithMany()
                    .HasForeignKey(l => l.MedicoAnteriorId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con Medico Nuevo (opcional)
                entity.HasOne(l => l.MedicoNuevo)
                    .WithMany()
                    .HasForeignKey(l => l.MedicoNuevoId)
                    .OnDelete(DeleteBehavior.SetNull);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(l => l.UsuarioOperadorId)
                    .HasColumnType("char(36)");

                // Índices para optimizar reportes y trazabilidad
                entity.HasIndex(l => l.DetalleServicioId);
                entity.HasIndex(l => l.MedicoAnteriorId);
                entity.HasIndex(l => l.MedicoNuevoId);
                entity.HasIndex(l => l.UsuarioOperadorId);
                entity.HasIndex(l => l.FechaAccion);
            });
            builder.Entity<DetalleServicioCuenta>(entity =>
            {
                entity.ToTable("DetallesServicioCuenta");
                entity.HasKey(d => d.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(d => d.Descripcion)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(d => d.UsuarioCarga)
                    .HasMaxLength(100);

                entity.Property(d => d.LegacyMappingId)
                    .HasMaxLength(50);

                // ==========================================
                // Precisión de Decimales (Crítico para evitar truncamiento)
                // ==========================================
                entity.Property(d => d.Precio).HasPrecision(18, 2);
                entity.Property(d => d.Honorario).HasPrecision(18, 2); // 👈 Agregado
                entity.Property(d => d.PrecioCatalogoHistorico).HasPrecision(18, 2);
                entity.Property(d => d.Cantidad).HasPrecision(18, 4);

                entity.Property(d => d.IncluidoEnTarifaBase).IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // Relación principal con la Cuenta
                entity.HasOne(d => d.CuentaServicio)
                    .WithMany(c => c.Detalles) // o .WithMany() si CuentaServicios no tiene la lista
                    .HasForeignKey(d => d.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con el Médico Responsable
                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(d => d.MedicoResponsableId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación recursiva (Padre-Hijo: Informe -> Estudio Base)
                entity.HasOne(d => d.DetallePadre)
                    .WithMany()
                    .HasForeignKey(d => d.DetallePadreId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Área Clínica
                entity.HasOne(d => d.AreaClinica)
                    .WithMany()
                    .HasForeignKey(d => d.AreaClinicaId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con Tipo de Servicio
                entity.HasOne(d => d.TipoServicioNav)
                    .WithMany()
                    .HasForeignKey(d => d.TipoServicioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // =========================================================================
                // ✅ CRÍTICO: Mapeo de la colección MedicosResponsables (Evita error EF8)
                // =========================================================================
                entity.HasMany(d => d.MedicosResponsables)
                    .WithOne(m => m.DetalleServicioCuenta) // Ajusta el nombre de navegación inversa en DetalleServicioMedicoResponsable
                    .HasForeignKey("DetalleServicioCuentaId")
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(d => d.UsuarioCargaId).HasColumnType("char(36)");

                entity.HasIndex(d => d.CuentaServicioId);
                entity.HasIndex(d => d.ServicioId);
                entity.HasIndex(d => d.AreaClinicaId);
                entity.HasIndex(d => d.MedicoResponsableId);
                entity.HasIndex(d => d.DetallePadreId);
                entity.HasIndex(d => d.TipoServicioId);
                entity.HasIndex(d => d.UsuarioCargaId);
                entity.HasIndex(d => d.FechaCarga);
            });

            builder.Entity<PacienteAdmision>(entity =>
            {
                entity.ToTable("PacientesAdmision");
                entity.HasKey(p => p.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                // ⚠️ CRÍTICO en MySQL: Columnas con índice UNIQUE deben tener HasMaxLength explícito
                entity.Property(p => p.CedulaPasaporte)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(p => p.NombreCorto)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(p => p.TelefonoContact)
                    .HasMaxLength(30);

                entity.Property(p => p.Direccion)
                    .HasMaxLength(300);

                // ==========================================
                // Índices Únicos y de Búsqueda
                // ==========================================
                // Cédula o pasaporte único
                entity.HasIndex(p => p.CedulaPasaporte)
                    .IsUnique();

                // Enlace con el ID Legacy (permite múltiples NULLs en MySQL)
                entity.HasIndex(p => p.IdPacienteLegacy)
                    .IsUnique();

                // Índice para optimizar búsquedas y autocompletado por nombre
                entity.HasIndex(p => p.NombreCorto);
            });

            builder.Entity<OrdenDeServicio>(entity =>
            {
                entity.ToTable("OrdenesDeServicio");
                entity.HasKey(o => o.Id);

                // ==========================================
                // Discriminador de Herencia (TPH)
                // ==========================================
                entity.HasDiscriminator<string>("Discriminator")
                    .HasValue<OrdenRX>("OrdenRX"); // Si agregas más tipos (ej. OrdenLaboratorio), añádelos aquí

                // Limitamos la columna discriminadora para evitar varchar largo/longtext en MySQL
                entity.Property<string>("Discriminator")
                    .HasMaxLength(50);

                // ==========================================
                // Propiedades y Conversiones
                // ==========================================
                entity.Property(o => o.TipoIngreso)
                    .IsRequired()
                    .HasMaxLength(50);

                // Conversión de Enum a Entero
                entity.Property(o => o.EstadoFacturacion)
                    .HasConversion<int>();

                // ==========================================
                // Relación con PacienteAdmision
                // ==========================================
                entity.HasOne(o => o.Paciente)
                    .WithMany() // o .WithMany(p => p.Ordenes) si PacienteAdmision tiene la colección
                    .HasForeignKey(o => o.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento
                // ==========================================
                entity.HasIndex(o => o.PacienteId);
                entity.HasIndex(o => o.FechaCreacion);
                entity.HasIndex(o => o.ConvenioId);

                // Índice compuesto para buscar el número de llegada por fecha rápidamente
                entity.HasIndex(o => new { o.FechaCreacion, o.NumeroLlegadaDiario });
            });

            builder.Entity<OrdenRX>(entity =>
            {
                // ==========================================
                // Jerarquía de Herencia (TPH)
                // ==========================================
                entity.HasBaseType<OrdenDeServicio>();

                // ==========================================
                // Propiedades Específicas de RX
                // ==========================================
                entity.Property(rx => rx.EstudioSolicitado)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(rx => rx.Procesada)
                    .IsRequired();

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(rx => rx.AsistenteRxId)
                    .HasColumnType("char(36)");

                // Índices para optimizar la cola de trabajo de Rayos X
                entity.HasIndex(rx => rx.AsistenteRxId);
                entity.HasIndex(rx => rx.Procesada);
                entity.HasIndex(rx => rx.FechaProcesada);
            });

            builder.Entity<TurnoMedico>(entity =>
            {
                entity.ToTable("TurnosMedicos");
                entity.HasKey(t => t.Id);

                // ==========================================
                // Propiedades
                // ==========================================
                entity.Property(t => t.FechaHoraToma)
                    .IsRequired();

                entity.Property(t => t.IgnorandoIncidencia)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // Relación con el Médico
                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(t => t.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con el Paciente
                entity.HasOne<PacienteAdmision>()
                    .WithMany()
                    .HasForeignKey(t => t.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento y Agenda
                // ==========================================
                entity.HasIndex(t => t.MedicoId);
                entity.HasIndex(t => t.PacienteId);
                entity.HasIndex(t => t.FechaHoraToma);
                entity.HasIndex(t => t.IncidenciaIgnoradaId);

                // ⚡ Índice compuesto clave para consultar disponibilidad y agenda del médico por fecha
                entity.HasIndex(t => new { t.MedicoId, t.FechaHoraToma });
            });
            builder.Entity<IncidenciaHorario>(entity =>
            {
                entity.ToTable("IncidenciasHorario");
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Tipo).HasConversion<int>();
                entity.Property(i => i.Descripcion).IsRequired().HasMaxLength(500);
                entity.Property(i => i.Inicio).IsRequired();
                entity.Property(i => i.Fin).IsRequired();
                entity.Property(i => i.FechaCreacion).IsRequired();

                // Relación con Medico (Ambos del Domain)
                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(i => i.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ✅ Relación FK con la tabla de Identity sin contaminar el Domain
                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(i => i.CreadoPor)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(i => i.CreadoPor).HasColumnType("char(36)");

                entity.HasIndex(i => i.CreadoPor);
                entity.HasIndex(i => i.MedicoId);
                entity.HasIndex(i => new { i.MedicoId, i.Inicio, i.Fin });
            });

            builder.Entity<RegistroAuditoriaIncidencia>(entity =>
            {
                entity.ToTable("RegistroAuditoriaIncidencias");
                entity.HasKey(r => r.Id);

                // ==========================================
                // Propiedades de Texto y Fechas
                // ==========================================
                entity.Property(r => r.Motivo)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(r => r.FechaTraza)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys en Infraestructura)
                // ==========================================
                // 1. Relación con el Turno Médico auditado
                entity.HasOne<TurnoMedico>()
                    .WithMany()
                    .HasForeignKey(r => r.TurnoMedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con la Incidencia de Horario que fue ignorada
                entity.HasOne<IncidenciaHorario>()
                    .WithMany()
                    .HasForeignKey(r => r.IncidenciaIgnoradaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con el Usuario (Operador que forzó el agendamiento)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(r => r.OperadorId)
                    .OnDelete(DeleteBehavior.Restrict); // Protege la auditoría: no se puede borrar el usuario

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(r => r.OperadorId)
                    .HasColumnType("char(36)");

                // Índices para reportes de auditoría y trazabilidad
                entity.HasIndex(r => r.TurnoMedicoId);
                entity.HasIndex(r => r.IncidenciaIgnoradaId);
                entity.HasIndex(r => r.OperadorId);
                entity.HasIndex(r => r.FechaTraza);
            });

            builder.Entity<CuentaServicios>(entity =>
            {
                entity.ToTable("CuentasServicios");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(c => c.SubAreaClinica)
                    .HasMaxLength(100);

                entity.Property(c => c.ProcesamientoEstado)
                    .HasMaxLength(50);

                entity.Property(c => c.DestinoPaciente)
                    .HasMaxLength(200);

                entity.Property(c => c.PersonalRelevo)
                    .HasMaxLength(150);

                // ==========================================
                // Relaciones Principales (Foreign Keys)
                // ==========================================
                // 1. Relación con Paciente
                entity.HasOne(c => c.Paciente)
                    .WithMany()
                    .HasForeignKey(c => c.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. Relación con Convenio / Seguro (Faltaba en la config original)
                entity.HasOne(c => c.Convenio)
                    .WithMany()
                    .HasForeignKey(c => c.ConvenioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación Recursiva (Sub-cuentas / Cuenta Principal)
                entity.HasOne(c => c.CuentaPrincipal)
                    .WithMany()
                    .HasForeignKey(c => c.CuentaPrincipalId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 4. Relaciones con Ubicaciones y Médico
                entity.HasOne(c => c.AreaClinica)
                    .WithMany()
                    .HasForeignKey(c => c.AreaClinicaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(c => c.CamaRetenida)
                    .WithMany()
                    .HasForeignKey(c => c.CamaRetenidaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(c => c.Medico)
                    .WithMany()
                    .HasForeignKey(c => c.MedicoId)
                    .OnDelete(DeleteBehavior.SetNull);

                // 5. Catálogos de Estado y Tipo de Ingreso
                entity.HasOne(c => c.EstadoNav)
                    .WithMany()
                    .HasForeignKey(c => c.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.TipoIngresoNav)
                    .WithMany()
                    .HasForeignKey(c => c.TipoIngresoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Relaciones con Colecciones Hijas (Cascade Delete)
                // ==========================================
                // Detalles de la cuenta (Con backing-field _detalles)
                entity.HasMany(c => c.Detalles)
                    .WithOne(d => d.CuentaServicio)
                    .HasForeignKey(d => d.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation(c => c.Detalles)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                // Triages de Enfermería
                entity.HasMany(c => c.Triages)
                    .WithOne(t => t.CuentaServicio)
                    .HasForeignKey(t => t.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Valoraciones Físicas
                entity.HasMany(c => c.Valoraciones)
                    .WithOne(v => v.CuentaServicio)
                    .HasForeignKey(v => v.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Claves Foráneas de Auditoría (Identity)
                // ==========================================
                entity.Property(c => c.UsuarioCargaId).HasColumnType("char(36)");
                entity.Property(c => c.UsuarioValidacionId).HasColumnType("char(36)");
                entity.Property(c => c.UsuarioAuditoriaId).HasColumnType("char(36)");

                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioCargaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioValidacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioAuditoriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices para Rendimiento
                // ==========================================
                entity.HasIndex(c => c.PacienteId);
                entity.HasIndex(c => c.ConvenioId);
                entity.HasIndex(c => c.CuentaPrincipalId);
                entity.HasIndex(c => c.AreaClinicaId);
                entity.HasIndex(c => c.CamaRetenidaId);
                entity.HasIndex(c => c.MedicoId);
                entity.HasIndex(c => c.EstadoId);
                entity.HasIndex(c => c.TipoIngresoId);
                entity.HasIndex(c => c.LegacyOrderId);
                entity.HasIndex(c => c.FechaCarga);
                entity.HasIndex(c => c.FechaCierre);
                entity.HasIndex(c => c.UsuarioCargaId);
                entity.HasIndex(c => c.UsuarioValidacionId);
                entity.HasIndex(c => c.UsuarioAuditoriaId);
            });
            builder.Entity<EstadoCuenta>(entity =>
            {
                entity.ToTable("EstadosCuenta");
                entity.HasKey(e => e.Id);

                // Identificador manual (catálogo fijo con constantes)
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                // ==========================================
                // Propiedades y Restricciones
                // ==========================================
                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Índice único para el código de catálogo
                entity.HasIndex(e => e.Codigo)
                    .IsUnique();

                // ==========================================
                // Semilla de Datos (Seed Data)
                // ==========================================
                entity.HasData(
                    new EstadoCuenta(EstadoCuentaConstants.AbiertaId, "ABIERTA", "Abierta", activo: true),
                    new EstadoCuenta(EstadoCuentaConstants.FacturadaId, "FACTURADA", "Facturada", activo: true),
                    new EstadoCuenta(EstadoCuentaConstants.AnuladaId, "ANULADA", "Anulada", activo: true),
                    new EstadoCuenta(EstadoCuentaConstants.ValidadaId, "VALIDADA", "Validada", activo: true)
                );
            });

            builder.Entity<TipoIngreso>(entity =>
            {
                entity.ToTable("TiposIngreso");
                entity.HasKey(t => t.Id);

                // Identificador manual (catálogo fijo con constantes)
                entity.Property(t => t.Id)
                    .ValueGeneratedNever();

                // ==========================================
                // Propiedades y Restricciones
                // ==========================================
                entity.Property(t => t.Codigo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(t => t.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(t => t.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Índice único para el código
                entity.HasIndex(t => t.Codigo)
                    .IsUnique();

                // ==========================================
                // Semilla de Datos (Seed Data)
                // ==========================================
                entity.HasData(
                    new TipoIngreso(TipoIngresoConstants.ParticularId, "PARTICULAR", "Particular", activo: true),
                    new TipoIngreso(TipoIngresoConstants.SeguroId, "SEGURO", "Seguro", activo: true),
                    new TipoIngreso(TipoIngresoConstants.HospitalizacionId, "HOSPITALIZACION", "Hospitalización", activo: true),
                    new TipoIngreso(TipoIngresoConstants.EmergenciaId, "EMERGENCIA", "Emergencia", activo: true),
                    new TipoIngreso(TipoIngresoConstants.UciId, "UCI", "UCI", activo: true)
                );
            });
            
           
            builder.Entity<CitaMedica>(entity =>
            {
                entity.ToTable("CitasMedicas");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Propiedades de Texto y Fechas
                // ==========================================
                entity.Property(c => c.Comentario)
                    .HasMaxLength(500);

                entity.Property(c => c.HoraPautada)
                    .IsRequired();

                entity.Property(c => c.FechaRegistro)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Médico (Faltaba en la config original)
                entity.HasOne(c => c.Medico)
                    .WithMany()
                    .HasForeignKey(c => c.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. Relación con Paciente
                entity.HasOne<PacienteAdmision>()
                    .WithMany()
                    .HasForeignKey(c => c.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con la Cuenta de Servicio
                entity.HasOne(c => c.CuentaServicio)
                    .WithMany()
                    .HasForeignKey(c => c.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 4. Relación con Área Clínica / Consultorio
                entity.HasOne(c => c.AreaClinica)
                    .WithMany()
                    .HasForeignKey(c => c.AreaClinicaId)
                    .OnDelete(DeleteBehavior.SetNull);

                // 5. Relación con el Catálogo de Estados de Cita
                entity.HasOne(c => c.EstadoNav)
                    .WithMany()
                    .HasForeignKey(c => c.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento y Agenda
                // ==========================================
                entity.HasIndex(c => c.MedicoId);
                entity.HasIndex(c => c.PacienteId);
                entity.HasIndex(c => c.CuentaServicioId);
                entity.HasIndex(c => c.AreaClinicaId);
                entity.HasIndex(c => c.EstadoId);
                entity.HasIndex(c => c.HoraPautada);
                entity.HasIndex(c => c.FechaRegistro);

                // ⚡ Índice compuesto clave para consultar la agenda de citas del médico por horario
                entity.HasIndex(c => new { c.MedicoId, c.HoraPautada });
            });

            builder.Entity<EstadoCitaMedica>(entity =>
            {
                entity.ToTable("EstadosCitaMedica");
                entity.HasKey(e => e.Id);

                // Identificador manual (catálogo fijo con constantes)
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                // ==========================================
                // Propiedades y Restricciones
                // ==========================================
                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Índice único para el código
                entity.HasIndex(e => e.Codigo)
                    .IsUnique();

                // ==========================================
                // Semilla de Datos (Seed Data)
                // ==========================================
                entity.HasData(
                    new EstadoCitaMedica(EstadoCitaConstants.PendienteId, "PENDIENTE", "Pendiente", activo: true),
                    new EstadoCitaMedica(EstadoCitaConstants.ConfirmadaId, "CONFIRMADA", "Confirmada", activo: true),
                    new EstadoCitaMedica(EstadoCitaConstants.AtendidaId, "ATENDIDA", "Atendida", activo: true),
                    new EstadoCitaMedica(EstadoCitaConstants.CanceladaId, "CANCELADA", "Cancelada", activo: true)
                );
            });
            
            builder.Entity<Medico>(entity =>
            {
                entity.ToTable("Medicos");
                entity.HasKey(m => m.Id);

                // ==========================================
                // Propiedades de Texto y Restricciones
                // ==========================================
                entity.Property(m => m.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(m => m.Telefono)
                    .HasMaxLength(30);

                // ==========================================
                // Valores Numéricos, Precisión y Defaults
                // ==========================================
                // ⚠️ Crítico: Precisión decimal para evitar pérdidas en honorarios
                entity.Property(m => m.HonorarioBase)
                    .HasPrecision(18, 2)
                    .HasDefaultValue(0.00m);

                entity.Property(m => m.IntervaloTurnoMinutos)
                    .IsRequired()
                    .HasDefaultValue(30);

                entity.Property(m => m.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // ==========================================
                // Relación con Especialidad (Muchos a 1)
                // ==========================================
                entity.HasOne(m => m.Especialidad)
                    .WithMany() // o .WithMany(e => e.Medicos) si Especialidad tiene la colección
                    .HasForeignKey(m => m.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict); // No permite borrar especialidad si tiene médicos asignados

                // ==========================================
                // Índices de Rendimiento
                // ==========================================
                entity.HasIndex(m => m.EspecialidadId);
                entity.HasIndex(m => m.Nombre); // Acelera búsquedas por nombre del médico
                entity.HasIndex(m => m.Activo); // Acelera filtros de médicos activos en selectores
            });
            builder.Entity<ServicioClinico>(entity =>
            {
                entity.ToTable("ServiciosClinicos");
                entity.HasKey(s => s.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(s => s.Codigo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.Descripcion)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(s => s.TipoServicio)
                    .HasMaxLength(50);

                entity.Property(s => s.LegacyMappingId)
                    .HasMaxLength(50);

                entity.Property(s => s.HonorariumCategory)
                    .HasMaxLength(50);

                entity.Property(s => s.UnidadMedida)
                    .HasMaxLength(50);

                entity.Property(s => s.DesactivadoPorUsuarioId)
                    .HasMaxLength(100);

                // ==========================================
                // Precisión de Decimales (Crítico)
                // ==========================================
                entity.Property(s => s.PrecioBase)
                    .HasPrecision(18, 2);

                entity.Property(s => s.HonorarioBase)
                    .HasPrecision(18, 2); // 👈 Agregado para evitar advertencias de EF

                // ==========================================
                // Valores por Defecto y Boleanos
                // ==========================================
                entity.Property(s => s.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(s => s.RequiereInventario)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(s => s.EsServicioInforme)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(s => s.PermiteFraccionamiento)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Conversión de Enum Categoría (Entero o String según tu estándar)
                entity.Property(s => s.Category)
                    .HasConversion<int>(); // o .HasConversion<string>().HasMaxLength(50);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Tipo de Servicio
                entity.HasOne<TipoServicio>()
                    .WithMany()
                    .HasForeignKey(s => s.TipoServicioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. Relación con Especialidad Médica (Opcional)
                entity.HasOne(s => s.Especialidad)
                    .WithMany()
                    .HasForeignKey(s => s.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación Recursiva con Servicio de Informe (Opcional)
                entity.HasOne(s => s.ServicioInforme)
                    .WithMany()
                    .HasForeignKey(s => s.ServicioInformeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices para Rendimiento y Búsquedas
                // ==========================================
                entity.HasIndex(s => s.Codigo)
                    .IsUnique(); // Código de servicio único

                entity.HasIndex(s => s.Descripcion); // Acelera el autocompletado en facturación y cargos
                entity.HasIndex(s => s.TipoServicioId);
                entity.HasIndex(s => s.EspecialidadId);
                entity.HasIndex(s => s.ServicioInformeId);
                entity.HasIndex(s => s.Activo);
            });

            builder.Entity<TriageEnfermeria>(entity =>
            {
                entity.ToTable("TriagesEnfermeria");
                entity.HasKey(t => t.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(t => t.TensionArterial)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(t => t.MotivoConsulta)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(t => t.DescripcionRapida)
                    .HasMaxLength(250);

                entity.Property(t => t.DescripcionDetallada)
                    .HasMaxLength(2000);

                // ==========================================
                // Signos Vitales y Precisión
                // ==========================================
                entity.Property(t => t.Temperatura)
                    .HasPrecision(4, 2); // Permite valores como 36.50, 39.80 °C

                entity.Property(t => t.FechaRegistro)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con la Cuenta de Servicios
                entity.HasOne<CuentaServicios>()
                    .WithMany(c => c.Triages)
                    .HasForeignKey(t => t.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación de Auditoría con Usuario (Enfermero/a que registró)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(t => t.UsuarioRegistroId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(t => t.UsuarioRegistroId)
                    .HasColumnType("char(36)");

                // Índices para optimizar consultas de historia clínica y reportes
                entity.HasIndex(t => t.CuentaServicioId);
                entity.HasIndex(t => t.UsuarioRegistroId);
                entity.HasIndex(t => t.FechaRegistro);
            });
           
            builder.Entity<ValoracionFisica>(entity =>
            {
                entity.ToTable("ValoracionesFisicas");
                entity.HasKey(v => v.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(v => v.EstadoConciencia).HasMaxLength(50).IsRequired();
                entity.Property(v => v.ViaAerea).HasMaxLength(50).IsRequired();
                entity.Property(v => v.Ventilacion).HasMaxLength(50).IsRequired();
                entity.Property(v => v.Pulso).HasMaxLength(50).IsRequired();
                entity.Property(v => v.PielMucosas).HasMaxLength(50).IsRequired();
                entity.Property(v => v.LlenadoCapilar).HasMaxLength(50).IsRequired();
                entity.Property(v => v.Pupilas).HasMaxLength(50).IsRequired();

                entity.Property(v => v.Alergias).HasMaxLength(500);
                entity.Property(v => v.AccesosVenosos).HasMaxLength(500);
                entity.Property(v => v.Pertenencias).HasMaxLength(500);
                entity.Property(v => v.AntecedentesMedicos).HasMaxLength(1000);

                entity.Property(v => v.UsuarioRegistro).HasMaxLength(100);
                entity.Property(v => v.FechaRegistro).IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con la Cuenta de Servicio
                entity.HasOne(v => v.CuentaServicio)
                    .WithMany(c => c.Valoraciones)
                    .HasForeignKey(v => v.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación de Auditoría con Usuario
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(v => v.UsuarioRegistroId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(v => v.UsuarioRegistroId).HasColumnType("char(36)");

                entity.HasIndex(v => v.CuentaServicioId);
                entity.HasIndex(v => v.UsuarioRegistroId);
                entity.HasIndex(v => v.FechaRegistro);
            });

           builder.Entity<HorarioAtencionMedico>(entity =>
            {
                entity.ToTable("HorariosAtencionMedicos");
                entity.HasKey(h => h.Id);

                // ==========================================
                // Propiedades y Tipos
                // ==========================================
                entity.Property(h => h.DiaSemana)
                    .IsRequired();

                // En MySQL, TimeSpan mapea de forma nativa a columnas tipo TIME
                entity.Property(h => h.HoraInicio)
                    .IsRequired();

                entity.Property(h => h.HoraFin)
                    .IsRequired();

                // ==========================================
                // Relación con Médico (Cascade)
                // ==========================================
                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(h => h.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade); // Si se elimina el médico, se borran sus horarios

                // ==========================================
                // Índices de Rendimiento para Agenda
                // ==========================================
                entity.HasIndex(h => h.MedicoId);

                // ⚡ Índice compuesto clave: Buscar los horarios de atención de un médico en un día específico (ej. lunes = 1)
                entity.HasIndex(h => new { h.MedicoId, h.DiaSemana });
            });

            builder.Entity<PrecioServicioConvenio>(entity =>
            {
                entity.ToTable("PreciosServicioConvenio");
                entity.HasKey(p => p.Id);

                // ==========================================
                // Precisión Decimal
                // ==========================================
                entity.Property(p => p.PrecioDiferencial)
                    .HasPrecision(18, 2)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // Relación con el Servicio Clínico
                entity.HasOne(p => p.Servicio)
                    .WithMany()
                    .HasForeignKey(p => p.ServicioClinicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con el Convenio / Seguro
                entity.HasOne(p => p.Convenio)
                    .WithMany()
                    .HasForeignKey(p => p.SeguroConvenioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(p => p.ServicioClinicoId);
                entity.HasIndex(p => p.SeguroConvenioId);

                // ⚡ CRÍTICO: Índice único compuesto para evitar tarifas duplicadas del mismo servicio en el mismo convenio
                entity.HasIndex(p => new { p.ServicioClinicoId, p.SeguroConvenioId })
                    .IsUnique();
            });

           builder.Entity<ConvenioPerfilPrecio>(entity =>
            {
                entity.ToTable("ConvenioPerfilPrecios");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Precisión de Precios en Monedas (HNL y USD)
                // ==========================================
                entity.Property(c => c.PrecioHNL)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(c => c.PrecioUSD)
                    .HasPrecision(18, 2)
                    .IsRequired();

                // ==========================================
                // Estados y Fechas
                // ==========================================
                entity.Property(c => c.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(c => c.UltimaActualizacion)
                    .IsRequired();

                // ==========================================
                // Relación con SeguroConvenio (Cascade Delete)
                // ==========================================
                entity.HasOne(c => c.Convenio)
                    .WithMany()
                    .HasForeignKey(c => c.SeguroConvenioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(c => c.SeguroConvenioId);
                entity.HasIndex(c => c.PerfilId); // Acelera búsquedas de precios por perfil de laboratorio
                entity.HasIndex(c => c.Activo);

                // ⚡ Índice único compuesto: Evita duplicar el precio de un mismo perfil en el mismo convenio
                entity.HasIndex(c => new { c.SeguroConvenioId, c.PerfilId })
                    .IsUnique();
            });

            builder.Entity<CuentaPorCobrar>(entity =>
            {
                entity.ToTable("CuentasPorCobrar");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Precisión de Decimales
                // ==========================================
                entity.Property(c => c.MontoTotalBase)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(c => c.MontoPagadoBase)
                    .HasPrecision(18, 2)
                    .IsRequired();

                // Ignorar propiedad calculada
                entity.Ignore(c => c.SaldoPendienteBase);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(c => c.Estado)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.UsuarioAuditoria)
                    .HasMaxLength(100);

                entity.Property(c => c.QuienAutorizo)
                    .HasMaxLength(150);

                entity.Property(c => c.DoctorProcedimiento)
                    .HasMaxLength(150);

                entity.Property(c => c.InformacionAdicional)
                    .HasMaxLength(1000);

                entity.Property(c => c.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con la Cuenta de Servicios
                entity.HasOne(c => c.Cuenta)
                    .WithMany()
                    .HasForeignKey(c => c.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Paciente
                entity.HasOne<PacienteAdmision>()
                    .WithMany()
                    .HasForeignKey(c => c.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con la colección de ítems de garantía (Cascade)
                entity.HasMany(c => c.GarantiasItems)
                    .WithOne(g => g.CuentaPorCobrar)
                    .HasForeignKey(g => g.CuentaPorCobrarId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 4. Relación de Auditoría con Usuario
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioAuditoriaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(c => c.UsuarioAuditoriaId).HasColumnType("char(36)");

                entity.HasIndex(c => c.CuentaServicioId);
                entity.HasIndex(c => c.PacienteId);
                entity.HasIndex(c => c.UsuarioAuditoriaId);
                entity.HasIndex(c => c.Estado);
                entity.HasIndex(c => c.FechaCreacion);
            });
            
           builder.Entity<GarantiaItem>(entity =>
            {
                entity.ToTable("GarantiasItems");
                entity.HasKey(g => g.Id);

                // ==========================================
                // Propiedades de Texto y Precisión
                // ==========================================
                entity.Property(g => g.Descripcion)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(g => g.ValorEstimado)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(g => g.FechaRegistro)
                    .IsRequired();

                // ==========================================
                // Relación Inversa con CuentaPorCobrar (Cascade)
                // ==========================================
                entity.HasOne(g => g.CuentaPorCobrar)
                    .WithMany(c => c.GarantiasItems)
                    .HasForeignKey(g => g.CuentaPorCobrarId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Índices
                // ==========================================
                entity.HasIndex(g => g.CuentaPorCobrarId);
                entity.HasIndex(g => g.FechaRegistro);
            });

            builder.Entity<CompromisoPago>(entity =>
            {
                entity.ToTable("CompromisosPago");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Propiedades de Texto y Estados
                // ==========================================
                entity.Property(c => c.Observacion)
                    .HasMaxLength(1000);

                entity.Property(c => c.UsuarioCreacion)
                    .HasMaxLength(100);

                entity.Property(c => c.Omitido)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(c => c.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con CuentaPorCobrar (Cascade)
                entity.HasOne(c => c.CuentaPorCobrar)
                    .WithMany()
                    .HasForeignKey(c => c.CuentaPorCobrarId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Catálogo de Motivo de Autorización
                entity.HasOne(c => c.MotivoAutorizacion)
                    .WithMany()
                    .HasForeignKey(c => c.MotivoAutorizacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación de Auditoría con Usuario
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioCreacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(c => c.UsuarioCreacionId)
                    .HasColumnType("char(36)");

                entity.HasIndex(c => c.CuentaPorCobrarId);
                entity.HasIndex(c => c.MotivoAutorizacionId);
                entity.HasIndex(c => c.UsuarioCreacionId);
                entity.HasIndex(c => c.FechaCreacion);
            });

            builder.Entity<MotivoAutorizacion>(entity =>
            {
                entity.ToTable("MotivosAutorizacion");
                entity.HasKey(m => m.Id);

                // ==========================================
                // Propiedades y Restricciones
                // ==========================================
                entity.Property(m => m.Id)
                    .ValueGeneratedOnAdd(); // Autoincremental para nuevos motivos

                entity.Property(m => m.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(m => m.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Índice único para evitar nombres de motivos duplicados
                entity.HasIndex(m => m.Nombre)
                    .IsUnique();

                // ==========================================
                // Semilla de Datos Inicial (Seed Data)
                // ==========================================
                entity.HasData(
                    new MotivoAutorizacion(1, "Autorizado por Dirección Médica", activo: true),
                    new MotivoAutorizacion(2, "Exoneración por Presidencia", activo: true),
                    new MotivoAutorizacion(3, "Convenio Institucional", activo: true)
                );
            });

            builder.Entity<TasaCambio>(entity =>
            {
                // Nombre de tabla en plural siguiendo la convención del sistema
                entity.ToTable("TasasCambio");
                entity.HasKey(t => t.Id);

                // ==========================================
                // Precisión y Valores por Defecto
                // ==========================================
                // ⚠️ Crítico: 4 decimales para tasas de cambio cambiarias (ej. 36.4520)
                entity.Property(t => t.Monto)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(t => t.Fecha)
                    .IsRequired();

                entity.Property(t => t.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // ==========================================
                // Índices de Rendimiento
                // ==========================================
                entity.HasIndex(t => t.Fecha);
                entity.HasIndex(t => t.Activo);

                // ⚡ Índice compuesto clave: Acelera la consulta de la tasa activa más reciente
                // (Ej: Where(t => t.Activo).OrderByDescending(t => t.Fecha).FirstOrDefault())
                entity.HasIndex(t => new { t.Activo, t.Fecha });
            });
            
            builder.Entity<ErrorTicket>(entity =>
            {
                entity.ToTable("ErrorTickets");
                entity.HasKey(e => e.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(e => e.RequestPath)
                    .HasMaxLength(500);

                entity.Property(e => e.MetodoHTTP)
                    .HasMaxLength(10);

                entity.Property(e => e.MensajeExcepcion)
                    .HasMaxLength(2000);

                entity.Property(e => e.UsuarioAsociado)
                    .HasMaxLength(100);

                entity.Property(e => e.ResueltoPor)
                    .HasMaxLength(100);

                entity.Property(e => e.ComentariosResolucion)
                    .HasMaxLength(1000);

                // ==========================================
                // Estados y Fechas
                // ==========================================
                entity.Property(e => e.Resuelto)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Relaciones con Identity (SetNull para no bloquear tickets)
                // ==========================================
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioAsociadoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(e => e.ResueltoPorId)
                    .OnDelete(DeleteBehavior.SetNull);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(e => e.UsuarioAsociadoId).HasColumnType("char(36)");
                entity.Property(e => e.ResueltoPorId).HasColumnType("char(36)");

                entity.HasIndex(e => e.UsuarioAsociadoId);
                entity.HasIndex(e => e.ResueltoPorId);
                entity.HasIndex(e => e.Resuelto); // Acelera listar tickets pendientes/no resueltos
                entity.HasIndex(e => e.FechaCreacion);
            });

            builder.Entity<Especialidad>(entity =>
            {
                entity.ToTable("Especialidades");
                entity.HasKey(e => e.Id);

                // ==========================================
                // Propiedades y Restricciones
                // ==========================================
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                // ⚡ Índice único para evitar especialidades médicas duplicadas
                entity.HasIndex(e => e.Nombre)
                    .IsUnique();

                // Índice para optimizar dropdowns y filtros de especialidades activas
                entity.HasIndex(e => e.Activo);
            });

            builder.Entity<ReservaTemporal>(entity =>
            {
                entity.ToTable("ReservasTemporales");
                entity.HasKey(r => r.Id);

                // ==========================================
                // Propiedades de Texto y Fechas
                // ==========================================
                entity.Property(r => r.Comentario)
                    .HasMaxLength(500);

                entity.Property(r => r.HoraPautada)
                    .IsRequired();

                entity.Property(r => r.ExpiracionUtc)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // Relación con Médico
                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(r => r.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Usuario (Identity)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(r => r.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(r => r.UsuarioIdentityId)
                    .HasColumnType("char(36)");

                entity.HasIndex(r => r.UsuarioIdentityId);

                // ⚡ Índice para el Background Job que limpia reservas vencidas:
                // (Ej: Where(r => r.ExpiracionUtc < DateTime.UtcNow).ExecuteDeleteAsync())
                entity.HasIndex(r => r.ExpiracionUtc);

                // ⚡ Índice único compuesto: Bloquea el cupo para que dos recepcionistas no reserven la misma hora
                entity.HasIndex(r => new { r.MedicoId, r.HoraPautada })
                    .IsUnique();
            });
            
            builder.Entity<BloqueoHorario>(entity =>
            {
                entity.ToTable("BloqueosHorarios");
                entity.HasKey(b => b.Id);

                // ==========================================
                // Propiedades de Texto y Fechas
                // ==========================================
                entity.Property(b => b.Motivo)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(b => b.HoraPautada)
                    .IsRequired();

                entity.Property(b => b.FechaRegistro)
                    .IsRequired();

                // ==========================================
                // Relación con Médico (Cascade)
                // ==========================================
                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(b => b.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade); // Si se elimina el médico, se limpian sus bloqueos

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(b => b.MedicoId);
                entity.HasIndex(b => b.FechaRegistro);

                // ⚡ Índice único compuesto: Evita crear bloqueos duplicados en el mismo slot de horario del médico
                entity.HasIndex(b => new { b.MedicoId, b.HoraPautada })
                    .IsUnique();
            });

           builder.Entity<ConfiguracionGeneral>(entity =>
            {
                entity.ToTable("ConfiguracionGeneral");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(c => c.NombreEmpresa)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.Rif)
                    .IsRequired()
                    .HasMaxLength(30);

                // Espacio suficiente para almacenar la clave o su HASH seguro
                entity.Property(c => c.ClaveSupervisor)
                    .IsRequired()
                    .HasMaxLength(255);

                // Tipo LONGTEXT en MySQL para soportar imágenes en Base64
                entity.Property(c => c.LogoBase64)
                    .HasColumnType("longtext");

                // ==========================================
                // Precisión y Valores Numéricos
                // ==========================================
                // Permite porcentajes como 16.00% o 8.00%
                entity.Property(c => c.Iva)
                    .HasPrecision(5, 2)
                    .IsRequired();

                // ==========================================
                // Boleanos y Fechas
                // ==========================================
                entity.Property(c => c.FacturarLaboratorio)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(c => c.MostrarDetalleFacturacion)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(c => c.UltimaActualizacion)
                    .IsRequired();
            });

           builder.Entity<ServicioSugerencia>(entity =>
            {
                // Convención estándar en PascalCase (coherente con las demás tablas)
                entity.ToTable("ServiciosSugerencias");
                entity.HasKey(s => s.Id);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Servicio Origen (Padre -> Cascade Delete)
                entity.HasOne(s => s.ServicioOrigen)
                    .WithMany(sc => sc.Sugerencias) // o .WithMany() si ServicioClinico no tiene la colección
                    .HasForeignKey(s => s.ServicioOrigenId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Servicio Sugerido (Hijo -> Restrict para no borrar en cascada otros servicios)
                entity.HasOne(s => s.ServicioSugerido)
                    .WithMany()
                    .HasForeignKey(s => s.ServicioSugeridoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(s => s.ServicioOrigenId);
                entity.HasIndex(s => s.ServicioSugeridoId);

                // ⚡ CRÍTICO: Índice único compuesto para evitar sugerir el mismo servicio dos veces
                entity.HasIndex(s => new { s.ServicioOrigenId, s.ServicioSugeridoId })
                    .IsUnique();
            });


           builder.Entity<LogAuditoriaPrecio>(entity =>
            {
                entity.ToTable("AuditLogsPrecios");
                entity.HasKey(a => a.Id);

                // ==========================================
                // Precisión de Decimales (Precios y Honorarios)
                // ==========================================
                entity.Property(a => a.PrecioOriginal)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(a => a.PrecioModificado)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(a => a.HonorarioAnterior)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(a => a.NuevoHonorario)
                    .HasPrecision(18, 2)
                    .IsRequired();

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(a => a.DescripcionServicio)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(a => a.UsuarioOperador)
                    .HasMaxLength(100);

                entity.Property(a => a.AutorizadoPor)
                    .HasMaxLength(100);

                entity.Property(a => a.FechaModificacion)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con el Detalle del Servicio
                entity.HasOne(a => a.DetalleServicio)
                    .WithMany()
                    .HasForeignKey(a => a.DetalleServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación de Auditoría con Operador
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(a => a.UsuarioOperadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación de Auditoría con Autorizador (Supervisor)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(a => a.AutorizadoPorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(a => a.UsuarioOperadorId).HasColumnType("char(36)");
                entity.Property(a => a.AutorizadoPorId).HasColumnType("char(36)");

                entity.HasIndex(a => a.DetalleServicioId);
                entity.HasIndex(a => a.UsuarioOperadorId);
                entity.HasIndex(a => a.AutorizadoPorId);
                entity.HasIndex(a => a.FechaModificacion);
            });

            builder.Entity<OrdenImagen>(entity =>
            {
                entity.ToTable("OrdenesImagenes");
                entity.HasKey(o => o.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(o => o.Estudio)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(o => o.TipoServicio)
                    .IsRequired()
                    .HasMaxLength(50); // RX, TOMO, ECO

                entity.Property(o => o.ProcesadoPor)
                    .HasMaxLength(100);

                entity.Property(o => o.ValidadorPor)
                    .HasMaxLength(100);

                entity.Property(o => o.LinkInforme)
                    .HasMaxLength(500);

                entity.Property(o => o.ObservacionesMedico)
                    .HasMaxLength(1000);

                entity.Property(o => o.Informe)
                    .HasColumnType("text"); // Soporta reportes e informes radiológicos extensos

                // ==========================================
                // Conversiones y Estados Boleanos
                // ==========================================
                entity.Property(o => o.Estado)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(o => o.EsDirecta)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(o => o.RequiereValidacion)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(o => o.Validada)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(o => o.RequiereInforme)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(o => o.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Cuenta de Servicios
                entity.HasOne<CuentaServicios>()
                    .WithMany()
                    .HasForeignKey(o => o.CuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Paciente (Restrict para proteger historial médico)
                entity.HasOne(o => o.Paciente)
                    .WithMany()
                    .HasForeignKey(o => o.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con Médico Solicitante
                entity.HasOne(o => o.MedicoSolicitante)
                    .WithMany()
                    .HasForeignKey(o => o.MedicoSolicitanteId)
                    .OnDelete(DeleteBehavior.SetNull);

                // 4. Relación con Médico Radiólogo / Intérprete (Faltaba en la config original)
                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(o => o.MedicoInterpreteId)
                    .OnDelete(DeleteBehavior.SetNull);

                // ==========================================
                // Índices para Cola de Trabajo en Tiempo Real
                // ==========================================
                entity.HasIndex(o => o.CuentaId);
                entity.HasIndex(o => o.PacienteId);
                entity.HasIndex(o => o.MedicoSolicitanteId);
                entity.HasIndex(o => o.MedicoInterpreteId);
                entity.HasIndex(o => o.FechaCreacion);

                // ⚡ Índice compuesto clave: Acelera las pantallas de trabajo de los técnicos (Ej: RX Pendientes / TOMO Pendientes)
                entity.HasIndex(o => new { o.TipoServicio, o.Estado });
            });

           builder.Entity<Moneda>(entity =>
            {
                entity.ToTable("Monedas");
                entity.HasKey(m => m.Id);

                // Identificador manual (catálogo fijo con constantes)
                entity.Property(m => m.Id)
                    .ValueGeneratedNever();

                // ==========================================
                // Propiedades y Restricciones
                // ==========================================
                entity.Property(m => m.Codigo)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(m => m.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(m => m.Simbolo)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(m => m.EsBaseUsd)
                    .IsRequired()
                    .HasDefaultValue(false);

                // ==========================================
                // Índices y Unicidad
                // ==========================================
                // ⚡ Índice único para evitar códigos de moneda duplicados (ej. no tener dos 'USD')
                entity.HasIndex(m => m.Codigo)
                    .IsUnique();

                // ==========================================
                // Semilla de Datos Inicial (Seed Data)
                // ==========================================
                entity.HasData(
                    new Moneda(1, "USD", "Dólar", "$", esBaseUsd: true),
                    new Moneda(2, "VES", "Bolívar", "Bs.", esBaseUsd: false),
                    new Moneda(3, "EUR", "Euro", "€", esBaseUsd: false),
                    new Moneda(4, "COP", "Peso Colombiano", "COP$", esBaseUsd: false),
                    new Moneda(5, "ARS", "Peso Argentino", "ARS$", esBaseUsd: false)
                );
            });

            builder.Entity<CatalogoMetodoPago>(entity =>
            {
                entity.ToTable("CatalogoMetodosPago");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Valor)
                    .IsRequired()
                    .HasMaxLength(100);

                // ==========================================
                // Boleanos, Valores por Defecto y Orden
                // ==========================================
                entity.Property(c => c.GrupoMoneda)
                    .IsRequired()
                    .HasDefaultValue(1); // 1 = USD por defecto

                entity.Property(c => c.EsUSD)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(c => c.EsVuelto)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(c => c.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(c => c.Orden)
                    .IsRequired()
                    .HasDefaultValue(0);

                // ==========================================
                // Relación con Moneda (Muchos a 1)
                // ==========================================
                entity.HasOne(c => c.Moneda)
                    .WithMany()
                    .HasForeignKey(c => c.GrupoMoneda)
                    .OnDelete(DeleteBehavior.Restrict); // No permite eliminar la moneda si tiene métodos de pago asociados

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                // ⚡ Índice único para el valor del método de pago (ej. no duplicar 'Zelle' o 'PagoMovil')
                entity.HasIndex(c => c.Valor)
                    .IsUnique();

                entity.HasIndex(c => c.GrupoMoneda);
                entity.HasIndex(c => c.Activo);

                // ⚡ Índice compuesto para ordenar métodos de pago activos en la UI de cobro/caja
                entity.HasIndex(c => new { c.Activo, c.Orden });
            });
            builder.Entity<DocumentLog>(entity =>
            {
                entity.ToTable("DocumentLogs");
                entity.HasKey(d => d.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(d => d.DocumentType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(d => d.ReferenceId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(d => d.Action)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(d => d.Details)
                    .HasMaxLength(2000); // Admite detalles en formato JSON o descripción de eventos

                entity.Property(d => d.Timestamp)
                    .IsRequired();

                // ==========================================
                // Relación de Auditoría con Usuario (Identity)
                // ==========================================
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(d => d.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict); // No permite borrar el usuario si tiene trazas de auditoría

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(d => d.UsuarioIdentityId)
                    .HasColumnType("char(36)");

                entity.HasIndex(d => d.UsuarioIdentityId);
                entity.HasIndex(d => d.ReferenceId);
                entity.HasIndex(d => d.Timestamp);

                // ⚡ Índice compuesto clave: Acelera consultar el historial de auditoría de un documento específico
                // (Ej: Where(d => d.DocumentType == "ReciboFactura" && d.ReferenceId == "REC-2024-001"))
                entity.HasIndex(d => new { d.DocumentType, d.ReferenceId });
            });

           builder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(a => a.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(a => a.ActionType)
                    .IsRequired()
                    .HasMaxLength(100);


                entity.Property(a => a.IpAddress)
                    .HasMaxLength(50);

                // Tipos de texto amplios para almacenar deltas / snapshots en formato JSON
                entity.Property(a => a.OldValue)
                    .HasColumnType("longtext");

                entity.Property(a => a.NewValue)
                    .HasColumnType("longtext");

                entity.Property(a => a.Timestamp)
                    .IsRequired();

                // ==========================================
                // Relación de Auditoría con Usuario (Identity)
                // ==========================================
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(a => a.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict); // No permite borrar el usuario si tiene trazas de auditoría

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(a => a.UsuarioIdentityId)
                    .HasColumnType("char(36)");

                entity.HasIndex(a => a.UsuarioIdentityId);
                entity.HasIndex(a => a.ActionType);
                entity.HasIndex(a => a.Timestamp);

                // ⚡ Índice compuesto para reportes de seguridad y trazabilidad por acción y fecha
                entity.HasIndex(a => new { a.ActionType, a.Timestamp });
            });

          builder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(n => n.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(n => n.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(n => n.Message)
                    .IsRequired()
                    .HasMaxLength(1000); // Permite mensajes explicativos más detallados

                entity.Property(n => n.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(n => n.TargetRole)
                    .HasMaxLength(100);

                entity.Property(n => n.ActionUrl)
                    .HasMaxLength(500); // Enlace de redirección al hacer clic en la alerta

                // ==========================================
                // Boleanos y Fechas
                // ==========================================
                entity.Property(n => n.IsRead)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(n => n.Timestamp)
                    .IsRequired();

                // ==========================================
                // Relación con Usuario Destino (Identity)
                // ==========================================
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(n => n.TargetUserGuidId)
                    .OnDelete(DeleteBehavior.Cascade); // Si se elimina el usuario, se limpian sus notificaciones

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(n => n.TargetUserGuidId)
                    .HasColumnType("char(36)");

                entity.HasIndex(n => n.TargetUserGuidId);
                entity.HasIndex(n => n.TargetRole);
                entity.HasIndex(n => n.Timestamp);

                // ⚡ CRÍTICO: Índice compuesto para la campana de notificaciones (Contador de no leídas)
                // (Ej: Where(n => n.TargetUserGuidId == userId && !n.IsRead))
                entity.HasIndex(n => new { n.TargetUserGuidId, n.IsRead });
            });

           builder.Entity<HonorariumMappingRule>(entity =>
            {
                entity.ToTable("HonorariumMappingRules");
                entity.HasKey(h => h.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(h => h.Pattern)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(h => h.Category)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(h => h.UsuarioCreo)
                    .HasMaxLength(100);

                // ==========================================
                // Conversiones y Estados Boleanos
                // ==========================================
                entity.Property(h => h.MappingRuleType)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(h => h.Priority)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.Property(h => h.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(h => h.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Relación de Auditoría con Usuario (Identity)
                // ==========================================
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioCreoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(h => h.UsuarioCreoId)
                    .HasColumnType("char(36)");

                entity.HasIndex(h => h.UsuarioCreoId);
                entity.HasIndex(h => h.Category);

                // ⚡ CRÍTICO: Índice compuesto para el motor de mapeo de honorarios
                // (Ej: Where(r => r.IsActive).OrderBy(r => r.Priority))
                entity.HasIndex(h => new { h.IsActive, h.Priority });
            });

            builder.Entity<HonorarioMedicoServicio>(entity =>
            {
                entity.ToTable("HonorariosMedicosServicios");
                entity.HasKey(h => h.Id);

                // ==========================================
                // Precisión Decimal y Textos
                // ==========================================
                entity.Property(h => h.MontoHonorario)
                    .HasPrecision(18, 2)
                    .IsRequired();
                entity.Property(h => h.FechaModificacion)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Servicio Clínico
                entity.HasOne(h => h.Servicio)
                    .WithMany(s => s.HonorariosMedicos) // o .WithMany() si ServicioClinico no tiene la colección
                    .HasForeignKey(h => h.ServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Médico
                entity.HasOne(h => h.Medico)
                    .WithMany()
                    .HasForeignKey(h => h.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 3. Relación de Auditoría con Usuario
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioModificoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(h => h.UsuarioModificoId)
                    .HasColumnType("char(36)");

                entity.HasIndex(h => h.ServicioId);
                entity.HasIndex(h => h.MedicoId);
                entity.HasIndex(h => h.UsuarioModificoId);
                entity.HasIndex(h => h.FechaModificacion);

                // ⚡ CRÍTICO: Índice único compuesto para evitar asignar dos tarifas al mismo médico para el mismo servicio
                entity.HasIndex(h => new { h.ServicioId, h.MedicoId })
                    .IsUnique();
            });

          builder.Entity<HistorialModificacionCuenta>(entity =>
            {
                entity.ToTable("HistorialModificacionCuentas");
                entity.HasKey(h => h.Id);

                // ==========================================
                // Precisión de Decimales (9 Columnas Financieras)
                // ==========================================
                entity.Property(h => h.TotalAnteriorUSD).HasPrecision(18, 2);
                entity.Property(h => h.TotalNuevoUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboTotalAnteriorUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboTotalNuevoUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboVueltoAnteriorUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboVueltoNuevoUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboPagadoUSD).HasPrecision(18, 2);
                entity.Property(h => h.CxCSaldoAnteriorUSD).HasPrecision(18, 2);
                entity.Property(h => h.CxCSaldoNuevoUSD).HasPrecision(18, 2);

                // ==========================================
                // Propiedades de Texto y Fechas
                // ==========================================

                entity.Property(h => h.TipoIngresoAnterior)
                    .HasMaxLength(50);

                entity.Property(h => h.TipoIngresoNuevo)
                    .HasMaxLength(50);

                entity.Property(h => h.FechaModificacion)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con CuentaServicios (Cascade)
                entity.HasOne<CuentaServicios>()
                    .WithMany()
                    .HasForeignKey(h => h.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Colección de Detalles Modificados (Cascade)
                entity.HasMany(h => h.DetallesModificados)
                    .WithOne(d => d.HistorialModificacionCuenta) // Ajusta el nombre si la propiedad existe en el detalle
                    .HasForeignKey("HistorialModificacionCuentaId")
                    .OnDelete(DeleteBehavior.Cascade);

                // 3. Relaciones opcionales con Paciente y Convenio (SetNull)
                entity.HasOne<PacienteAdmision>()
                    .WithMany()
                    .HasForeignKey(h => h.PacienteAnteriorId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<PacienteAdmision>()
                    .WithMany()
                    .HasForeignKey(h => h.PacienteNuevoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<SeguroConvenio>()
                    .WithMany()
                    .HasForeignKey(h => h.ConvenioAnteriorId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<SeguroConvenio>()
                    .WithMany()
                    .HasForeignKey(h => h.ConvenioNuevoId)
                    .OnDelete(DeleteBehavior.SetNull);

                // 4. Relación de Auditoría con Usuario (Identity)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(h => h.UsuarioId).HasColumnType("char(36)");

                entity.HasIndex(h => h.CuentaServicioId);
                entity.HasIndex(h => h.UsuarioId);
                entity.HasIndex(h => h.PacienteAnteriorId);
                entity.HasIndex(h => h.PacienteNuevoId);
                entity.HasIndex(h => h.ConvenioAnteriorId);
                entity.HasIndex(h => h.ConvenioNuevoId);
                entity.HasIndex(h => h.FechaModificacion);
            });
            builder.Entity<HistorialModificacionCuentaDetalle>(entity =>
            {
                entity.ToTable("HistorialModificacionCuentaDetalles");
                entity.HasKey(d => d.Id);

                // ==========================================
                // Precisión de Decimales (Precios y Honorarios: 18, 2)
                // ==========================================
                entity.Property(d => d.PrecioAnterior)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(d => d.PrecioNuevo)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(d => d.HonorarioAnterior)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(d => d.HonorarioNuevo)
                    .HasPrecision(18, 2)
                    .IsRequired();

                // ==========================================
                // Precisión de Cantidades (18, 4 para soportar fraccionamiento)
                // ==========================================
                entity.Property(d => d.CantidadAnterior)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(d => d.CantidadNueva)
                    .HasPrecision(18, 4)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación Inversa con la cabecera del Historial (Cascade)
                entity.HasOne(d => d.HistorialModificacionCuenta)
                    .WithMany(h => h.DetallesModificados)
                    .HasForeignKey(d => d.HistorialModificacionCuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con el Detalle de Servicio
                entity.HasOne(d => d.DetalleServicio)
                    .WithMany()
                    .HasForeignKey(d => d.DetalleServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Índices de Rendimiento
                // ==========================================
                entity.HasIndex(d => d.HistorialModificacionCuentaId);
                entity.HasIndex(d => d.DetalleServicioId);
            });
          builder.Entity<Insumo>(entity =>
            {
                entity.ToTable("Insumos");
                entity.HasKey(i => i.Id);

                // ==========================================
                // Ignorar Propiedades Calculadas
                // ==========================================
                entity.Ignore(i => i.StockActual);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(i => i.Codigo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.Nombre)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(i => i.UnidadMedidaBase)
                    .HasMaxLength(50);

                entity.Property(i => i.ReactivosCombinados)
                    .HasMaxLength(500);

                entity.Property(i => i.Indicaciones)
                    .HasMaxLength(1000);

                // ==========================================
                // Precisión de Costos (18, 4 para micro-dosificaciones)
                // ==========================================
                entity.Property(i => i.CostoUnitarioBaseUSD)
                    .HasPrecision(18, 4)
                    .IsRequired();

                // ==========================================
                // Boleanos y Valores por Defecto
                // ==========================================
                entity.Property(i => i.PermiteFraccionamiento)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(i => i.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(i => i.OcultoEnTraslados)
                    .IsRequired()
                    .HasDefaultValue(false);

                // ==========================================
                // Relaciones (Foreign Keys y Colecciones)
                // ==========================================
                // 1. Relación con Unidad de Medida (3FN)
                entity.HasOne(i => i.UnidadMedidaNav)
                    .WithMany()
                    .HasForeignKey(i => i.UnidadMedidaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. Relación con Categoría de Insumo
                entity.HasOne(i => i.CategoriaInsumo)
                    .WithMany()
                    .HasForeignKey(i => i.CategoriaInsumoId)
                    .OnDelete(DeleteBehavior.SetNull);

                // 3. Relación con Stock por Sede (Cascade)
                entity.HasMany(i => i.StocksPorSede)
                    .WithOne(s => s.Insumo) // o .WithOne()
                    .HasForeignKey(s => s.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 4. Relación N:M con Principios Activos (Cascade)
                entity.HasMany(i => i.PrincipiosActivos)
                    .WithOne(p => p.Insumo)
                    .HasForeignKey(p => p.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Índices para Búsquedas y Kardex
                // ==========================================
                entity.HasIndex(i => i.Codigo)
                    .IsUnique(); // Código de barra o SKU único

                entity.HasIndex(i => i.Nombre); // Acelera el autocompletado de medicamentos e insumos
                entity.HasIndex(i => i.UnidadMedidaId);
                entity.HasIndex(i => i.CategoriaInsumoId);
                entity.HasIndex(i => i.IsDeleted);
                entity.HasIndex(i => i.OcultoEnTraslados);
                entity.HasIndex(i => i.FechaVencimiento);
            });
            
            builder.Entity<UnidadMedidaCatalogo>(entity =>
            {
                entity.ToTable("UnidadesMedida");
                entity.HasKey(u => u.Id);

                // Identificador manual (catálogo maestro con IDs constantes)
                entity.Property(u => u.Id)
                    .ValueGeneratedNever();

                // ==========================================
                // Propiedades y Restricciones
                // ==========================================
                entity.Property(u => u.Codigo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(u => u.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Simbolo)
                    .IsRequired()
                    .HasMaxLength(20);

                // ==========================================
                // Boleanos y Valores por Defecto
                // ==========================================
                entity.Property(u => u.EsFraccionable)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(u => u.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                // ⚡ Índice único para evitar códigos de unidades duplicados
                entity.HasIndex(u => u.Codigo)
                    .IsUnique();

                entity.HasIndex(u => u.Activo);

                // ==========================================
                // Semilla de Datos Inicial (Seed Data 3FN)
                // ==========================================
                entity.HasData(
                    new UnidadMedidaCatalogo(UnidadMedidaConstants.UnidadId, "UNIDAD", "Unidad", "UND", esFraccionable: true, activo: true),
                    new UnidadMedidaCatalogo(UnidadMedidaConstants.KgId, "KG", "Kilogramo", "kg", esFraccionable: true, activo: true),
                    new UnidadMedidaCatalogo(UnidadMedidaConstants.GramoId, "G", "Gramo", "g", esFraccionable: true, activo: true),
                    new UnidadMedidaCatalogo(UnidadMedidaConstants.DecigramoId, "DG", "Decigramo", "dg", esFraccionable: true, activo: true),
                    new UnidadMedidaCatalogo(UnidadMedidaConstants.MiligramoId, "MG", "Miligramo", "mg", esFraccionable: true, activo: true),
                    new UnidadMedidaCatalogo(UnidadMedidaConstants.LitroId, "L", "Litro", "L", esFraccionable: true, activo: true),
                    new UnidadMedidaCatalogo(UnidadMedidaConstants.MililitroId, "ML", "Mililitro", "mL", esFraccionable: true, activo: true)
                );
            });

           builder.Entity<CategoriaInsumo>(entity =>
            {
                entity.ToTable("CategoriasInsumo");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(c => c.Codigo)
                    .HasMaxLength(50);

                // ==========================================
                // Boleanos, Defaults y Fechas
                // ==========================================
                entity.Property(c => c.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(c => c.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                // ⚡ Índice único para evitar nombres de categorías duplicados
                entity.HasIndex(c => c.Nombre)
                    .IsUnique();

                // Índice para códigos de categoría
                entity.HasIndex(c => c.Codigo);

                // Índice para optimizar dropdowns de categorías activas
                entity.HasIndex(c => c.Activo);

                // ==========================================
                // Semilla de Datos Inicial (Seed Data)
                // ==========================================
                entity.HasData(
                    new CategoriaInsumo("Medicamento", "MED", SeedConstants.CategoriaId_Medicamento, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                    new CategoriaInsumo("Descartable", "DESC", SeedConstants.CategoriaId_Descartable, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                    new CategoriaInsumo("Material Médico", "MAT-MED", SeedConstants.CategoriaId_MaterialMedico, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                    new CategoriaInsumo("Reactivo", "REACT", SeedConstants.CategoriaId_Reactivo, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                    new CategoriaInsumo("Material Quirúrgico", "MAT-QX", SeedConstants.CategoriaId_MaterialQuirurgico, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                    new CategoriaInsumo("Otro", "OTRO", SeedConstants.CategoriaId_Otro, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                );
            });
            
           builder.Entity<PrincipioActivo>(entity =>
            {
                entity.ToTable("PrincipiosActivos");
                entity.HasKey(p => p.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(p => p.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                // ==========================================
                // Boleanos y Valores por Defecto
                // ==========================================
                entity.Property(p => p.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // ==========================================
                // Relación con la tabla intermedia (Cascade)
                // ==========================================
                // ⚡ CRÍTICO: Mapeo explícito de la colección Insumos (Evita error EF Core 8)
                entity.HasMany(p => p.Insumos)
                    .WithOne(i => i.PrincipioActivo)
                    .HasForeignKey(i => i.PrincipioActivoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                // ⚡ Índice único para evitar principios activos repetidos (ej. no duplicar 'Paracetamol' o 'Amoxicilina')
                entity.HasIndex(p => p.Nombre)
                    .IsUnique();

                // Índice para optimizar búsquedas de principios activos activos
                entity.HasIndex(p => p.Activo);
            });

           builder.Entity<InsumoPrincipioActivo>(entity =>
            {
                entity.ToTable("InsumosPrincipiosActivos");

                // ==========================================
                // Clave Primaria Compuesta (N:M)
                // ==========================================
                entity.HasKey(ipa => new { ipa.InsumoId, ipa.PrincipioActivoId });

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(ipa => ipa.Concentracion)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue(string.Empty);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Insumo (Cascade: Si se elimina el medicamento, se elimina la relación)
                entity.HasOne(ipa => ipa.Insumo)
                    .WithMany(i => i.PrincipiosActivos)
                    .HasForeignKey(ipa => ipa.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con PrincipioActivo (Restrict: No permite borrar el genérico si está en uso)
                entity.HasOne(ipa => ipa.PrincipioActivo)
                    .WithMany(pa => pa.Insumos)
                    .HasForeignKey(ipa => ipa.PrincipioActivoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento
                // ==========================================
                entity.HasIndex(ipa => ipa.InsumoId);

                // ⚡ Índice para acelerar la búsqueda inversa: Buscar todos los medicamentos que contengan un principio activo
                // (Ej: Buscar todos los fármacos con 'Ibuprofeno')
                entity.HasIndex(ipa => ipa.PrincipioActivoId);
            });

            builder.Entity<ServicioInsumoReceta>(entity =>
            {
                entity.ToTable("ServiciosInsumoRecetas");
                entity.HasKey(r => r.Id);

                // ==========================================
                // Precisión de Cantidades
                // ==========================================
                // ⚠️ 18, 4 para soportar dosis fraccionadas exactas (ej. 0.25 mL, 2.5000 g)
                entity.Property(r => r.Cantidad)
                    .HasPrecision(18, 4)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Servicio Clínico (Cascade: Si se elimina el estudio/servicio, se borra su receta de insumos)
                entity.HasOne(r => r.ServicioClinico)
                    .WithMany()
                    .HasForeignKey(r => r.ServicioClinicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Insumo (Restrict: No permite borrar el insumo si está en la receta de un servicio)
                entity.HasOne(r => r.Insumo)
                    .WithMany()
                    .HasForeignKey(r => r.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con Unidad de Medida (3FN)
                entity.HasOne(r => r.UnidadMedidaNav)
                    .WithMany()
                    .HasForeignKey(r => r.UnidadMedidaConsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(r => r.ServicioClinicoId);
                entity.HasIndex(r => r.InsumoId);
                entity.HasIndex(r => r.UnidadMedidaConsumoId);

                // ⚡ CRÍTICO: Índice único compuesto para evitar agregar el mismo insumo dos veces a la misma receta de servicio
                entity.HasIndex(r => new { r.ServicioClinicoId, r.InsumoId })
                    .IsUnique();
            });

           builder.Entity<ConsumoServicioRealizado>(entity =>
            {
                entity.ToTable("ConsumosServiciosRealizados");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Precisión de Cantidades y Costos
                // ==========================================
                entity.Property(c => c.CantidadConsumidaBase)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(c => c.CostoTotalUSD)
                    .HasPrecision(18, 2) // o (18, 4) si manejas micro-costos acumulados
                    .IsRequired();

                entity.Property(c => c.FechaConsumo)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Detalle de Servicio en Cuenta (Cascade: Si se elimina el cargo, se borra el consumo registrado)
                entity.HasOne(c => c.DetalleServicioCuenta)
                    .WithMany()
                    .HasForeignKey(c => c.DetalleServicioCuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Insumo (Restrict: No permite borrar el insumo si tiene consumos históricos)
                entity.HasOne(c => c.Insumo)
                    .WithMany()
                    .HasForeignKey(c => c.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices para Reportes de Kardex y Costos
                // ==========================================
                entity.HasIndex(c => c.DetalleServicioCuentaId);
                entity.HasIndex(c => c.InsumoId);

                // ⚡ Índice para reportes de costos de insumos consumidos por rango de fechas
                entity.HasIndex(c => c.FechaConsumo);
            });

            builder.Entity<MovimientoInsumo>(entity =>
            {
                entity.ToTable("MovimientosInsumo");
                entity.HasKey(m => m.Id);

                // ==========================================
                // Conversiones y Precisión Decimal
                // ==========================================
                entity.Property(m => m.TipoMovimiento)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(m => m.CantidadBase)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(m => m.CantidadOriginal)
                    .HasPrecision(18, 4)
                    .IsRequired();

                // ==========================================
                // Propiedades de Texto y Fechas
                // ==========================================
                entity.Property(m => m.Motivo)
                    .HasMaxLength(500);

                entity.Property(m => m.UsuarioId)
                    .HasMaxLength(100);

                entity.Property(m => m.Fecha)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys - Restrict para trazabilidad)
                // ==========================================
                // 1. Relación con Insumo
                entity.HasOne(m => m.Insumo)
                    .WithMany()
                    .HasForeignKey(m => m.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. Relación con Sede
                entity.HasOne(m => m.Sede)
                    .WithMany()
                    .HasForeignKey(m => m.SedeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con Unidad de Medida (3FN)
                entity.HasOne(m => m.UnidadMedidaNav)
                    .WithMany()
                    .HasForeignKey(m => m.UnidadMedidaOriginalId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 4. Relación de Auditoría con Usuario (Identity)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(m => m.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices de Kardex
                // ==========================================
                entity.Property(m => m.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(m => m.InsumoId);
                entity.HasIndex(m => m.SedeId);
                entity.HasIndex(m => m.UnidadMedidaOriginalId);
                entity.HasIndex(m => m.UsuarioIdentityId);
                entity.HasIndex(m => m.TipoMovimiento);
                entity.HasIndex(m => m.Fecha);

                // ⚡ CRÍTICO: Índice compuesto para la consulta del Kardex por Insumo, Sede y Fecha
                // (Ej: Where(m => m.InsumoId == id && m.SedeId == sedeId).OrderBy(m => m.Fecha))
                entity.HasIndex(m => new { m.InsumoId, m.SedeId, m.Fecha });
            });

          builder.Entity<CierreInventario>(entity =>
            {
                entity.ToTable("CierresInventario");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Propiedades de Texto y Fechas
                // ==========================================
                entity.Property(c => c.Observaciones)
                    .HasMaxLength(1000);

                entity.Property(c => c.Usuario)
                    .HasMaxLength(100);

                entity.Property(c => c.FechaCierre)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Sede (Restrict: No permite borrar la sede si tiene cierres de inventario)
                entity.HasOne(c => c.Sede)
                    .WithMany()
                    .HasForeignKey(c => c.SedeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. Relación con la colección de Detalles del Cierre (Cascade)
                entity.HasMany(c => c.Detalles)
                    .WithOne(d => d.CierreInventario)
                    .HasForeignKey(d => d.CierreInventarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 3. Relación de Auditoría con Usuario (Identity)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(c => c.UsuarioId)
                    .HasColumnType("char(36)");

                entity.HasIndex(c => c.SedeId);
                entity.HasIndex(c => c.UsuarioId);
                entity.HasIndex(c => c.FechaCierre);

                // ⚡ Índice compuesto para consultar cierres de inventario por sede ordenados por fecha
                entity.HasIndex(c => new { c.SedeId, c.FechaCierre });
            });

            builder.Entity<CierreInventarioDetalle>(entity =>
            {
                entity.ToTable("CierresInventarioDetalles");
                entity.HasKey(d => d.Id);

                // ==========================================
                // Precisión de Cantidades y Costos (18, 4)
                // ==========================================
                entity.Property(d => d.StockTeoricoBase)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(d => d.StockRealBase)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(d => d.CostoBaseUSD)
                    .HasPrecision(18, 4)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con la Cabecera de Cierre (Cascade: Si se elimina el cierre, se borran sus renglones)
                entity.HasOne(d => d.CierreInventario)
                    .WithMany(c => c.Detalles)
                    .HasForeignKey(d => d.CierreInventarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Insumo (Restrict: No permite borrar el insumo si tiene auditorías de cierre de stock)
                entity.HasOne(d => d.Insumo)
                    .WithMany()
                    .HasForeignKey(d => d.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(d => d.CierreInventarioId);
                entity.HasIndex(d => d.InsumoId);

                // ⚡ CRÍTICO: Índice único compuesto para evitar auditar el mismo insumo dos veces en el mismo cierre
                entity.HasIndex(d => new { d.CierreInventarioId, d.InsumoId })
                    .IsUnique();
            });

            builder.Entity<Sede>(entity =>
            {
                entity.ToTable("Sedes");
                entity.HasKey(s => s.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(s => s.Codigo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                // ==========================================
                // Boleanos y Valores por Defecto
                // ==========================================
                entity.Property(s => s.EsPrincipal)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(s => s.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // ==========================================
                // Relación con Áreas Clínicas (1:N)
                // ==========================================
                // ⚡ CRÍTICO: Mapeo explícito de la colección AreasClinicas (Restrict: No permite borrar la sede si tiene camas/áreas)
                entity.HasMany(s => s.AreasClinicas)
                    .WithOne(a => a.Sede)
                    .HasForeignKey(a => a.SedeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                // ⚡ Índice único para evitar códigos de sede duplicados (ej. 'SEDE-PRINCIPAL')
                entity.HasIndex(s => s.Codigo)
                    .IsUnique();

                entity.HasIndex(s => s.Nombre);
                entity.HasIndex(s => s.Activo);
                entity.HasIndex(s => s.EsPrincipal);

                // ==========================================
                // Semilla de Datos Inicial (Seed Data)
                // ==========================================
                entity.HasData(
                    new Sede("SEDE-PRINCIPAL", "Almacén Principal / Farmacia Central", esPrincipal: true, SeedConstants.SedeId_Principal),
                    new Sede("SEDE-EMG", "Depósito Emergencia", esPrincipal: false, SeedConstants.SedeId_Emergencia),
                    new Sede("SEDE-HOSP", "Depósito Hospitalización", esPrincipal: false, SeedConstants.SedeId_Hospitalizacion),
                    new Sede("SEDE-UCI", "Depósito UCI", esPrincipal: false, SeedConstants.SedeId_UCI),
                    new Sede("SEDE-CIRUGIA", "Quirófano / Pabellón Central", esPrincipal: false, SeedConstants.SedeId_Cirugia)
                );
            });

            builder.Entity<ClasificacionArea>(entity =>
            {
                entity.ToTable("ClasificacionesAreas");
                entity.HasKey(c => c.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(c => c.Codigo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Descripcion)
                    .IsRequired()
                    .HasMaxLength(150);

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                // ⚡ Índice único para evitar códigos de clasificación duplicados
                entity.HasIndex(c => c.Codigo)
                    .IsUnique();

                entity.HasIndex(c => c.Descripcion);

                // ==========================================
                // Semilla de Datos Inicial (Seed Data 3FN)
                // ==========================================
                // 🎯 Esta semilla resuelve de raíz el error 500 inicial al crear Áreas Clínicas
                entity.HasData(
                    new ClasificacionArea("CAMA", "Cama", SeedConstants.ClasificacionId_Cama),
                    new ClasificacionArea("QUIROFANO", "Quirófano", SeedConstants.ClasificacionId_Quirofano),
                    new ClasificacionArea("SALA_PARTO", "Sala de Parto", SeedConstants.ClasificacionId_SalaParto)
                );
            });

            builder.Entity<AreaClinica>(entity =>
            {
                entity.ToTable("AreasClinicas");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(a => a.Estado).HasConversion<int>().IsRequired();
                entity.Property(a => a.EsAreaAdmision).IsRequired();

                entity.HasOne(a => a.Sede)
                    .WithMany(s => s.AreasClinicas)
                    .HasForeignKey(a => a.SedeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Clasificacion)
                    .WithMany()
                    .HasForeignKey(a => a.ClasificacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.ServicioTarifaBase)
                    .WithMany()
                    .HasForeignKey(a => a.ServicioTarifaBaseId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(a => new { a.SedeId, a.Codigo }).IsUnique();

                entity.HasData(
                    new
                    {
                        Id = SeedConstants.AreaId_Emergencia,
                        SedeId = SeedConstants.SedeId_Emergencia,
                        Codigo = "BOX-1",
                        Nombre = "Box Emergencia 1",
                        Activo = true,
                        Estado = EstadoUbicacion.Disponible,
                        EsSubAreaAlmacenPrincipal = false,
                        EsAreaAdmision = true,
                        ServicioTarifaBaseId = (Guid?)null,
                        ClasificacionId = SeedConstants.ClasificacionId_Cama
                    },
                    new
                    {
                        Id = SeedConstants.AreaId_Hospitalizacion,
                        SedeId = SeedConstants.SedeId_Hospitalizacion,
                        Codigo = "HAB-101",
                        Nombre = "Habitación 101",
                        Activo = true,
                        Estado = EstadoUbicacion.Disponible,
                        EsSubAreaAlmacenPrincipal = false,
                        EsAreaAdmision = false,
                        ServicioTarifaBaseId = (Guid?)null,
                        ClasificacionId = SeedConstants.ClasificacionId_Cama
                    },
                    new
                    {
                        Id = SeedConstants.AreaId_UCI,
                        SedeId = SeedConstants.SedeId_UCI,
                        Codigo = "UCI-1",
                        Nombre = "Cama UCI 1",
                        Activo = true,
                        Estado = EstadoUbicacion.Disponible,
                        EsSubAreaAlmacenPrincipal = false,
                        EsAreaAdmision = false,
                        ServicioTarifaBaseId = (Guid?)null,
                        ClasificacionId = SeedConstants.ClasificacionId_Cama
                    },
                    new
                    {
                        Id = SeedConstants.AreaId_Farmacia,
                        SedeId = SeedConstants.SedeId_Principal,
                        Codigo = "FARMACIA",
                        Nombre = "Farmacia Central",
                        Activo = true,
                        Estado = EstadoUbicacion.Disponible,
                        EsSubAreaAlmacenPrincipal = false,
                        EsAreaAdmision = false,
                        ServicioTarifaBaseId = (Guid?)null,
                        ClasificacionId = SeedConstants.ClasificacionId_Cama
                    },
                    new
                    {
                        Id = SeedConstants.AreaId_Laboratorio,
                        SedeId = SeedConstants.SedeId_Principal,
                        Codigo = "LABORATORIO",
                        Nombre = "Laboratorio Central",
                        Activo = true,
                        Estado = EstadoUbicacion.Disponible,
                        EsSubAreaAlmacenPrincipal = false,
                        EsAreaAdmision = false,
                        ServicioTarifaBaseId = (Guid?)null,
                        ClasificacionId = SeedConstants.ClasificacionId_Cama
                    },
                    new
                    {
                        Id = SeedConstants.AreaId_Cirugia,
                        SedeId = SeedConstants.SedeId_Cirugia,
                        Codigo = "QX-1",
                        Nombre = "Quirófano 1 (Cirugía Mayor)",
                        Activo = true,
                        Estado = EstadoUbicacion.Disponible,
                        EsSubAreaAlmacenPrincipal = false,
                        EsAreaAdmision = false,
                        ServicioTarifaBaseId = (Guid?)null,
                        ClasificacionId = SeedConstants.ClasificacionId_Quirofano
                    }
                );
            });

           builder.Entity<ServicioIncluidoArea>(entity =>
            {
                entity.ToTable("ServiciosIncluidosArea");

                // ==========================================
                // Clave Primaria Compuesta (N:M)
                // ==========================================
                entity.HasKey(sia => new { sia.AreaClinicaId, sia.ServicioClinicoId });

                // ==========================================
                // Boleanos y Valores por Defecto
                // ==========================================
                entity.Property(sia => sia.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // ==========================================
                // Relaciones (Foreign Keys - Cascade)
                // ==========================================
                // 1. Relación con Área Clínica
                entity.HasOne(sia => sia.AreaClinica)
                    .WithMany()
                    .HasForeignKey(sia => sia.AreaClinicaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Servicio Clínico
                entity.HasOne(sia => sia.ServicioClinico)
                    .WithMany()
                    .HasForeignKey(sia => sia.ServicioClinicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ==========================================
                // Índices de Rendimiento
                // ==========================================
                entity.HasIndex(sia => sia.AreaClinicaId);
                entity.HasIndex(sia => sia.ServicioClinicoId);

                // ⚡ CRÍTICO: Índice compuesto para consultar rápidamente los servicios incluidos activos de un área/cama
                // (Ej: Where(sia => sia.AreaClinicaId == areaId && sia.Activo))
                entity.HasIndex(sia => new { sia.AreaClinicaId, sia.Activo });
            });

            builder.Entity<InsumoCirugiaPaciente>(entity =>
            {
                entity.ToTable("InsumosCirugiaPaciente");
                entity.HasKey(icp => icp.Id);

                // ==========================================
                // Ignorar Propiedades Calculadas
                // ==========================================
                entity.Ignore(icp => icp.CantidadConsumida);

                // ==========================================
                // Precisión de Cantidades (18, 4 para insumos fraccionables)
                // ==========================================
                entity.Property(icp => icp.CantidadEntregada)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(icp => icp.CantidadDevuelta)
                    .HasPrecision(18, 4)
                    .IsRequired()
                    .HasDefaultValue(0.0000m);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Cuenta de Servicios (Cascade: Si se elimina la cuenta clínica, se borran los insumos quirúrgicos)
                entity.HasOne(icp => icp.CuentaServicio)
                    .WithMany()
                    .HasForeignKey(icp => icp.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Orden de Cirugía / Pabellón (Opcional)
                entity.HasOne(icp => icp.OrdenCirugia)
                    .WithMany()
                    .HasForeignKey(icp => icp.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.SetNull);

                // 3. Relación con Insumo (Restrict: No permite borrar el insumo si está en un kit quirúrgico despachado)
                entity.HasOne(icp => icp.Insumo)
                    .WithMany()
                    .HasForeignKey(icp => icp.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento para Quirófano
                // ==========================================
                entity.HasIndex(icp => icp.CuentaServicioId);
                entity.HasIndex(icp => icp.OrdenCirugiaId);
                entity.HasIndex(icp => icp.InsumoId);

                // ⚡ Índice compuesto para consultar rápidamente el kit de insumos quirúrgicos de una cuenta
                entity.HasIndex(icp => new { icp.CuentaServicioId, icp.InsumoId });
            });

            builder.Entity<StockSede>(entity =>
            {
                entity.ToTable("StocksSede");
                entity.HasKey(s => s.Id);

                // ==========================================
                // Precisión de Existencias y Límites (18, 4)
                // ==========================================
                entity.Property(s => s.StockActual)
                    .HasPrecision(18, 4)
                    .IsRequired()
                    .HasDefaultValue(0.0000m);

                entity.Property(s => s.StockMinimo)
                    .HasPrecision(18, 4);

                entity.Property(s => s.StockMaximo)
                    .HasPrecision(18, 4);

                // ==========================================
                // ⚡ Concurrencia Optimista (Evita condiciones de carrera en ventas simultáneas)
                // ==========================================
                entity.Property(s => s.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Insumo (Cascade: Si se elimina el insumo, se eliminan sus stocks)
                entity.HasOne(s => s.Insumo)
                    .WithMany(i => i.StocksPorSede)
                    .HasForeignKey(s => s.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Sede (Restrict: No permite borrar la sede si tiene existencias registradas)
                entity.HasOne(s => s.Sede)
                    .WithMany()
                    .HasForeignKey(s => s.SedeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(s => s.InsumoId);
                entity.HasIndex(s => s.SedeId);

                // ⚡ CRÍTICO: Índice único compuesto para asegurar que un insumo solo tenga UN registro de stock por sede
                entity.HasIndex(s => new { s.InsumoId, s.SedeId })
                    .IsUnique();

                // ⚡ Índice para alertas de stock bajo y reorden por sede
                entity.HasIndex(s => new { s.SedeId, s.StockActual });
            });


            builder.Entity<PedidoInterSede>(entity =>
            {
                entity.ToTable("PedidosInterSede");
                entity.HasKey(p => p.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(p => p.Correlativo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.UsuarioCreador)
                    .HasMaxLength(100);

                entity.Property(p => p.Observaciones)
                    .HasMaxLength(1000);

                // ==========================================
                // Conversiones y Fechas
                // ==========================================
                entity.Property(p => p.Estado)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(p => p.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Sede Solicitante (Restrict)
                entity.HasOne(p => p.SedeSolicitante)
                    .WithMany()
                    .HasForeignKey(p => p.SedeSolicitanteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. Relación con Sede Proveedora (Restrict)
                entity.HasOne(p => p.SedeProveedora)
                    .WithMany()
                    .HasForeignKey(p => p.SedeProveedoraId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con Detalles del Pedido (Cascade)
                entity.HasMany(p => p.Detalles)
                    .WithOne(d => d.PedidoInterSede) // o .WithOne()
                    .HasForeignKey(d => d.PedidoInterSedeId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 4. Relación de Auditoría con Usuario (Identity)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(p => p.UsuarioCreadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(p => p.UsuarioCreadorId).HasColumnType("char(36)");

                // ⚡ Correlativo de transferencia único (ej. PED-2026-0001)
                entity.HasIndex(p => p.Correlativo).IsUnique();

                entity.HasIndex(p => p.SedeSolicitanteId);
                entity.HasIndex(p => p.SedeProveedoraId);
                entity.HasIndex(p => p.UsuarioCreadorId);
                entity.HasIndex(p => p.Estado);
                entity.HasIndex(p => p.FechaCreacion);

                // ⚡ Índices compuestos para pantallas de traslados pendientes de despacho / recepción
                entity.HasIndex(p => new { p.SedeSolicitanteId, p.Estado });
                entity.HasIndex(p => new { p.SedeProveedoraId, p.Estado });
            });

            builder.Entity<PedidoInterSedeDetalle>(entity =>
            {
                entity.ToTable("PedidosInterSedeDetalles");
                entity.HasKey(d => d.Id);

                // ==========================================
                // Precisión de Cantidades (18, 4 para medicamentos fraccionables)
                // ==========================================
                entity.Property(d => d.CantidadSolicitada)
                        .HasPrecision(18, 4)
                        .IsRequired();

                entity.Property(d => d.CantidadDespachada)
                        .HasPrecision(18, 4)
                        .IsRequired()
                        .HasDefaultValue(0.0000m);

                entity.Property(d => d.CantidadRecibida)
                        .HasPrecision(18, 4)
                        .IsRequired()
                        .HasDefaultValue(0.0000m);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(d => d.ObservacionDespacho)
                        .HasMaxLength(500);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación Inversa con PedidoInterSede (Cascade)
                entity.HasOne(d => d.PedidoInterSede)
                        .WithMany(p => p.Detalles)
                        .HasForeignKey(d => d.PedidoInterSedeId)
                        .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Insumo (Restrict: No permite borrar el insumo si está en un traslado)
                entity.HasOne(d => d.Insumo)
                        .WithMany()
                        .HasForeignKey(d => d.InsumoId)
                        .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(d => d.PedidoInterSedeId);
                entity.HasIndex(d => d.InsumoId);

                // ⚡ CRÍTICO: Índice único compuesto para evitar pedir el mismo insumo dos veces en el mismo pedido de traslado
                entity.HasIndex(d => new { d.PedidoInterSedeId, d.InsumoId })
                        .IsUnique();
            });

            
            builder.Entity<DetalleServicioMedicoResponsable>(entity =>
            {
                entity.ToTable("DetalleServiciosMedicosResponsables");
                entity.HasKey(dsm => dsm.Id);

                // ==========================================
                // Propiedades de Texto y Precisión
                // ==========================================
                entity.Property(dsm => dsm.Rol)
                    .IsRequired()
                    .HasMaxLength(100); // Cirujano Principal, Ayudante, Anestesiólogo, Instrumentista

                entity.Property(dsm => dsm.MontoHonorario)
                    .HasPrecision(18, 2)
                    .IsRequired()
                    .HasDefaultValue(0.00m);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación Inversa con DetalleServicioCuenta (Cascade: Si se elimina el cargo médico, se limpian sus honorarios asignados)
                entity.HasOne(dsm => dsm.DetalleServicioCuenta)
                    .WithMany(dsc => dsc.MedicosResponsables)
                    .HasForeignKey(dsm => dsm.DetalleServicioCuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Médico (Restrict: No permite borrar el médico si tiene honorarios liquidados o pendientes)
                entity.HasOne(dsm => dsm.Medico)
                    .WithMany()
                    .HasForeignKey(dsm => dsm.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento y Liquidación
                // ==========================================
                entity.HasIndex(dsm => dsm.DetalleServicioCuentaId);
                entity.HasIndex(dsm => dsm.MedicoId);
                entity.HasIndex(dsm => dsm.Rol);

                // ⚡ Índice compuesto para reportes de liquidación de honorarios por médico
                entity.HasIndex(dsm => new { dsm.MedicoId, dsm.MontoHonorario });
            });

           builder.Entity<TipoServicio>(entity =>
            {
                entity.ToTable("TiposServicio");
                entity.HasKey(t => t.Id);

                // Identificador manual (catálogo fijo con constantes)
                entity.Property(t => t.Id)
                    .ValueGeneratedNever();

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(t => t.Codigo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(t => t.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                // ⚡ Índice único para evitar códigos de tipo de servicio duplicados (ej. 'LAB', 'RX', 'MEDICO')
                entity.HasIndex(t => t.Codigo)
                    .IsUnique();

                entity.HasIndex(t => t.Nombre);

                // ==========================================
                // Semilla de Datos Inicial (Seed Data 3FN)
                // ==========================================
                entity.HasData(
                    new TipoServicio(TipoServicioConstants.Medico, "Servicio Médico / Consulta", "MEDICO"),
                    new TipoServicio(TipoServicioConstants.Laboratorio, "Examen de Laboratorio", "LAB"),
                    new TipoServicio(TipoServicioConstants.RX, "Rayos X / Imagenología", "RX"),
                    new TipoServicio(TipoServicioConstants.Tomo, "Tomografía Axial", "TOMO"),
                    new TipoServicio(TipoServicioConstants.Insumo, "Insumo / Medicamento", "INSUMO"),
                    new TipoServicio(TipoServicioConstants.Informe, "Informe / Lectura Médica", "INFORME")
                );
            });

            builder.Entity<OrdenCirugia>(entity =>
            {
                entity.ToTable("OrdenesCirugias");
                entity.HasKey(o => o.Id);

                // ==========================================
                // ⚡ Ignorar Propiedades del Patrón State (No persisten en BD)
                // ==========================================
                entity.Ignore(o => o.CurrentState);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(o => o.DescripcionCirugia)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(o => o.Estado)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(o => o.SalaQuirofano)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(o => o.ModalidadAnestesia)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(o => o.MotivoCancelacion)
                    .HasMaxLength(500);

                // ==========================================
                // Precisión de Precios (Derecho de Sala y Base)
                // ==========================================
                entity.Property(o => o.PrecioBaseUsd)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(o => o.PrecioDerechoSalaUsd)
                    .HasPrecision(18, 2)
                    .IsRequired()
                    .HasDefaultValue(0.00m);

                // ==========================================
                // Boleanos y Fechas
                // ==========================================
                entity.Property(o => o.EsAlquilado)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(o => o.FechaHoraProgramada)
                    .IsRequired();

                entity.Property(o => o.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación con Cuenta de Servicios (Cascade)
                entity.HasOne(o => o.CuentaServicio)
                    .WithMany()
                    .HasForeignKey(o => o.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Paciente
                entity.HasOne(o => o.Paciente)
                    .WithMany()
                    .HasForeignKey(o => o.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con Cirujano Principal
                entity.HasOne(o => o.Medico)
                    .WithMany()
                    .HasForeignKey(o => o.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 4. Relaciones con Ubicaciones (Quirófano y Origen)
                entity.HasOne(o => o.AreaClinica)
                    .WithMany()
                    .HasForeignKey(o => o.AreaClinicaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(o => o.SedeQuirofano)
                    .WithMany()
                    .HasForeignKey(o => o.SedeQuirofanoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(o => o.AreaClinicaOrigen)
                    .WithMany()
                    .HasForeignKey(o => o.AreaClinicaOrigenId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(o => o.SedeOrigen)
                    .WithMany()
                    .HasForeignKey(o => o.SedeOrigenId)
                    .OnDelete(DeleteBehavior.SetNull);

                // 5. Relación de Auditoría con Usuario (Identity)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(o => o.UsuarioCreacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // =========================================================================
                // ⚡ CRÍTICO: Mapeo de Colecciones Encapsuladas con Backing-Fields
                // =========================================================================
                // 1. Logs de Auditoría Quirúrgica (_logs)
                entity.HasMany(o => o.Logs)
                    .WithOne()
                    .HasForeignKey("OrdenCirugiaId")
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.Logs).UsePropertyAccessMode(PropertyAccessMode.Field);

                // 2. Requisitos Quirúrgicos (_requisitos)
                entity.HasMany(o => o.Requisitos)
                    .WithOne()
                    .HasForeignKey("OrdenCirugiaId")
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.Requisitos).UsePropertyAccessMode(PropertyAccessMode.Field);

                // 3. Historial de Observaciones (_historialObservaciones)
                entity.HasMany(o => o.HistorialObservaciones)
                    .WithOne()
                    .HasForeignKey("OrdenCirugiaId")
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.HistorialObservaciones).UsePropertyAccessMode(PropertyAccessMode.Field);

                // 4. Honorarios del Equipo Médico (_medicosHonorarios)
                entity.HasMany(o => o.MedicosHonorarios)
                    .WithOne()
                    .HasForeignKey("OrdenCirugiaId")
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.MedicosHonorarios).UsePropertyAccessMode(PropertyAccessMode.Field);

                // 5. Solicitudes de Insumos Extra (_solicitudesInsumos)
                entity.HasMany(o => o.SolicitudesInsumos)
                    .WithOne()
                    .HasForeignKey("OrdenCirugiaId")
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.SolicitudesInsumos).UsePropertyAccessMode(PropertyAccessMode.Field);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(o => o.UsuarioCreacionId).HasColumnType("char(36)");

                entity.HasIndex(o => o.CuentaServicioId);
                entity.HasIndex(o => o.PacienteId);
                entity.HasIndex(o => o.MedicoId);
                entity.HasIndex(o => o.AreaClinicaId);
                entity.HasIndex(o => o.SedeQuirofanoId);
                entity.HasIndex(o => o.Estado);
                entity.HasIndex(o => o.FechaHoraProgramada);

                // ⚡ Índices compuestos para disponibilidad de quirófanos y agenda quirúrgica médica
                entity.HasIndex(o => new { o.AreaClinicaId, o.FechaHoraProgramada });
                entity.HasIndex(o => new { o.MedicoId, o.FechaHoraProgramada });
            });

           builder.Entity<CirugiaMedicoHonorario>(entity =>
            {
                entity.ToTable("CirugiaMedicosHonorarios");
                entity.HasKey(cmh => cmh.Id);

                // ==========================================
                // Precisión de Honorarios y Boleanos
                // ==========================================
                entity.Property(cmh => cmh.MontoHonorarioUsd)
                    .HasPrecision(18, 2)
                    .IsRequired()
                    .HasDefaultValue(0.00m);

                entity.Property(cmh => cmh.EsCirujanoPrincipal)
                    .IsRequired()
                    .HasDefaultValue(false);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación Inversa con Orden de Cirugía (Cascade)
                entity.HasOne(cmh => cmh.OrdenCirugia)
                    .WithMany(o => o.MedicosHonorarios)
                    .HasForeignKey(cmh => cmh.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Médico (Restrict: No permite borrar médico con honorarios quirúrgicos)
                entity.HasOne(cmh => cmh.Medico)
                    .WithMany()
                    .HasForeignKey(cmh => cmh.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con Especialidad del Rol Quirúrgico (Restrict)
                entity.HasOne(cmh => cmh.Especialidad)
                    .WithMany()
                    .HasForeignKey(cmh => cmh.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                entity.HasIndex(cmh => cmh.OrdenCirugiaId);
                entity.HasIndex(cmh => cmh.MedicoId);
                entity.HasIndex(cmh => cmh.EspecialidadId);

                // ⚡ CRÍTICO: Índice único compuesto para evitar asignar al mismo médico dos veces en el mismo equipo quirúrgico
                entity.HasIndex(cmh => new { cmh.OrdenCirugiaId, cmh.MedicoId })
                    .IsUnique();

                // ⚡ Índice para reportes y liquidación de honorarios quirúrgicos por médico
                entity.HasIndex(cmh => new { cmh.MedicoId, cmh.MontoHonorarioUsd });
            });

            builder.Entity<CirugiaMedicoHonorario>(entity =>
            {
                entity.ToTable("CirugiaMedicosHonorarios");
                entity.HasKey(cmh => cmh.Id);

                // ==========================================
                // Precisión de Honorarios y Boleanos
                // ==========================================
                entity.Property(cmh => cmh.MontoHonorarioUsd)
                    .HasPrecision(18, 2)
                    .IsRequired()
                    .HasDefaultValue(0.00m);

                entity.Property(cmh => cmh.EsCirujanoPrincipal)
                    .IsRequired()
                    .HasDefaultValue(false);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación Inversa con Orden de Cirugía (Cascade)
                entity.HasOne(cmh => cmh.OrdenCirugia)
                    .WithMany(o => o.MedicosHonorarios)
                    .HasForeignKey(cmh => cmh.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Médico (Restrict: No permite borrar médico con honorarios quirúrgicos)
                entity.HasOne(cmh => cmh.Medico)
                    .WithMany()
                    .HasForeignKey(cmh => cmh.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con Especialidad del Rol Quirúrgico (Restrict)
                entity.HasOne(cmh => cmh.Especialidad)
                    .WithMany()
                    .HasForeignKey(cmh => cmh.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                entity.HasIndex(cmh => cmh.OrdenCirugiaId);
                entity.HasIndex(cmh => cmh.MedicoId);
                entity.HasIndex(cmh => cmh.EspecialidadId);

                // ⚡ CRÍTICO: Índice único compuesto para evitar asignar al mismo médico dos veces en el mismo equipo quirúrgico
                entity.HasIndex(cmh => new { cmh.OrdenCirugiaId, cmh.MedicoId })
                    .IsUnique();

                // ⚡ Índice para reportes y liquidación de honorarios quirúrgicos por médico
                entity.HasIndex(cmh => new { cmh.MedicoId, cmh.MontoHonorarioUsd });
            });

           builder.Entity<TransferenciaReposicionStock>(entity =>
            {
                entity.ToTable("TransferenciasReposicionStock");
                entity.HasKey(t => t.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(t => t.Motivo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(t => t.Observaciones)
                    .HasMaxLength(1000);

                // ==========================================
                // Precisión de Cantidades (18, 4) y Fechas
                // ==========================================
                entity.Property(t => t.Cantidad)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(t => t.FechaTransferencia)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys - Restrict para trazabilidad contable)
                // ==========================================
                // 1. Relación con Insumo
                entity.HasOne(t => t.Insumo)
                    .WithMany()
                    .HasForeignKey(t => t.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 2. Relación con Sede Origen
                entity.HasOne(t => t.SedeOrigen)
                    .WithMany()
                    .HasForeignKey(t => t.SedeOrigenId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Relación con Sede Destino
                entity.HasOne(t => t.SedeDestino)
                    .WithMany()
                    .HasForeignKey(t => t.SedeDestinoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 4. Relación de Auditoría con Usuario Supervisor (Identity)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(t => t.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices de Auditoría
                // ==========================================
                entity.Property(t => t.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(t => t.InsumoId);
                entity.HasIndex(t => t.SedeOrigenId);
                entity.HasIndex(t => t.SedeDestinoId);
                entity.HasIndex(t => t.UsuarioIdentityId);
                entity.HasIndex(t => t.FechaTransferencia);

                // ⚡ Índice compuesto para consultar transferencias entre sedes por fecha
                entity.HasIndex(t => new { t.SedeOrigenId, t.SedeDestinoId, t.FechaTransferencia });
            });

            builder.Entity<CirugiaLog>(entity =>
            {
                entity.ToTable("CirugiaLogs");
                entity.HasKey(cl => cl.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(cl => cl.Evento)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(cl => cl.Detalle)
                    .HasMaxLength(1000);

                entity.Property(cl => cl.UsuarioId)
                    .HasMaxLength(100);

                entity.Property(cl => cl.Timestamp)
                    .IsRequired();

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación Inversa con Orden de Cirugía (Cascade)
                entity.HasOne(cl => cl.OrdenCirugia)
                    .WithMany(o => o.Logs)
                    .HasForeignKey(cl => cl.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación de Auditoría con Usuario (Identity)
                entity.HasOne<SistemaSatHospitalario.Infrastructure.Identity.Models.UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(cl => cl.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Configuración de Guids / MySQL e Índices
                // ==========================================
                entity.Property(cl => cl.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(cl => cl.OrdenCirugiaId);
                entity.HasIndex(cl => cl.UsuarioIdentityId);
                entity.HasIndex(cl => cl.Timestamp);

                // ⚡ CRÍTICO: Índice compuesto para consultar la línea de tiempo (Timeline) cronológica de la cirugía
                // (Ej: Where(l => l.OrdenCirugiaId == id).OrderBy(l => l.Timestamp))
                entity.HasIndex(cl => new { cl.OrdenCirugiaId, cl.Timestamp });
            });

           builder.Entity<RequisitoCirugia>(entity =>
            {
                entity.ToTable("RequisitosCirugia");
                entity.HasKey(r => r.Id);

                // ==========================================
                // Propiedades de Texto y Longitudes
                // ==========================================
                entity.Property(r => r.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(r => r.Descripcion)
                    .HasMaxLength(500);

                // ==========================================
                // Boleanos y Valores por Defecto
                // ==========================================
                entity.Property(r => r.EsActivo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(r => r.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // Mapeo de Colección Encapsulada con Backing-Field
                // ==========================================
                entity.HasMany(r => r.OrdenesRequisitos)
                    .WithOne(or => or.RequisitoCirugia)
                    .HasForeignKey(or => or.RequisitoCirugiaId)
                    .OnDelete(DeleteBehavior.Restrict); // No permite borrar un requisito del catálogo si está asociado a cirugías

                entity.Navigation(r => r.OrdenesRequisitos)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                // ==========================================
                // Índices de Rendimiento y Unicidad
                // ==========================================
                // ⚡ Índice único para evitar requisitos pre-operatorios duplicados (ej. 'Consentimiento Informado')
                entity.HasIndex(r => r.Nombre)
                    .IsUnique();

                // Índice para optimizar la carga del checklist pre-quirúrgico activo
                entity.HasIndex(r => r.EsActivo);


                entity.HasData(
                new RequisitoCirugia("Evaluación Cardiovascular / Riesgo Quirúrgico", "Informe de cardiología y electrocardiograma vigente.", true, SeedConstants.RequisitoId_Cardiovascular, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                new RequisitoCirugia("Exámenes Preoperatorios (Laboratorio)", "Hematología completa, TP, TPT, Glucemia, Urea, Creatinina y VIH/VDRL.", true, SeedConstants.RequisitoId_Laboratorio, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                new RequisitoCirugia("Consentimiento Informado Firmado", "Firma del paciente o familiar responsable para procedimiento quirúrgico y anestesia.", true, SeedConstants.RequisitoId_Consentimiento, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                new RequisitoCirugia("Ayuno Verificado (Mínimo 8 Horas)", "Verificación por enfermería de ayuno estricto.", true, SeedConstants.RequisitoId_Ayuno, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                new RequisitoCirugia("Valoración Anestésica", "Aprobación formal firmada por el médico anestesiólogo.", true, SeedConstants.RequisitoId_ValoracionAnestesica, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                new RequisitoCirugia("Reserva de Sangre / Hemoderivados", "Disponibilidad confirmada con Banco de Sangre (cuando aplique).", true, SeedConstants.RequisitoId_ReservaSangre, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                new RequisitoCirugia("Disponibilidad de Cama Postoperatoria (UCI / Hosp)", "Cama confirmada para el traslado post-quirúrgico.", true, SeedConstants.RequisitoId_CamaPostoperatoria, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            );
            });

            builder.Entity<OrdenCirugiaRequisito>(entity =>
            {
                entity.ToTable("OrdenCirugiaRequisitos");
                entity.HasKey(ocr => ocr.Id);

                // ==========================================
                // Propiedades de Texto y Boleanos
                // ==========================================
                entity.Property(ocr => ocr.VerificadoPor)
                    .HasMaxLength(100);

                entity.Property(ocr => ocr.Cumplido)
                    .IsRequired()
                    .HasDefaultValue(false);

                // ==========================================
                // Relaciones (Foreign Keys)
                // ==========================================
                // 1. Relación Inversa con Orden de Cirugía (Cascade)
                entity.HasOne(ocr => ocr.OrdenCirugia)
                    .WithMany(o => o.Requisitos)
                    .HasForeignKey(ocr => ocr.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relación con Catálogo Maestro de Requisitos (Restrict: No permite borrar requisito del catálogo si está en uso)
                entity.HasOne(ocr => ocr.RequisitoCirugia)
                    .WithMany(r => r.OrdenesRequisitos)
                    .HasForeignKey(ocr => ocr.RequisitoCirugiaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ==========================================
                // Índices y Restricción de Unicidad
                // ==========================================
                entity.HasIndex(ocr => ocr.OrdenCirugiaId);
                entity.HasIndex(ocr => ocr.RequisitoCirugiaId);
                entity.HasIndex(ocr => ocr.Cumplido);

                // ⚡ CRÍTICO: Índice único compuesto para evitar asociar el mismo requisito dos veces a la misma orden de cirugía
                entity.HasIndex(ocr => new { ocr.OrdenCirugiaId, ocr.RequisitoCirugiaId })
                    .IsUnique();
            });

        builder.Entity<CirugiaObservacionHistorial>(entity =>
            {
                entity.ToTable("CirugiaObservacionesHistorial");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Observacion)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.FechaRegistro)
                    .IsRequired();

                entity.Property(e => e.UsuarioRegistroId)
                    .IsRequired(false);

                // Relación con OrdenCirugia
                entity.HasOne(e => e.OrdenCirugia)
                    .WithMany() // Cambiar por .WithMany(x => x.ObservacionesHistorial) si la propiedad de navegación existe en OrdenCirugia
                    .HasForeignKey(e => e.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices recomendados para optimización de lecturas
                entity.HasIndex(e => e.OrdenCirugiaId);
                entity.HasIndex(e => e.UsuarioRegistroId);
            });

           builder.Entity<OrdenCompraInventario>(entity =>
            {
                entity.ToTable("OrdenesCompraInventario");

                entity.HasKey(o => o.Id);

                entity.Property(o => o.NumeroFactura)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(o => o.FechaEmision)
                    .IsRequired();

                entity.Property(o => o.MontoTotalUSD)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(o => o.Estado)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(o => o.Observaciones)
                    .HasMaxLength(1000);

                // Ignorar propiedades calculadas en memoria (3FN pura)
                entity.Ignore(o => o.TotalAbonadoUSD);
                entity.Ignore(o => o.SaldoPendienteUSD);

                // Relación con Proveedor
                entity.HasOne(o => o.Proveedor)
                    .WithMany()
                    .HasForeignKey(o => o.ProveedorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Pagos
                entity.HasMany(o => o.Pagos)
                    .WithOne(p => p.OrdenCompra)
                    .HasForeignKey(p => p.OrdenCompraId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Mapeo del backing field _pagos para la colección de solo lectura
                entity.Navigation(o => o.Pagos)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                // Índices recomendados
                entity.HasIndex(o => o.NumeroFactura);
                entity.HasIndex(o => o.Estado);
                entity.HasIndex(o => o.ProveedorId);
            });

            builder.Entity<PagoProveedor>(entity =>
            {
                entity.ToTable("PagosProveedores");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.FechaPago)
                    .IsRequired();

                entity.Property(p => p.MontoAbonadoUSD)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(p => p.TasaCambio)
                    .IsRequired()
                    .HasPrecision(18, 4);

                // Se ignora el cálculo derivado para cumplir 3FN
                entity.Ignore(p => p.MontoAbonadoBs);

                entity.Property(p => p.MetodoPago)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.Referencia)
                    .HasMaxLength(100);

                entity.Property(p => p.UsuarioIdentityId)
                    .IsRequired(false);

                entity.Property(p => p.Observaciones)
                    .HasMaxLength(1000);

                // Relación con OrdenCompraInventario
                entity.HasOne(p => p.OrdenCompra)
                    .WithMany(o => o.Pagos)
                    .HasForeignKey(p => p.OrdenCompraId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices de optimización de lecturas
                entity.HasIndex(p => p.OrdenCompraId);
                entity.HasIndex(p => p.FechaPago);
                entity.HasIndex(p => p.UsuarioIdentityId);
            });

            builder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("Proveedores");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.RIF)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(p => p.RazonSocial)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Direccion)
                    .HasMaxLength(500);

                entity.Property(p => p.Telefono)
                    .HasMaxLength(50);

                entity.Property(p => p.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(p => p.FechaRegistro)
                    .IsRequired();

                // Índice único para RIF para prevenir duplicados fiscales
                entity.HasIndex(p => p.RIF)
                    .IsUnique();

                // Índices de optimización de búsquedas
                entity.HasIndex(p => p.RazonSocial);
                entity.HasIndex(p => p.Activo);
            });
        }
    }
}