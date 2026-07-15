using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Common;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

using SistemaSatHospitalario.Core.Domain.Common;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Contexts
{
    public class SatHospitalarioDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<CajaDiaria> CajasDiarias { get; set; }
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
        public DbSet<HistorialModificacionCuenta> HistorialModificacionCuentas { get; set; }
        public DbSet<TriageEnfermeria> TriagesEnfermeria { get; set; }
        public DbSet<ValoracionFisica> ValoracionesFisicas { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
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

        public SatHospitalarioDbContext(DbContextOptions<SatHospitalarioDbContext> options) : base(options) { }
        public Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) => Database.BeginTransactionAsync(cancellationToken);

        public override int SaveChanges()
        {
            EnforceMovimientoInsumoImmutability();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            EnforceMovimientoInsumoImmutability();
            return base.SaveChangesAsync(cancellationToken);
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

                entity.HasOne<PacienteAdmision>()
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
                entity.Property(d => d.Cantidad).HasPrecision(18, 4);
                entity.HasOne(d => d.AreaClinica)
                      .WithMany()
                      .HasForeignKey(d => d.AreaClinicaId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<TipoServicio>()
                      .WithMany()
                      .HasForeignKey(d => d.TipoServicioId)
                      .OnDelete(DeleteBehavior.Restrict);
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
                entity.HasIndex(i => i.Codigo).IsUnique();
            });

            builder.Entity<ServicioInsumoReceta>(entity =>
            {
                entity.ToTable("ServiciosInsumoRecetas");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.ServicioCodigo).IsRequired().HasMaxLength(50);
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
            });

            builder.Entity<AreaClinica>(entity =>
            {
                entity.ToTable("AreasClinicas");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Nombre).IsRequired().HasMaxLength(150);
                entity.HasOne(a => a.Sede)
                      .WithMany(s => s.AreasClinicas)
                      .HasForeignKey(a => a.SedeId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(a => new { a.SedeId, a.Codigo }).IsUnique();
            });

            builder.Entity<StockSede>(entity =>
            {
                entity.ToTable("StocksSede");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.StockActual).HasPrecision(18, 4);
                entity.Property(s => s.StockMinimo).HasPrecision(18, 4);
                entity.Property(s => s.StockMaximo).HasPrecision(18, 4);

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
                entity.Property(t => t.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Codigo).IsRequired().HasMaxLength(10);

                entity.HasData(
                    new TipoServicio(1, "Medico", "MED"),
                    new TipoServicio(2, "Laboratorio", "LAB"),
                    new TipoServicio(3, "RX", "RX"),
                    new TipoServicio(4, "Tomo", "TOMO"),
                    new TipoServicio(5, "Insumo", "INS")
                );
            });
        }

    }
}
