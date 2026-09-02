using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Common;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

using SistemaSatHospitalario.Core.Domain.Common;
using SistemaSatHospitalario.Core.Domain.Constants;

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
        public DbSet<OrdenRX> OrdenesRX { get; set; } // TPH o TPT

        public DbSet<TurnoMedico> TurnosMedicos { get; set; }
        public DbSet<IncidenciaHorario> IncidenciasHorario { get; set; }
        public DbSet<RegistroAuditoriaIncidencia> RegistrosAuditoriaIncidencia { get; set; }

        public DbSet<CuentaServicios> CuentasServicios { get; set; }
        public DbSet<DetalleServicioCuenta> DetallesServicioCuenta { get; set; }
        public DbSet<CitaMedica> CitasMedicas { get; set; }
        public DbSet<EstadoCitaMedica> EstadosCitaMedica { get; set; }
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
                if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                {
                    entry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }
            }
            foreach (var entry in ChangeTracker.Entries<CirugiaObservacionHistorial>())
            {
                if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                {
                    entry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }
            }
        }

        private void EnforceMovimientoInsumoImmutability()
        {
            foreach (var entry in ChangeTracker.Entries<MovimientoInsumo>())
            {
                if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Modified || entry.State == Microsoft.EntityFrameworkCore.EntityState.Deleted)
                {
                    throw new InvalidOperationException("Los movimientos de insumos de inventario son inmutables y no se pueden modificar ni eliminar.");
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // MySQL no soporta esquemas, se ignora para compatibilidad multi-proveedor
            // builder.HasDefaultSchema("Admision");

            // [PHASE-5] Ignore Domain Events during persistence (V14.1 Senior Patch)
            // Ensures purely in-memory event handling and prevents "Missing PK" EF errors.
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
                entity.Property(c => c.TotalCobrado).HasPrecision(18, 2);
                entity.Property(c => c.Diferencia).HasPrecision(18, 2);
                entity.Property(c => c.DeclaracionCierreJson).HasColumnType("longtext");
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
                entity.Property(r => r.TasaCambioDia).HasPrecision(18, 4);
                entity.Property(r => r.NumeroComprobante).HasMaxLength(50);
                
                entity.HasOne(r => r.CajaDiaria)
                      .WithMany()
                      .HasForeignKey(r => r.CajaDiariaId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(r => r.CuentaServicio)
                      .WithMany()
                      .HasForeignKey(r => r.CuentaServicioId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Ignore(r => r.Estado);
            });

            builder.Entity<DetallePago>(entity =>
            {
                entity.ToTable("DetallesPago");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.MontoAbonadoMoneda).HasPrecision(18, 2);
                entity.Property(d => d.EquivalenteAbonadoBase).HasPrecision(18, 2);
                entity.Property(d => d.TasaCambioAplicada).HasPrecision(18, 4);

                entity.HasOne(d => d.ReciboFactura)
                      .WithMany(r => r.DetallesPago)
                      .HasForeignKey(d => d.ReciboFacturaId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índice para reporte de ingresos (Fase 7)
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
                entity.Property(h => h.CategoriaServicio).IsRequired().HasMaxLength(50);
                entity.HasOne(h => h.MedicoDefault).WithMany().HasForeignKey(h => h.MedicoDefaultId).OnDelete(DeleteBehavior.SetNull);
                entity.HasIndex(h => h.CategoriaServicio).IsUnique();
            });

            builder.Entity<LogAsignacionHonorario>(entity =>
            {
                entity.ToTable("LogsAsignacionHonorario");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.TipoAccion).IsRequired().HasMaxLength(50);
                entity.HasIndex(l => l.FechaAccion);
                entity.HasIndex(l => l.DetalleServicioId);
            });

            builder.Entity<DetalleServicioCuenta>()
                .HasOne<Medico>()
                .WithMany()
                .HasForeignKey(d => d.MedicoResponsableId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<DetalleServicioCuenta>()
                .HasOne(d => d.DetallePadre)
                .WithMany()
                .HasForeignKey(d => d.DetallePadreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServicioClinico>()
                .HasOne(s => s.ServicioInforme)
                .WithMany()
                .HasForeignKey(s => s.ServicioInformeId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<PacienteAdmision>(entity =>
            {
                entity.ToTable("PacientesAdmision");
                entity.HasKey(p => p.Id);
                // Guid generado por el sistema nuevo (V11.0 Sync Pro)
                entity.HasIndex(p => p.IdPacienteLegacy).IsUnique();
                entity.HasIndex(p => p.CedulaPasaporte).IsUnique();
            });

            builder.Entity<OrdenDeServicio>(entity =>
            {
                entity.ToTable("OrdenesDeServicio");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.TotalCobrado).HasPrecision(18, 2);
                entity.Property(o => o.EstadoFacturacion).HasConversion<int>();

                entity.HasOne(o => o.Paciente)
                      .WithMany(p => p.Ordenes)
                      .HasForeignKey(o => o.PacienteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // TPH Configuration for OrdenRX
            builder.Entity<OrdenRX>().HasBaseType<OrdenDeServicio>();

            builder.Entity<TurnoMedico>(entity => {
                entity.ToTable("TurnosMedicos");
                entity.HasKey(t => t.Id);
            });

            builder.Entity<IncidenciaHorario>(entity => {
                entity.ToTable("IncidenciasHorario");
                entity.HasKey(i => i.Id);
            });

            builder.Entity<RegistroAuditoriaIncidencia>(entity => {
                entity.ToTable("RegistroAuditoriaIncidencias");
                entity.HasKey(r => r.Id);
            });

            builder.Entity<CuentaServicios>(entity =>
            {
                entity.ToTable("CuentasServicios");
                entity.HasKey(c => c.Id);
                
                entity.HasMany(c => c.Detalles)
                      .WithOne(d => d.CuentaServicio)
                      .HasForeignKey(d => d.CuentaServicioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Paciente)
                      .WithMany()
                      .HasForeignKey(c => c.PacienteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CuentaPrincipal)
                      .WithMany()
                      .HasForeignKey(c => c.CuentaPrincipalId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Triages)
                      .WithOne(t => t.CuentaServicio)
                      .HasForeignKey(t => t.CuentaServicioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Valoraciones)
                      .WithOne(v => v.CuentaServicio)
                      .HasForeignKey(v => v.CuentaServicioId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(c => c.AreaClinica)
                      .WithMany()
                      .HasForeignKey(c => c.AreaClinicaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CamaRetenida)
                      .WithMany()
                      .HasForeignKey(c => c.CamaRetenidaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Medico)
                      .WithMany()
                      .HasForeignKey(c => c.MedicoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.SubAreaClinica)
                      .HasMaxLength(100);

                // Índice para búsqueda por fecha (Fase 7)
                entity.HasIndex(c => c.FechaCarga);
            });

            builder.Entity<DetalleServicioCuenta>(entity =>
            {
                entity.ToTable("DetallesServicioCuenta");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Precio).HasPrecision(18, 2);
                entity.Property(d => d.PrecioCatalogoHistorico).HasPrecision(18, 2);
                entity.Property(d => d.IncluidoEnTarifaBase).IsRequired();
                entity.Property(d => d.Cantidad).HasPrecision(18, 4);
                entity.HasOne(d => d.AreaClinica)
                      .WithMany()
                      .HasForeignKey(d => d.AreaClinicaId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.TipoServicioNav)
                      .WithMany()
                      .HasForeignKey(d => d.TipoServicioId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 3FN: FKs lógicas a Usuarios (Identity, PK Guid). Sin restricción FK física
                // porque la tabla Usuarios vive en el contexto de Identity.
                entity.Property(d => d.UsuarioCargaId).HasColumnType("char(36)");
                entity.Property(d => d.UsuarioTecnicoId).HasColumnType("char(36)");
                entity.HasIndex(d => d.UsuarioCargaId);
                entity.HasIndex(d => d.UsuarioTecnicoId);
            });

            builder.Entity<CitaMedica>(entity =>
            {
                entity.ToTable("CitasMedicas");
                entity.HasKey(c => c.Id);

                // Índice para búsqueda por fecha (Fase 7)
                entity.HasIndex(c => c.HoraPautada);

                entity.HasOne(c => c.AreaClinica)
                      .WithMany()
                      .HasForeignKey(c => c.AreaClinicaId)
                      .OnDelete(DeleteBehavior.SetNull);

                // 3FN: FK al catálogo de estados de cita
                entity.HasOne(c => c.EstadoNav)
                      .WithMany()
                      .HasForeignKey(c => c.EstadoId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(c => c.EstadoId);
            });

            // 3FN: Catálogo de estados de cita médica
            builder.Entity<EstadoCitaMedica>(entity =>
            {
                entity.ToTable("EstadosCitaMedica");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Codigo).IsUnique();

                entity.HasData(
                    new EstadoCitaMedica(SistemaSatHospitalario.Core.Domain.Constants.EstadoCitaConstants.PendienteId, "PENDIENTE", "Pendiente"),
                    new EstadoCitaMedica(SistemaSatHospitalario.Core.Domain.Constants.EstadoCitaConstants.ConfirmadaId, "CONFIRMADA", "Confirmada"),
                    new EstadoCitaMedica(SistemaSatHospitalario.Core.Domain.Constants.EstadoCitaConstants.AtendidaId, "ATENDIDA", "Atendida"),
                    new EstadoCitaMedica(SistemaSatHospitalario.Core.Domain.Constants.EstadoCitaConstants.CanceladaId, "CANCELADA", "Cancelada")
                );
            });

            builder.Entity<Medico>(entity =>
            {
                entity.ToTable("Medicos");
                entity.HasKey(m => m.Id);

                entity.HasOne(m => m.Especialidad)
                      .WithMany()
                      .HasForeignKey(m => m.EspecialidadId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(m => m.HonorarioBase).HasPrecision(18, 2);
            });

            builder.Entity<ServicioClinico>(entity =>
            {
                entity.ToTable("ServiciosClinicos");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.PrecioBase).HasPrecision(18, 2);

                entity.HasOne(s => s.Especialidad)
                      .WithMany()
                      .HasForeignKey(s => s.EspecialidadId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(s => s.HonorariumCategory).HasMaxLength(50);
                entity.Property(s => s.UnidadMedida).HasMaxLength(50);
            });

            builder.Entity<TriageEnfermeria>(entity =>
            {
                entity.ToTable("TriagesEnfermeria");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.TensionArterial).HasMaxLength(20).IsRequired();
                entity.Property(t => t.MotivoConsulta).HasMaxLength(500).IsRequired();
                entity.Property(t => t.Temperatura).HasPrecision(4, 2);
                entity.Property(t => t.UsuarioRegistro).HasMaxLength(100).IsRequired();
                entity.HasIndex(t => t.FechaRegistro);
            });

            builder.Entity<ValoracionFisica>(entity =>
            {
                entity.ToTable("ValoracionesFisicas");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.EstadoConciencia).HasMaxLength(50).IsRequired();
                entity.Property(v => v.ViaAerea).HasMaxLength(50).IsRequired();
                entity.Property(v => v.Ventilacion).HasMaxLength(50).IsRequired();
                entity.Property(v => v.Pulso).HasMaxLength(50).IsRequired();
                entity.Property(v => v.PielMucosas).HasMaxLength(50).IsRequired();
                entity.Property(v => v.LlenadoCapilar).HasMaxLength(50).IsRequired();
                entity.Property(v => v.Pupilas).HasMaxLength(50).IsRequired();
                entity.Property(v => v.UsuarioRegistro).HasMaxLength(100).IsRequired();
                entity.HasIndex(v => v.FechaRegistro);
            });

            builder.Entity<HorarioAtencionMedico>(entity =>
            {
                entity.ToTable("HorariosAtencionMedicos");
                entity.HasKey(h => h.Id);

                entity.HasOne<Medico>()
                      .WithMany()
                      .HasForeignKey(h => h.MedicoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PrecioServicioConvenio>(entity =>
            {
                entity.ToTable("PreciosServicioConvenio");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.PrecioDiferencial).HasPrecision(18, 2);

                entity.HasOne(p => p.Servicio)
                      .WithMany()
                      .HasForeignKey(p => p.ServicioClinicoId);

                entity.HasOne(p => p.Convenio)
                      .WithMany()
                      .HasForeignKey(p => p.SeguroConvenioId);
            });

            builder.Entity<ConvenioPerfilPrecio>(entity =>
            {
                entity.ToTable("ConvenioPerfilPrecios");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.PrecioHNL).HasPrecision(18, 2);
                entity.Property(c => c.PrecioUSD).HasPrecision(18, 2);

                entity.HasOne(c => c.Convenio)
                      .WithMany()
                      .HasForeignKey(c => c.SeguroConvenioId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Índice para búsqueda rápida por convenio y perfil
                entity.HasIndex(c => new { c.SeguroConvenioId, c.PerfilId }).IsUnique();
            });

            builder.Entity<CuentaPorCobrar>(entity =>
            {
                entity.ToTable("CuentasPorCobrar");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.MontoTotalBase).HasPrecision(18, 2);
                entity.Property(c => c.MontoPagadoBase).HasPrecision(18, 2);
                entity.Ignore(c => c.SaldoPendienteBase);
                // 3FN: FK lógica a Usuarios (Identity, PK Guid). Sin restricción FK física
                // porque la tabla Usuarios vive en el contexto de Identity.
                entity.Property(c => c.UsuarioAuditoriaId).HasColumnType("char(36)");
                entity.HasIndex(c => c.UsuarioAuditoriaId);

                entity.HasOne(c => c.Cuenta)
                      .WithMany()
                      .HasForeignKey(c => c.CuentaServicioId);

                // [V12.8] Relación 1:N con ítems de garantía prendaria
                entity.HasMany(c => c.GarantiasItems)
                      .WithOne(g => g.CuentaPorCobrar)
                      .HasForeignKey(g => g.CuentaPorCobrarId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // [V12.8] Tabla de ítems de garantía prendaria
            builder.Entity<GarantiaItem>(entity =>
            {
                entity.ToTable("GarantiasItems");
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Descripcion).IsRequired().HasMaxLength(500);
                entity.Property(g => g.ValorEstimado).HasPrecision(18, 2);
                entity.HasIndex(g => g.CuentaPorCobrarId);
            });

            builder.Entity<CompromisoPago>(entity =>
            {
                entity.ToTable("CompromisosPago");
                entity.HasKey(c => c.Id);
                // 3FN: FK lógica a Usuarios (Identity, PK Guid). Sin restricción FK física
                // porque la tabla Usuarios vive en el contexto de Identity.
                entity.Property(c => c.UsuarioCreacionId).HasColumnType("char(36)");
                entity.HasIndex(c => c.UsuarioCreacionId);

                // 3FN: FK al catálogo de motivos de autorización
                entity.HasOne(c => c.MotivoAutorizacion)
                      .WithMany()
                      .HasForeignKey(c => c.MotivoAutorizacionId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(c => c.MotivoAutorizacionId);
            });

            // 3FN: Catálogo de motivos de autorización/omisión
            builder.Entity<MotivoAutorizacion>(entity =>
            {
                entity.ToTable("MotivosAutorizacion");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.Property(m => m.Nombre).IsRequired().HasMaxLength(150);
                entity.HasIndex(m => m.Nombre).IsUnique();

                entity.HasData(
                    new { Id = 1, Nombre = "Autorizado por Dirección Médica", Activo = true },
                    new { Id = 2, Nombre = "Exoneración por Presidencia", Activo = true },
                    new { Id = 3, Nombre = "Convenio Institucional", Activo = true }
                );
            });

            builder.Entity<TasaCambio>(entity =>
            {
                entity.ToTable("TasaCambio");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Monto).HasPrecision(18, 4);
                entity.Property(t => t.Activo).IsRequired(); // Mapeo implícito a TINYINT(1)/BIT
            });

            builder.Entity<ErrorTicket>(entity =>
            {
                entity.ToTable("ErrorTickets");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestPath).HasMaxLength(500);
                entity.Property(e => e.MetodoHTTP).HasMaxLength(10);
            });

            builder.Entity<Especialidad>(entity =>
            {
                entity.ToTable("Especialidades");
                entity.HasKey(e => e.Id);
            });

            builder.Entity<ReservaTemporal>(entity =>
            {
                entity.ToTable("ReservasTemporales");
                entity.HasKey(r => r.Id);
                entity.HasIndex(r => new { r.MedicoId, r.HoraPautada }).IsUnique();
            });

            builder.Entity<BloqueoHorario>(entity =>
            {
                entity.ToTable("BloqueosHorarios");
                entity.HasKey(b => b.Id);
                entity.HasIndex(b => new { b.MedicoId, b.HoraPautada }).IsUnique();
            });

            builder.Entity<ConfiguracionGeneral>(entity =>
            {
                entity.ToTable("ConfiguracionGeneral");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Iva).HasPrecision(5, 2);
                entity.Property(c => c.LogoBase64).HasColumnType("longtext");
            });

            builder.Entity<ServicioSugerencia>(entity =>
            {
                entity.ToTable("serviciossugerencias"); // Obligatorio minúscula por restricción MySQL Cloud
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.ServicioOrigen)
                      .WithMany(sc => sc.Sugerencias)
                      .HasForeignKey(s => s.ServicioOrigenId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.ServicioSugerido)
                      .WithMany()
                      .HasForeignKey(s => s.ServicioSugeridoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<LogAuditoriaPrecio>(entity =>
            {
                entity.ToTable("AuditLogsPrecios");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.PrecioOriginal).HasPrecision(18, 2);
                entity.Property(a => a.PrecioModificado).HasPrecision(18, 2);
                entity.Property(a => a.UsuarioOperador).IsRequired().HasMaxLength(100);
                entity.Property(a => a.AutorizadoPor).IsRequired().HasMaxLength(100);
                entity.Property(a => a.DescripcionServicio).IsRequired().HasMaxLength(500);
            });

            builder.Entity<OrdenImagen>(entity =>
            {
                entity.ToTable("OrdenesImagenes");
                entity.HasKey(o => o.Id);
                entity.HasIndex(o => o.Estado);
                entity.HasIndex(o => o.TipoServicio);

                entity.HasOne(o => o.Paciente)
                      .WithMany()
                      .HasForeignKey(o => o.PacienteId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.MedicoSolicitante)
                      .WithMany()
                      .HasForeignKey(o => o.MedicoSolicitanteId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Moneda>(entity =>
            {
                entity.ToTable("Monedas");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedNever();
                entity.Property(m => m.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(m => m.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Simbolo).IsRequired().HasMaxLength(10);

                entity.HasData(
                    new Moneda(1, "USD", "Dólar", "$", true),
                    new Moneda(2, "VES", "Bolívar", "Bs.", false),
                    new Moneda(3, "EUR", "Euro", "€", false),
                    new Moneda(4, "COP", "Peso Colombiano", "COP$", false),
                    new Moneda(5, "ARS", "Peso Argentino", "ARS$", false)
                );
            });

            builder.Entity<CatalogoMetodoPago>(entity =>
            {
                entity.ToTable("CatalogoMetodosPago");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Valor).IsRequired().HasMaxLength(100);
                entity.Property(c => c.GrupoMoneda).HasDefaultValue(1);
                entity.HasIndex(c => c.Valor).IsUnique();

                entity.HasOne(c => c.Moneda)
                      .WithMany()
                      .HasForeignKey(c => c.GrupoMoneda)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<DocumentLog>(entity =>
            {
                entity.ToTable("DocumentLogs");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.DocumentType).IsRequired().HasMaxLength(100);
                entity.Property(d => d.ReferenceId).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Action).IsRequired().HasMaxLength(100);
                entity.Property(d => d.UserId).IsRequired().HasMaxLength(100);
                entity.Property(d => d.UserName).IsRequired().HasMaxLength(200);
                entity.HasIndex(d => d.ReferenceId);
                entity.HasIndex(d => d.Timestamp);
            });

            builder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
                entity.Property(n => n.Message).IsRequired().HasMaxLength(500);
                entity.Property(n => n.Type).IsRequired().HasMaxLength(50);
                entity.Property(n => n.TargetUserId).HasMaxLength(100);
                entity.Property(n => n.TargetRole).HasMaxLength(100);
                entity.HasIndex(n => n.TargetUserId);
                entity.HasIndex(n => n.TargetRole);
                entity.HasIndex(n => n.Timestamp);
            });

            builder.Entity<HonorariumMappingRule>(entity =>
            {
                entity.ToTable("HonorariumMappingRules");
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Pattern).IsRequired().HasMaxLength(100);
                entity.Property(h => h.Category).IsRequired().HasMaxLength(50);
                entity.HasIndex(h => h.Priority);
                entity.HasIndex(h => h.IsActive);
            });

            builder.Entity<HonorarioMedicoServicio>(entity =>
            {
                entity.ToTable("HonorariosMedicosServicios");
                entity.HasKey(h => h.Id);
                entity.Property(h => h.MontoHonorario).HasPrecision(18, 2);
                entity.HasOne(h => h.Servicio)
                      .WithMany(s => s.HonorariosMedicos)
                      .HasForeignKey(h => h.ServicioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(h => h.Medico)
                      .WithMany()
                      .HasForeignKey(h => h.MedicoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<HistorialModificacionCuenta>(entity =>
            {
                entity.ToTable("HistorialModificacionCuentas");
                entity.HasKey(h => h.Id);
                entity.Property(h => h.TotalAnteriorUSD).HasPrecision(18, 2);
                entity.Property(h => h.TotalNuevoUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboTotalAnteriorUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboTotalNuevoUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboVueltoAnteriorUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboVueltoNuevoUSD).HasPrecision(18, 2);
                entity.Property(h => h.ReciboPagadoUSD).HasPrecision(18, 2);
                entity.Property(h => h.CxCSaldoAnteriorUSD).HasPrecision(18, 2);
                entity.Property(h => h.CxCSaldoNuevoUSD).HasPrecision(18, 2);
                entity.Property(h => h.DetalleServiciosCambiosJson).HasColumnType("longtext");
                entity.Property(h => h.Usuario).IsRequired().HasMaxLength(100);

                entity.HasIndex(h => h.CuentaServicioId);
                entity.HasIndex(h => h.FechaModificacion);
            });

            builder.Entity<HistorialModificacionCuentaDetalle>(entity =>
            {
                entity.ToTable("HistorialModificacionCuentaDetalles");
                entity.HasKey(h => h.Id);
                entity.Property(h => h.PrecioAnterior).HasPrecision(18, 2);
                entity.Property(h => h.PrecioNuevo).HasPrecision(18, 2);
                entity.Property(h => h.HonorarioAnterior).HasPrecision(18, 2);
                entity.Property(h => h.HonorarioNuevo).HasPrecision(18, 2);
                entity.Property(h => h.CantidadAnterior).HasPrecision(18, 2);
                entity.Property(h => h.CantidadNueva).HasPrecision(18, 2);

                entity.HasOne(h => h.HistorialModificacionCuenta)
                      .WithMany()
                      .HasForeignKey(h => h.HistorialModificacionCuentaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.DetalleServicio)
                      .WithMany()
                      .HasForeignKey(h => h.DetalleServicioId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(h => h.HistorialModificacionCuentaId);
                entity.HasIndex(h => h.DetalleServicioId);
            });

            builder.Entity<Insumo>(entity =>
            {
                entity.ToTable("Insumos");
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(i => i.Nombre).IsRequired().HasMaxLength(200);
                entity.Ignore(i => i.StockActual);
                entity.Property(i => i.UnidadMedidaBase).HasConversion<string>().IsRequired().HasMaxLength(20);
                entity.Property(i => i.CostoUnitarioBaseUSD).HasPrecision(18, 4);
                entity.Property(i => i.PermiteFraccionamiento).IsRequired().HasDefaultValue(true);
                entity.Property(i => i.Categoria).HasMaxLength(50).HasDefaultValue("Medicamento");
                entity.Property(i => i.IsDeleted).IsRequired().HasDefaultValue(false);
                entity.Property(i => i.FechaInactivacion);
                entity.Property(i => i.OcultoEnTraslados).IsRequired().HasDefaultValue(false);
                entity.Property(i => i.ReactivosCombinados).HasMaxLength(500);
                entity.Property(i => i.Indicaciones);
                entity.Property(i => i.FechaVencimiento);
                entity.HasIndex(i => i.Codigo).IsUnique();
            });

            builder.Entity<CategoriaInsumo>(entity =>
            {
                entity.ToTable("CategoriasInsumo");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Codigo).HasMaxLength(50);
                entity.HasIndex(c => c.Nombre).IsUnique();

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
                entity.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
                entity.HasIndex(p => p.Nombre).IsUnique();
            });

            builder.Entity<InsumoPrincipioActivo>(entity =>
            {
                entity.ToTable("InsumosPrincipiosActivos");
                entity.HasKey(ipa => new { ipa.InsumoId, ipa.PrincipioActivoId });
                entity.Property(ipa => ipa.Concentracion).HasMaxLength(100);

                entity.HasOne(ipa => ipa.Insumo)
                      .WithMany(i => i.PrincipiosActivos)
                      .HasForeignKey(ipa => ipa.InsumoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ipa => ipa.PrincipioActivo)
                      .WithMany(pa => pa.Insumos)
                      .HasForeignKey(ipa => ipa.PrincipioActivoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ServicioInsumoReceta>(entity =>
            {
                entity.ToTable("ServiciosInsumoRecetas");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.UnidadMedidaConsumo).HasConversion<string>().IsRequired().HasMaxLength(20);
                entity.Property(r => r.Cantidad).HasPrecision(18, 4);

                entity.HasOne(r => r.ServicioClinico)
                      .WithMany()
                      .HasForeignKey(r => r.ServicioClinicoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Insumo)
                      .WithMany()
                      .HasForeignKey(r => r.InsumoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ConsumoServicioRealizado>(entity =>
            {
                entity.ToTable("ConsumosServiciosRealizados");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.CantidadConsumidaBase).HasPrecision(18, 4);
                entity.Property(c => c.CostoTotalUSD).HasPrecision(18, 4);

                entity.HasOne(c => c.DetalleServicioCuenta)
                      .WithMany()
                      .HasForeignKey(c => c.DetalleServicioCuentaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Insumo)
                      .WithMany()
                      .HasForeignKey(c => c.InsumoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<MovimientoInsumo>(entity =>
            {
                entity.ToTable("MovimientosInsumo");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.TipoMovimiento).IsRequired().HasMaxLength(50);
                entity.Property(m => m.CantidadBase).HasPrecision(18, 4);
                entity.Property(m => m.UnidadMedidaOriginal).HasConversion<string>().IsRequired().HasMaxLength(20);
                entity.Property(m => m.CantidadOriginal).HasPrecision(18, 4);
                entity.Property(m => m.Usuario).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Motivo).HasMaxLength(500);

                entity.HasOne(m => m.Insumo)
                      .WithMany()
                      .HasForeignKey(m => m.InsumoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Sede)
                      .WithMany()
                      .HasForeignKey(m => m.SedeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CierreInventario>(entity =>
            {
                entity.ToTable("CierresInventario");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Usuario).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Observaciones).HasMaxLength(1000);

                entity.HasOne(c => c.Sede)
                      .WithMany()
                      .HasForeignKey(c => c.SedeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CierreInventarioDetalle>(entity =>
            {
                entity.ToTable("CierresInventarioDetalles");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.StockTeoricoBase).HasPrecision(18, 4);
                entity.Property(d => d.StockRealBase).HasPrecision(18, 4);
                entity.Property(d => d.CostoBaseUSD).HasPrecision(18, 4);

                entity.HasOne(d => d.CierreInventario)
                      .WithMany(c => c.Detalles)
                      .HasForeignKey(d => d.CierreInventarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Insumo)
                      .WithMany()
                      .HasForeignKey(d => d.InsumoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Sede>(entity =>
            {
                entity.ToTable("Sedes");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(150);
                entity.HasIndex(s => s.Codigo).IsUnique();

                entity.HasData(
                    new Sede("SEDE-PRINCIPAL", "Almacén Principal / Farmacia Central", true, SeedConstants.SedeId_Principal),
                    new Sede("SEDE-EMG", "Depósito Emergencia", false, SeedConstants.SedeId_Emergencia),
                    new Sede("SEDE-HOSP", "Depósito Hospitalización", false, SeedConstants.SedeId_Hospitalizacion),
                    new Sede("SEDE-UCI", "Depósito UCI", false, SeedConstants.SedeId_UCI),
                    new Sede("SEDE-CIRUGIA", "Quirófano / Pabellón Central", false, SeedConstants.SedeId_Cirugia)
                );
            });

            builder.Entity<AreaClinica>(entity =>
            {
                entity.ToTable("AreasClinicas");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(a => a.Estado).IsRequired();
                entity.Property(a => a.EsAreaAdmision).IsRequired();
                entity.HasOne(a => a.Sede)
                      .WithMany(s => s.AreasClinicas)
                      .HasForeignKey(a => a.SedeId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(a => a.ServicioTarifaBase)
                      .WithMany()
                      .HasForeignKey(a => a.ServicioTarifaBaseId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(a => new { a.SedeId, a.Codigo }).IsUnique();

                entity.HasData(
                    new AreaClinica(SeedConstants.SedeId_Emergencia, "BOX-1", "Box Emergencia 1", true, null, SeedConstants.AreaId_Emergencia),
                    new AreaClinica(SeedConstants.SedeId_Hospitalizacion, "HAB-101", "Habitación 101", false, null, SeedConstants.AreaId_Hospitalizacion),
                    new AreaClinica(SeedConstants.SedeId_UCI, "UCI-1", "Cama UCI 1", false, null, SeedConstants.AreaId_UCI),
                    new AreaClinica(SeedConstants.SedeId_Principal, "FARMACIA", "Farmacia Central", false, null, SeedConstants.AreaId_Farmacia),
                    new AreaClinica(SeedConstants.SedeId_Principal, "LABORATORIO", "Laboratorio Central", false, null, SeedConstants.AreaId_Laboratorio),
                    new AreaClinica(SeedConstants.SedeId_Cirugia, "QX-1", "Quirófano 1 (Cirugía Mayor)", false, null, SeedConstants.AreaId_Cirugia)
                );
            });

            builder.Entity<ServicioIncluidoArea>(entity =>
            {
                entity.ToTable("ServiciosIncluidosAreas");
                entity.HasKey(s => new { s.AreaClinicaId, s.ServicioClinicoId });

                entity.HasOne(s => s.AreaClinica)
                      .WithMany()
                      .HasForeignKey(s => s.AreaClinicaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.ServicioClinico)
                      .WithMany()
                      .HasForeignKey(s => s.ServicioClinicoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<InsumoCirugiaPaciente>(entity =>
            {
                entity.ToTable("InsumosCirugiasPacientes");
                entity.HasKey(i => i.Id);

                entity.HasOne(i => i.CuentaServicio)
                      .WithMany()
                      .HasForeignKey(i => i.CuentaServicioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Insumo)
                      .WithMany()
                      .HasForeignKey(i => i.InsumoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(i => new { i.CuentaServicioId, i.InsumoId });

                entity.Property(i => i.CantidadEntregada).HasPrecision(18, 4);
                entity.Property(i => i.CantidadDevuelta).HasPrecision(18, 4);
            });

            builder.Entity<StockSede>(entity =>
            {
                entity.ToTable("StocksSede");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.StockActual).HasPrecision(18, 4);
                entity.Property(s => s.StockMinimo).HasPrecision(18, 4);
                entity.Property(s => s.StockMaximo).HasPrecision(18, 4);
                entity.Property(s => s.RowVersion).IsRowVersion();

                entity.HasOne(s => s.Insumo)
                      .WithMany(i => i.StocksPorSede)
                      .HasForeignKey(s => s.InsumoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Sede)
                      .WithMany()
                      .HasForeignKey(s => s.SedeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(s => new { s.SedeId, s.InsumoId }).IsUnique();
            });

            builder.Entity<MovimientoInsumo>(entity =>
            {
                entity.ToTable("MovimientosInsumo");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.TipoMovimiento).HasConversion<int>();
                entity.Property(m => m.CantidadBase).HasPrecision(18, 4);
                entity.Property(m => m.CantidadOriginal).HasPrecision(18, 4);
            });

            builder.Entity<PedidoInterSede>(entity =>
            {
                entity.ToTable("PedidosInterSede");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Correlativo).IsRequired().HasMaxLength(50);
                entity.HasIndex(p => p.Correlativo).IsUnique();

                entity.HasOne(p => p.SedeSolicitante)
                      .WithMany()
                      .HasForeignKey(p => p.SedeSolicitanteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.SedeProveedora)
                      .WithMany()
                      .HasForeignKey(p => p.SedeProveedoraId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PedidoInterSedeDetalle>(entity =>
            {
                entity.ToTable("PedidosInterSedeDetalles");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.CantidadSolicitada).HasPrecision(18, 4);
                entity.Property(d => d.CantidadDespachada).HasPrecision(18, 4);
                entity.Property(d => d.CantidadRecibida).HasPrecision(18, 4);
                entity.Property(d => d.ObservacionDespacho).HasMaxLength(500);

                entity.HasOne(d => d.PedidoInterSede)
                      .WithMany(p => p.Detalles)
                      .HasForeignKey(d => d.PedidoInterSedeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Insumo)
                      .WithMany()
                      .HasForeignKey(d => d.InsumoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<DetalleServicioMedicoResponsable>(entity =>
            {
                entity.ToTable("DetallesServiciosMedicosResponsables");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Rol).IsRequired().HasMaxLength(100);
                entity.Property(d => d.MontoHonorario).HasPrecision(18, 2);

                entity.HasOne(d => d.DetalleServicioCuenta)
                      .WithMany(dsc => dsc.MedicosResponsables)
                      .HasForeignKey(d => d.DetalleServicioCuentaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Medico)
                      .WithMany()
                      .HasForeignKey(d => d.MedicoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ServicioClinico>(entity =>
            {
                entity.ToTable("ServiciosClinicos");
                entity.HasOne<TipoServicio>()
                      .WithMany()
                      .HasForeignKey(s => s.TipoServicioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<TipoServicio>(entity =>
            {
                entity.ToTable("TiposServicio");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedNever();
                entity.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Codigo).IsRequired().HasMaxLength(50);

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
                entity.ToTable("OrdenesCirugia");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.DescripcionCirugia).IsRequired().HasMaxLength(500);
                entity.Property(o => o.PrecioBaseUsd).HasPrecision(18, 2);
                entity.Property(o => o.PrecioDerechoSalaUsd).HasPrecision(18, 2);
                entity.Property(o => o.SalaQuirofano).HasMaxLength(100);
                entity.Property(o => o.ModalidadAnestesia).HasMaxLength(100);
                entity.Property(o => o.Estado).IsRequired().HasMaxLength(50);
                entity.Property(o => o.MotivoCancelacion).HasMaxLength(500);
                entity.Property(o => o.UsuarioCreacion).IsRequired().HasMaxLength(100);

                entity.HasOne(o => o.CuentaServicio)
                      .WithMany()
                      .HasForeignKey(o => o.CuentaServicioId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Paciente)
                      .WithMany()
                      .HasForeignKey(o => o.PacienteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Medico)
                      .WithMany()
                      .HasForeignKey(o => o.MedicoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.SedeQuirofano)
                      .WithMany()
                      .HasForeignKey(o => o.SedeQuirofanoId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(o => o.AreaClinica)
                      .WithMany()
                      .HasForeignKey(o => o.AreaClinicaId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(o => o.AreaClinicaOrigen)
                      .WithMany()
                      .HasForeignKey(o => o.AreaClinicaOrigenId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(o => o.SedeOrigen)
                      .WithMany()
                      .HasForeignKey(o => o.SedeOrigenId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(o => o.Logs)
                      .WithOne(l => l.OrdenCirugia)
                      .HasForeignKey(l => l.OrdenCirugiaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.Requisitos)
                      .WithOne(r => r.OrdenCirugia)
                      .HasForeignKey(r => r.OrdenCirugiaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.HistorialObservaciones)
                      .WithOne(h => h.OrdenCirugia)
                      .HasForeignKey(h => h.OrdenCirugiaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.MedicosHonorarios)
                      .WithOne(m => m.OrdenCirugia)
                      .HasForeignKey(m => m.OrdenCirugiaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.SolicitudesInsumos)
                      .WithOne(s => s.OrdenCirugia)
                      .HasForeignKey(s => s.OrdenCirugiaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(o => o.FechaHoraProgramada);
                entity.HasIndex(o => o.Estado);
            });

            builder.Entity<CirugiaMedicoHonorario>(entity =>
            {
                entity.ToTable("CirugiasMedicosHonorarios");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.MontoHonorarioUsd).HasPrecision(18, 2);

                entity.HasOne(c => c.Medico)
                      .WithMany()
                      .HasForeignKey(c => c.MedicoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Especialidad)
                      .WithMany()
                      .HasForeignKey(c => c.EspecialidadId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.OrdenCirugiaId);
                entity.HasIndex(c => c.MedicoId);
                entity.HasIndex(c => c.EspecialidadId);
            });

            builder.Entity<SolicitudInsumoCirugia>(entity =>
            {
                entity.ToTable("SolicitudesInsumosCirugia");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.CantidadSolicitada).HasPrecision(18, 4);
                entity.Property(s => s.EstadoSolicitud).IsRequired().HasMaxLength(50);
                entity.Property(s => s.UsuarioSolicitud).IsRequired().HasMaxLength(100);
                entity.Property(s => s.UsuarioDespacho).HasMaxLength(100);
                entity.Property(s => s.Observaciones).HasMaxLength(500);

                entity.HasOne(s => s.Insumo)
                      .WithMany()
                      .HasForeignKey(s => s.InsumoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.AlmacenOrigen)
                      .WithMany()
                      .HasForeignKey(s => s.AlmacenOrigenId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.OrdenCirugiaId);
                entity.HasIndex(s => s.EstadoSolicitud);
            });

            builder.Entity<TransferenciaReposicionStock>(entity =>
            {
                entity.ToTable("TransferenciasReposicionStock");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Cantidad).HasPrecision(18, 4);
                entity.Property(t => t.Motivo).IsRequired().HasMaxLength(100);
                entity.Property(t => t.UsuarioId).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Observaciones).HasMaxLength(500);

                entity.HasOne(t => t.Insumo)
                      .WithMany()
                      .HasForeignKey(t => t.InsumoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.SedeOrigen)
                      .WithMany()
                      .HasForeignKey(t => t.SedeOrigenId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.SedeDestino)
                      .WithMany()
                      .HasForeignKey(t => t.SedeDestinoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => t.FechaTransferencia);
            });

            builder.Entity<CirugiaLog>(entity =>
            {
                entity.ToTable("CirugiaLogs");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.UsuarioId).IsRequired().HasMaxLength(100);
                entity.Property(l => l.Evento).IsRequired().HasMaxLength(50);
                entity.Property(l => l.Detalle).HasMaxLength(1000);

                entity.HasOne(l => l.OrdenCirugia)
                      .WithMany(o => o.Logs)
                      .HasForeignKey(l => l.OrdenCirugiaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(l => l.OrdenCirugiaId);
                entity.HasIndex(l => l.Timestamp);
            });

            builder.Entity<RequisitoCirugia>(entity =>
            {
                entity.ToTable("RequisitosCirugia");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Nombre).IsRequired().HasMaxLength(250);
                entity.Property(r => r.Descripcion).HasMaxLength(500);

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
                entity.ToTable("OrdenesCirugiaRequisitos");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.VerificadoPor).HasMaxLength(100);

                entity.HasOne(r => r.RequisitoCirugia)
                      .WithMany(m => m.OrdenesRequisitos)
                      .HasForeignKey(r => r.RequisitoCirugiaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => r.OrdenCirugiaId);
            });

            builder.Entity<CirugiaObservacionHistorial>(entity =>
            {
                entity.ToTable("CirugiasObservacionesHistorial");
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Observacion).IsRequired().HasMaxLength(1000);
                entity.Property(h => h.Tipo).HasConversion<int>();
                entity.Property(h => h.UsuarioRegistro).IsRequired().HasMaxLength(100);
                // 3FN: FK lógica a Usuarios (Identity, PK Guid). Sin restricción FK física
                // porque la tabla Usuarios vive en el contexto de Identity.
                entity.Property(h => h.UsuarioRegistroId).HasColumnType("char(36)");

                entity.HasOne(h => h.OrdenCirugia)
                      .WithMany(o => o.HistorialObservaciones)
                      .HasForeignKey(h => h.OrdenCirugiaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(h => h.OrdenCirugiaId);
                entity.HasIndex(h => h.UsuarioRegistroId);
            });

            builder.Entity<OrdenCompraInventario>(entity =>
            {
                entity.ToTable("OrdenesCompraInventario");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.NumeroFactura).IsRequired().HasMaxLength(100);
                entity.Property(o => o.ProveedorNombre).IsRequired().HasMaxLength(250);
                entity.Property(o => o.MontoTotalUSD).HasPrecision(18, 2);
                entity.Property(o => o.MontoTotalBs).HasPrecision(18, 2);
                entity.Property(o => o.TotalAbonadoUSD).HasPrecision(18, 2);
                entity.Property(o => o.SaldoPendienteUSD).HasPrecision(18, 2);
                entity.Property(o => o.Estado).IsRequired().HasMaxLength(50);
                entity.Property(o => o.Observaciones).HasMaxLength(1000);

                entity.HasMany(o => o.Pagos)
                      .WithOne(p => p.OrdenCompra)
                      .HasForeignKey(p => p.OrdenCompraId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(o => o.NumeroFactura);
                entity.HasIndex(o => o.ProveedorNombre);
                entity.HasIndex(o => o.Estado);
            });

            builder.Entity<PagoProveedor>(entity =>
            {
                entity.ToTable("PagosProveedores");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.MontoAbonadoUSD).HasPrecision(18, 2);
                entity.Property(p => p.TasaCambio).HasPrecision(18, 2);
                entity.Property(p => p.MontoAbonadoBs).HasPrecision(18, 2);
                entity.Property(p => p.MetodoPago).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Referencia).HasMaxLength(100);
                entity.Property(p => p.UsuarioId).HasMaxLength(100);
                entity.Property(p => p.Observaciones).HasMaxLength(1000);
            });

            builder.Entity<OrdenImagen>(entity =>
            {
                entity.ToTable("OrdenesImagenes");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Estado).HasConversion<int>();
            });

            builder.Entity<PagoProveedor>().HasIndex(p => p.OrdenCompraId);
            builder.Entity<PagoProveedor>().HasIndex(p => p.FechaPago);

            builder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("Proveedores");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.RIF).IsRequired().HasMaxLength(50);
                entity.Property(p => p.RazonSocial).IsRequired().HasMaxLength(250);
                entity.Property(p => p.Direccion).HasMaxLength(500);
                entity.Property(p => p.Telefono).HasMaxLength(50);

                entity.HasIndex(p => p.RIF).IsUnique();
                entity.HasIndex(p => p.RazonSocial);
            });
        }

    }
}
