using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            builder.Entity<CajaDiaria>(entity =>
            {
                entity.ToTable("CajasDiarias");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.MontoInicialDivisa).HasPrecision(18, 2);
                entity.Property(c => c.MontoInicialBs).HasPrecision(18, 2);
                entity.Property(c => c.UsuarioIdentityId).HasColumnType("char(36)");
                entity.HasIndex(c => c.UsuarioIdentityId);

                entity.HasOne(c => c.EstadoNav)
                    .WithMany()
                    .HasForeignKey(c => c.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(c => c.EstadoId);

                entity.HasMany(c => c.DeclaracionesPorMetodo)
                    .WithOne(d => d.CajaDiaria)
                    .HasForeignKey(d => d.CajaDiariaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            builder.Entity<SolicitudInsumoCirugia>(entity =>
            {
                entity.ToTable("SolicitudesInsumosCirugia");
                entity.HasKey(s => s.Id);

                entity.Property(s => s.CantidadSolicitada)
                    .HasPrecision(18, 4)
                    .IsRequired();

                entity.Property(s => s.EstadoSolicitud)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(s => s.FechaSolicitud)
                    .IsRequired();

                entity.Property(s => s.Observaciones)
                    .HasMaxLength(1000);

                entity.HasOne(s => s.OrdenCirugia)
                    .WithMany(o => o.SolicitudesInsumos)
                    .HasForeignKey(s => s.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Insumo)
                    .WithMany()
                    .HasForeignKey(s => s.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.AlmacenOrigen)
                    .WithMany()
                    .HasForeignKey(s => s.AlmacenOrigenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.OrdenCirugiaId);
                entity.HasIndex(s => s.InsumoId);
                entity.HasIndex(s => s.AlmacenOrigenId);
                entity.HasIndex(s => s.EstadoSolicitud);
            });

            builder.Entity<CajaDeclaracionMetodo>(entity =>
            {
                entity.ToTable("CajaDeclaracionesMetodos");
                entity.HasKey(d => d.Id);

                entity.Property(d => d.MontoIngresado).HasPrecision(18, 2);
                entity.Property(d => d.MontoVueltos).HasPrecision(18, 2);
                entity.Property(d => d.MontoEsperadoIngreso).HasPrecision(18, 2);
                entity.Property(d => d.MontoEsperadoVueltos).HasPrecision(18, 2);
                entity.Property(d => d.DiferenciaOriginal).HasPrecision(18, 2);
                entity.Property(d => d.DiferenciaBase).HasPrecision(18, 2);

                entity.HasOne(d => d.CajaDiaria)
                    .WithMany(c => c.DeclaracionesPorMetodo)
                    .HasForeignKey(d => d.CajaDiariaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.MetodoPago)
                    .WithMany()
                    .HasForeignKey(d => d.MetodoPagoId)
                    .OnDelete(DeleteBehavior.Restrict);

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

            builder.Entity<ReciboFactura>(entity =>
            {
                entity.ToTable("RecibosFacturas");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.NumeroRecibo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.NroControlFiscal)
                    .HasMaxLength(50);

                entity.Property(r => r.NumeroComprobante)
                    .HasMaxLength(50);

                entity.Property(r => r.TasaCambioDia).HasPrecision(18, 4);
                entity.Property(r => r.TotalFacturadoUSD).HasPrecision(18, 2);
                entity.Property(r => r.MontoVueltoUSD).HasPrecision(18, 2);

                entity.Property(r => r.UsuarioEmisionId).HasColumnType("char(36)");
                entity.HasIndex(r => r.UsuarioEmisionId);
                entity.HasIndex(r => r.PacienteId);
                entity.HasIndex(r => r.NumeroRecibo).IsUnique();

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

                entity.HasMany(r => r.DetallesPago)
                    .WithOne(d => d.ReciboFactura)
                    .HasForeignKey(d => d.ReciboFacturaId)
                    .OnDelete(DeleteBehavior.Cascade);

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

                entity.Property(d => d.ReferenciaBancaria).HasMaxLength(100);
                entity.Property(d => d.MontoAbonadoMoneda).HasPrecision(18, 2);
                entity.Property(d => d.EquivalenteAbonadoBase).HasPrecision(18, 2);
                entity.Property(d => d.TasaCambioAplicada).HasPrecision(18, 4);

                entity.HasOne(d => d.ReciboFactura)
                    .WithMany(r => r.DetallesPago)
                    .HasForeignKey(d => d.ReciboFacturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.MetodoPagoNav)
                    .WithMany()
                    .HasForeignKey(d => d.MetodoPagoId)
                    .OnDelete(DeleteBehavior.Restrict);

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

                entity.Property(h => h.CategoriaServicio).IsRequired().HasMaxLength(50);
                entity.Property(h => h.NotasConfig).HasMaxLength(500);

                entity.HasOne(h => h.MedicoDefault)
                    .WithMany()
                    .HasForeignKey(h => h.MedicoDefaultId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(h => h.UsuarioConfiguroId).HasColumnType("char(36)");

                entity.HasIndex(h => h.CategoriaServicio).IsUnique();
                entity.HasIndex(h => h.MedicoDefaultId);
                entity.HasIndex(h => h.UsuarioConfiguroId);
            });

            builder.Entity<LogAsignacionHonorario>(entity =>
            {
                entity.ToTable("LogsAsignacionHonorario");
                entity.HasKey(l => l.Id);

                entity.Property(l => l.TipoAccion).IsRequired().HasMaxLength(50);
                entity.Property(l => l.NombreServicio).IsRequired().HasMaxLength(200);
                entity.Property(l => l.MedicoAnteriorNombre).HasMaxLength(200);
                entity.Property(l => l.MedicoNuevoNombre).HasMaxLength(200);
                entity.Property(l => l.Observaciones).HasMaxLength(1000);

                entity.HasOne(l => l.DetalleServicio)
                    .WithMany()
                    .HasForeignKey(l => l.DetalleServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.MedicoAnterior)
                    .WithMany()
                    .HasForeignKey(l => l.MedicoAnteriorId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(l => l.MedicoNuevo)
                    .WithMany()
                    .HasForeignKey(l => l.MedicoNuevoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(l => l.UsuarioOperadorId).HasColumnType("char(36)");

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

                entity.Property(d => d.Descripcion).IsRequired().HasMaxLength(300);
                entity.Property(d => d.LegacyMappingId).HasMaxLength(50);

                entity.Property(d => d.Precio).HasPrecision(18, 2);
                entity.Property(d => d.Honorario).HasPrecision(18, 2);
                entity.Property(d => d.PrecioCatalogoHistorico).HasPrecision(18, 2);
                entity.Property(d => d.Cantidad).HasPrecision(18, 4);

                entity.Property(d => d.IncluidoEnTarifaBase).IsRequired();

                entity.HasOne(d => d.CuentaServicio)
                    .WithMany(c => c.Detalles)
                    .HasForeignKey(d => d.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(d => d.MedicoResponsableId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.DetallePadre)
                    .WithMany()
                    .HasForeignKey(d => d.DetallePadreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.AreaClinica)
                    .WithMany()
                    .HasForeignKey(d => d.AreaClinicaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.TipoServicioNav)
                    .WithMany()
                    .HasForeignKey(d => d.TipoServicioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.MedicosResponsables)
                    .WithOne(m => m.DetalleServicioCuenta)
                    .HasForeignKey(m => m.DetalleServicioCuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

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

                entity.Property(p => p.CedulaPasaporte).IsRequired().HasMaxLength(30);
                entity.Property(p => p.NombreCorto).IsRequired().HasMaxLength(150);
                entity.Property(p => p.TelefonoContact).HasMaxLength(30);
                entity.Property(p => p.Direccion).HasMaxLength(300);

                entity.HasIndex(p => p.CedulaPasaporte).IsUnique();
                entity.HasIndex(p => p.IdPacienteLegacy).IsUnique();
                entity.HasIndex(p => p.NombreCorto);
            });

            builder.Entity<OrdenDeServicio>(entity =>
            {
                entity.ToTable("OrdenesDeServicio");
                entity.HasKey(o => o.Id);

                entity.HasDiscriminator<string>("Discriminator")
                    .HasValue<OrdenDeServicio>("OrdenDeServicio")
                    .HasValue<OrdenRX>("OrdenRX");

                entity.Property<string>("Discriminator").HasMaxLength(50);
                entity.Property(o => o.TipoIngreso).IsRequired().HasMaxLength(50);
                entity.Property(o => o.EstadoFacturacion).HasConversion<int>();

                entity.HasOne(o => o.Paciente)
                    .WithMany()
                    .HasForeignKey(o => o.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.PacienteId);
                entity.HasIndex(o => o.FechaCreacion);
                entity.HasIndex(o => o.ConvenioId);
                entity.HasIndex(o => new { o.FechaCreacion, o.NumeroLlegadaDiario });
            });

            builder.Entity<OrdenRX>(entity =>
            {
                entity.HasBaseType<OrdenDeServicio>();
                entity.Property(rx => rx.EstudioSolicitado).IsRequired().HasMaxLength(250);
                entity.Property(rx => rx.Procesada).IsRequired();
                entity.Property(rx => rx.AsistenteRxId).HasColumnType("char(36)");

                entity.HasIndex(rx => rx.AsistenteRxId);
                entity.HasIndex(rx => rx.Procesada);
                entity.HasIndex(rx => rx.FechaProcesada);
            });

            builder.Entity<TurnoMedico>(entity =>
            {
                entity.ToTable("TurnosMedicos");
                entity.HasKey(t => t.Id);

                entity.Property(t => t.FechaHoraToma).IsRequired();
                entity.Property(t => t.IgnorandoIncidencia).IsRequired();

                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(t => t.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<PacienteAdmision>()
                    .WithMany()
                    .HasForeignKey(t => t.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => t.MedicoId);
                entity.HasIndex(t => t.PacienteId);
                entity.HasIndex(t => t.FechaHoraToma);
                entity.HasIndex(t => t.IncidenciaIgnoradaId);
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

                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(i => i.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

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

                entity.Property(r => r.Motivo).IsRequired().HasMaxLength(500);
                entity.Property(r => r.FechaTraza).IsRequired();

                entity.HasOne<TurnoMedico>()
                    .WithMany()
                    .HasForeignKey(r => r.TurnoMedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<IncidenciaHorario>()
                    .WithMany()
                    .HasForeignKey(r => r.IncidenciaIgnoradaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(r => r.OperadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(r => r.OperadorId).HasColumnType("char(36)");

                entity.HasIndex(r => r.TurnoMedicoId);
                entity.HasIndex(r => r.IncidenciaIgnoradaId);
                entity.HasIndex(r => r.OperadorId);
                entity.HasIndex(r => r.FechaTraza);
            });

            builder.Entity<CuentaServicios>(entity =>
            {
                entity.ToTable("CuentasServicios");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.SubAreaClinica).HasMaxLength(100);
                entity.Property(c => c.ProcesamientoEstado).HasMaxLength(50);
                entity.Property(c => c.DestinoPaciente).HasMaxLength(200);
                entity.Property(c => c.PersonalRelevo).HasMaxLength(150);

                entity.HasOne(c => c.Paciente)
                    .WithMany()
                    .HasForeignKey(c => c.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Convenio)
                    .WithMany()
                    .HasForeignKey(c => c.ConvenioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CuentaPrincipal)
                    .WithMany()
                    .HasForeignKey(c => c.CuentaPrincipalId)
                    .OnDelete(DeleteBehavior.Restrict);

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

                entity.HasOne(c => c.EstadoNav)
                    .WithMany()
                    .HasForeignKey(c => c.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.TipoIngresoNav)
                    .WithMany()
                    .HasForeignKey(c => c.TipoIngresoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Detalles)
                    .WithOne(d => d.CuentaServicio)
                    .HasForeignKey(d => d.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation(c => c.Detalles)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(c => c.Triages)
                    .WithOne(t => t.CuentaServicio)
                    .HasForeignKey(t => t.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Valoraciones)
                    .WithOne(v => v.CuentaServicio)
                    .HasForeignKey(v => v.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.UsuarioCargaId).HasColumnType("char(36)");
                entity.Property(c => c.UsuarioValidacionId).HasColumnType("char(36)");
                entity.Property(c => c.UsuarioAuditoriaId).HasColumnType("char(36)");

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioCargaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioValidacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioAuditoriaId)
                    .OnDelete(DeleteBehavior.Restrict);

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
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired().HasDefaultValue(true);
                entity.HasIndex(e => e.Codigo).IsUnique();

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
                entity.Property(t => t.Id).ValueGeneratedNever();
                entity.Property(t => t.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Activo).IsRequired().HasDefaultValue(true);
                entity.HasIndex(t => t.Codigo).IsUnique();

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

                entity.Property(c => c.Comentario).HasMaxLength(500);
                entity.Property(c => c.HoraPautada).IsRequired();
                entity.Property(c => c.FechaRegistro).IsRequired();

                entity.HasOne(c => c.Medico)
                    .WithMany()
                    .HasForeignKey(c => c.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<PacienteAdmision>()
                    .WithMany()
                    .HasForeignKey(c => c.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CuentaServicio)
                    .WithMany()
                    .HasForeignKey(c => c.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.AreaClinica)
                    .WithMany()
                    .HasForeignKey(c => c.AreaClinicaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(c => c.EstadoNav)
                    .WithMany()
                    .HasForeignKey(c => c.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.MedicoId);
                entity.HasIndex(c => c.PacienteId);
                entity.HasIndex(c => c.CuentaServicioId);
                entity.HasIndex(c => c.AreaClinicaId);
                entity.HasIndex(c => c.EstadoId);
                entity.HasIndex(c => c.HoraPautada);
                entity.HasIndex(c => c.FechaRegistro);
                entity.HasIndex(c => new { c.MedicoId, c.HoraPautada });
            });

            builder.Entity<EstadoCitaMedica>(entity =>
            {
                entity.ToTable("EstadosCitaMedica");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired().HasDefaultValue(true);
                entity.HasIndex(e => e.Codigo).IsUnique();

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

                entity.Property(m => m.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(m => m.Telefono).HasMaxLength(30);
                entity.Property(m => m.HonorarioBase).HasPrecision(18, 2).HasDefaultValue(0.00m);
                entity.Property(m => m.IntervaloTurnoMinutos).IsRequired().HasDefaultValue(30);
                entity.Property(m => m.Activo).IsRequired().HasDefaultValue(true);

                entity.HasOne(m => m.Especialidad)
                    .WithMany()
                    .HasForeignKey(m => m.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => m.EspecialidadId);
                entity.HasIndex(m => m.Nombre);
                entity.HasIndex(m => m.Activo);
            });

            builder.Entity<ServicioClinico>(entity =>
            {
                entity.ToTable("ServiciosClinicos");
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Descripcion).IsRequired().HasMaxLength(300);
                entity.Property(s => s.TipoServicio).HasMaxLength(50);
                entity.Property(s => s.LegacyMappingId).HasMaxLength(50);
                entity.Property(s => s.HonorariumCategory).HasMaxLength(50);
                entity.Property(s => s.UnidadMedida).HasMaxLength(50);
                entity.Property(s => s.DesactivadoPorUsuarioId).HasMaxLength(100);

                entity.Property(s => s.PrecioBase).HasPrecision(18, 2);
                entity.Property(s => s.HonorarioBase).HasPrecision(18, 2);

                entity.Property(s => s.Activo).IsRequired().HasDefaultValue(true);
                entity.Property(s => s.RequiereInventario).IsRequired().HasDefaultValue(true);
                entity.Property(s => s.EsServicioInforme).IsRequired().HasDefaultValue(false);
                entity.Property(s => s.PermiteFraccionamiento).IsRequired().HasDefaultValue(false);

                entity.Property(s => s.Category).HasConversion<int>();

                entity.HasOne<TipoServicio>()
                    .WithMany()
                    .HasForeignKey(s => s.TipoServicioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Especialidad)
                    .WithMany()
                    .HasForeignKey(s => s.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.ServicioInforme)
                    .WithMany()
                    .HasForeignKey(s => s.ServicioInformeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.Codigo).IsUnique();
                entity.HasIndex(s => s.Descripcion);
                entity.HasIndex(s => s.TipoServicioId);
                entity.HasIndex(s => s.EspecialidadId);
                entity.HasIndex(s => s.ServicioInformeId);
                entity.HasIndex(s => s.Activo);
            });

            builder.Entity<TriageEnfermeria>(entity =>
            {
                entity.ToTable("TriagesEnfermeria");
                entity.HasKey(t => t.Id);

                entity.Property(t => t.TensionArterial).IsRequired().HasMaxLength(20);
                entity.Property(t => t.MotivoConsulta).IsRequired().HasMaxLength(500);
                entity.Property(t => t.DescripcionRapida).HasMaxLength(250);
                entity.Property(t => t.DescripcionDetallada).HasMaxLength(2000);
                entity.Property(t => t.Temperatura).HasPrecision(4, 2);
                entity.Property(t => t.FechaRegistro).IsRequired();

                entity.HasOne<CuentaServicios>()
                    .WithMany(c => c.Triages)
                    .HasForeignKey(t => t.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(t => t.UsuarioRegistroId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.UsuarioRegistroId).HasColumnType("char(36)");

                entity.HasIndex(t => t.CuentaServicioId);
                entity.HasIndex(t => t.UsuarioRegistroId);
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

                entity.Property(v => v.Alergias).HasMaxLength(500);
                entity.Property(v => v.AccesosVenosos).HasMaxLength(500);
                entity.Property(v => v.Pertenencias).HasMaxLength(500);
                entity.Property(v => v.AntecedentesMedicos).HasMaxLength(1000);

                entity.Property(v => v.UsuarioRegistro).HasMaxLength(100);
                entity.Property(v => v.FechaRegistro).IsRequired();

                entity.HasOne(v => v.CuentaServicio)
                    .WithMany(c => c.Valoraciones)
                    .HasForeignKey(v => v.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(v => v.UsuarioRegistroId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(v => v.UsuarioRegistroId).HasColumnType("char(36)");

                entity.HasIndex(v => v.CuentaServicioId);
                entity.HasIndex(v => v.UsuarioRegistroId);
                entity.HasIndex(v => v.FechaRegistro);
            });

            builder.Entity<HorarioAtencionMedico>(entity =>
            {
                entity.ToTable("HorariosAtencionMedicos");
                entity.HasKey(h => h.Id);

                entity.Property(h => h.DiaSemana).IsRequired();
                entity.Property(h => h.HoraInicio).IsRequired();
                entity.Property(h => h.HoraFin).IsRequired();

                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(h => h.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(h => h.MedicoId);
                entity.HasIndex(h => new { h.MedicoId, h.DiaSemana });
            });

            builder.Entity<PrecioServicioConvenio>(entity =>
            {
                entity.ToTable("PreciosServicioConvenio");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.PrecioDiferencial).HasPrecision(18, 2).IsRequired();

                entity.HasOne(p => p.Servicio)
                    .WithMany()
                    .HasForeignKey(p => p.ServicioClinicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Convenio)
                    .WithMany()
                    .HasForeignKey(p => p.SeguroConvenioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.ServicioClinicoId);
                entity.HasIndex(p => p.SeguroConvenioId);
                entity.HasIndex(p => new { p.ServicioClinicoId, p.SeguroConvenioId }).IsUnique();
            });

            builder.Entity<ConvenioPerfilPrecio>(entity =>
            {
                entity.ToTable("ConvenioPerfilPrecios");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.PrecioHNL).HasPrecision(18, 2).IsRequired();
                entity.Property(c => c.PrecioUSD).HasPrecision(18, 2).IsRequired();
                entity.Property(c => c.Activo).IsRequired().HasDefaultValue(true);
                entity.Property(c => c.UltimaActualizacion).IsRequired();

                entity.HasOne(c => c.Convenio)
                    .WithMany()
                    .HasForeignKey(c => c.SeguroConvenioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.SeguroConvenioId);
                entity.HasIndex(c => c.PerfilId);
                entity.HasIndex(c => c.Activo);
                entity.HasIndex(c => new { c.SeguroConvenioId, c.PerfilId }).IsUnique();
            });

            builder.Entity<CuentaPorCobrar>(entity =>
            {
                entity.ToTable("CuentasPorCobrar");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.MontoTotalBase).HasPrecision(18, 2).IsRequired();
                entity.Property(c => c.MontoPagadoBase).HasPrecision(18, 2).IsRequired();
                entity.Ignore(c => c.SaldoPendienteBase);

                entity.Property(c => c.Estado).IsRequired().HasMaxLength(50);
                entity.Property(c => c.UsuarioAuditoria).HasMaxLength(100);
                entity.Property(c => c.QuienAutorizo).HasMaxLength(150);
                entity.Property(c => c.DoctorProcedimiento).HasMaxLength(150);
                entity.Property(c => c.InformacionAdicional).HasMaxLength(1000);
                entity.Property(c => c.FechaCreacion).IsRequired();

                entity.HasOne(c => c.Cuenta)
                    .WithMany()
                    .HasForeignKey(c => c.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<PacienteAdmision>()
                    .WithMany()
                    .HasForeignKey(c => c.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.GarantiasItems)
                    .WithOne(g => g.CuentaPorCobrar)
                    .HasForeignKey(g => g.CuentaPorCobrarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioAuditoriaId)
                    .OnDelete(DeleteBehavior.Restrict);

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

                entity.Property(g => g.Descripcion).IsRequired().HasMaxLength(500);
                entity.Property(g => g.ValorEstimado).HasPrecision(18, 2).IsRequired();
                entity.Property(g => g.FechaRegistro).IsRequired();

                entity.HasOne(g => g.CuentaPorCobrar)
                    .WithMany(c => c.GarantiasItems)
                    .HasForeignKey(g => g.CuentaPorCobrarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(g => g.CuentaPorCobrarId);
                entity.HasIndex(g => g.FechaRegistro);
            });

            builder.Entity<CompromisoPago>(entity =>
            {
                entity.ToTable("CompromisosPago");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Observacion).HasMaxLength(1000);
                entity.Property(c => c.UsuarioCreacion).HasMaxLength(100);
                entity.Property(c => c.Omitido).IsRequired().HasDefaultValue(false);
                entity.Property(c => c.FechaCreacion).IsRequired();

                entity.HasOne(c => c.CuentaPorCobrar)
                    .WithMany()
                    .HasForeignKey(c => c.CuentaPorCobrarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.MotivoAutorizacion)
                    .WithMany()
                    .HasForeignKey(c => c.MotivoAutorizacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioCreacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.UsuarioCreacionId).HasColumnType("char(36)");

                entity.HasIndex(c => c.CuentaPorCobrarId);
                entity.HasIndex(c => c.MotivoAutorizacionId);
                entity.HasIndex(c => c.UsuarioCreacionId);
                entity.HasIndex(c => c.FechaCreacion);
            });

            builder.Entity<MotivoAutorizacion>(entity =>
            {
                entity.ToTable("MotivosAutorizacion");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.Property(m => m.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(m => m.Activo).IsRequired().HasDefaultValue(true);
                entity.HasIndex(m => m.Nombre).IsUnique();

                entity.HasData(
                    new MotivoAutorizacion(1, "Autorizado por Dirección Médica", activo: true),
                    new MotivoAutorizacion(2, "Exoneración por Presidencia", activo: true),
                    new MotivoAutorizacion(3, "Convenio Institucional", activo: true)
                );
            });

            builder.Entity<TasaCambio>(entity =>
            {
                entity.ToTable("TasasCambio");
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Monto).HasPrecision(18, 4).IsRequired();
                entity.Property(t => t.Fecha).IsRequired();
                entity.Property(t => t.Activo).IsRequired().HasDefaultValue(true);

                entity.HasIndex(t => t.Fecha);
                entity.HasIndex(t => t.Activo);
                entity.HasIndex(t => new { t.Activo, t.Fecha });
            });
            
            builder.Entity<ErrorTicket>(entity =>
            {
                entity.ToTable("ErrorTickets");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RequestPath).HasMaxLength(500);
                entity.Property(e => e.MetodoHTTP).HasMaxLength(10);
                entity.Property(e => e.MensajeExcepcion).HasMaxLength(2000);
                entity.Property(e => e.UsuarioAsociado).HasMaxLength(100);
                entity.Property(e => e.ResueltoPor).HasMaxLength(100);
                entity.Property(e => e.ComentariosResolucion).HasMaxLength(1000);
                entity.Property(e => e.Resuelto).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.FechaCreacion).IsRequired();

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioAsociadoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(e => e.ResueltoPorId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(e => e.UsuarioAsociadoId).HasColumnType("char(36)");
                entity.Property(e => e.ResueltoPorId).HasColumnType("char(36)");

                entity.HasIndex(e => e.UsuarioAsociadoId);
                entity.HasIndex(e => e.ResueltoPorId);
                entity.HasIndex(e => e.Resuelto);
                entity.HasIndex(e => e.FechaCreacion);
            });

            builder.Entity<Especialidad>(entity =>
            {
                entity.ToTable("Especialidades");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired().HasDefaultValue(true);

                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.HasIndex(e => e.Activo);
            });

            builder.Entity<ReservaTemporal>(entity =>
            {
                entity.ToTable("ReservasTemporales");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Comentario).HasMaxLength(500);
                entity.Property(r => r.HoraPautada).IsRequired();
                entity.Property(r => r.ExpiracionUtc).IsRequired();

                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(r => r.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(r => r.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(r => r.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(r => r.UsuarioIdentityId);
                entity.HasIndex(r => r.ExpiracionUtc);
                entity.HasIndex(r => new { r.MedicoId, r.HoraPautada }).IsUnique();
            });
            
            builder.Entity<BloqueoHorario>(entity =>
            {
                entity.ToTable("BloqueosHorarios");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Motivo).IsRequired().HasMaxLength(500);
                entity.Property(b => b.HoraPautada).IsRequired();
                entity.Property(b => b.FechaRegistro).IsRequired();

                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(b => b.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(b => b.MedicoId);
                entity.HasIndex(b => b.FechaRegistro);
                entity.HasIndex(b => new { b.MedicoId, b.HoraPautada }).IsUnique();
            });

            builder.Entity<ConfiguracionGeneral>(entity =>
            {
                entity.ToTable("ConfiguracionGeneral");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.NombreEmpresa).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Rif).IsRequired().HasMaxLength(30);
                entity.Property(c => c.ClaveSupervisor).IsRequired().HasMaxLength(255);
                entity.Property(c => c.LogoBase64).HasColumnType("longtext");
                entity.Property(c => c.Iva).HasPrecision(5, 2).IsRequired();
                entity.Property(c => c.FacturarLaboratorio).IsRequired().HasDefaultValue(false);
                entity.Property(c => c.MostrarDetalleFacturacion).IsRequired().HasDefaultValue(false);
                entity.Property(c => c.UltimaActualizacion).IsRequired();
            });

            builder.Entity<ServicioSugerencia>(entity =>
            {
                entity.ToTable("ServiciosSugerencias");
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.ServicioOrigen)
                    .WithMany(sc => sc.Sugerencias)
                    .HasForeignKey(s => s.ServicioOrigenId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.ServicioSugerido)
                    .WithMany()
                    .HasForeignKey(s => s.ServicioSugeridoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.ServicioOrigenId);
                entity.HasIndex(s => s.ServicioSugeridoId);
                entity.HasIndex(s => new { s.ServicioOrigenId, s.ServicioSugeridoId }).IsUnique();
            });

            builder.Entity<LogAuditoriaPrecio>(entity =>
            {
                entity.ToTable("AuditLogsPrecios");
                entity.HasKey(a => a.Id);

                entity.Property(a => a.PrecioOriginal).HasPrecision(18, 2).IsRequired();
                entity.Property(a => a.PrecioModificado).HasPrecision(18, 2).IsRequired();
                entity.Property(a => a.HonorarioAnterior).HasPrecision(18, 2).IsRequired();
                entity.Property(a => a.NuevoHonorario).HasPrecision(18, 2).IsRequired();

                entity.Property(a => a.DescripcionServicio).IsRequired().HasMaxLength(500);
                entity.Property(a => a.UsuarioOperador).HasMaxLength(100);
                entity.Property(a => a.AutorizadoPor).HasMaxLength(100);
                entity.Property(a => a.FechaModificacion).IsRequired();

                entity.HasOne(a => a.DetalleServicio)
                    .WithMany()
                    .HasForeignKey(a => a.DetalleServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(a => a.UsuarioOperadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(a => a.AutorizadoPorId)
                    .OnDelete(DeleteBehavior.Restrict);

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

                entity.Property(o => o.Estudio).IsRequired().HasMaxLength(250);
                entity.Property(o => o.TipoServicio).IsRequired().HasMaxLength(50);
                entity.Property(o => o.ProcesadoPor).HasMaxLength(100);
                entity.Property(o => o.ValidadorPor).HasMaxLength(100);
                entity.Property(o => o.LinkInforme).HasMaxLength(500);
                entity.Property(o => o.ObservacionesMedico).HasMaxLength(1000);
                entity.Property(o => o.Informe).HasColumnType("text");

                entity.Property(o => o.Estado).HasConversion<int>().IsRequired();
                entity.Property(o => o.EsDirecta).IsRequired().HasDefaultValue(false);
                entity.Property(o => o.RequiereValidacion).IsRequired().HasDefaultValue(false);
                entity.Property(o => o.Validada).IsRequired().HasDefaultValue(false);
                entity.Property(o => o.RequiereInforme).IsRequired().HasDefaultValue(false);
                entity.Property(o => o.FechaCreacion).IsRequired();

                entity.HasOne<CuentaServicios>()
                    .WithMany()
                    .HasForeignKey(o => o.CuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.Paciente)
                    .WithMany()
                    .HasForeignKey(o => o.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.MedicoSolicitante)
                    .WithMany()
                    .HasForeignKey(o => o.MedicoSolicitanteId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(o => o.MedicoInterpreteId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(o => o.CuentaId);
                entity.HasIndex(o => o.PacienteId);
                entity.HasIndex(o => o.MedicoSolicitanteId);
                entity.HasIndex(o => o.MedicoInterpreteId);
                entity.HasIndex(o => o.FechaCreacion);
                entity.HasIndex(o => new { o.TipoServicio, o.Estado });
            });

            builder.Entity<Moneda>(entity =>
            {
                entity.ToTable("Monedas");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedNever();

                entity.Property(m => m.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(m => m.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Simbolo).IsRequired().HasMaxLength(10);
                entity.Property(m => m.EsBaseUsd).IsRequired().HasDefaultValue(false);

                entity.HasIndex(m => m.Codigo).IsUnique();

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

                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Valor).IsRequired().HasMaxLength(100);
                entity.Property(c => c.GrupoMoneda).IsRequired().HasDefaultValue(1);
                entity.Property(c => c.EsUSD).IsRequired().HasDefaultValue(false);
                entity.Property(c => c.EsVuelto).IsRequired().HasDefaultValue(false);
                entity.Property(c => c.Activo).IsRequired().HasDefaultValue(true);
                entity.Property(c => c.Orden).IsRequired().HasDefaultValue(0);

                entity.HasOne(c => c.Moneda)
                    .WithMany()
                    .HasForeignKey(c => c.GrupoMoneda)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.Valor).IsUnique();
                entity.HasIndex(c => c.GrupoMoneda);
                entity.HasIndex(c => c.Activo);
                entity.HasIndex(c => new { c.Activo, c.Orden });
            });

            builder.Entity<DocumentLog>(entity =>
            {
                entity.ToTable("DocumentLogs");
                entity.HasKey(d => d.Id);

                entity.Property(d => d.DocumentType).IsRequired().HasMaxLength(100);
                entity.Property(d => d.ReferenceId).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Action).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Details).HasMaxLength(2000);
                entity.Property(d => d.Timestamp).IsRequired();

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(d => d.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(d => d.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(d => d.UsuarioIdentityId);
                entity.HasIndex(d => d.ReferenceId);
                entity.HasIndex(d => d.Timestamp);
                entity.HasIndex(d => new { d.DocumentType, d.ReferenceId });
            });

            builder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(a => a.Id);

                entity.Property(a => a.ActionType).IsRequired().HasMaxLength(100);
                entity.Property(a => a.IpAddress).HasMaxLength(50);
                entity.Property(a => a.OldValue).HasColumnType("longtext");
                entity.Property(a => a.NewValue).HasColumnType("longtext");
                entity.Property(a => a.Timestamp).IsRequired();

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(a => a.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(a => a.UsuarioIdentityId);
                entity.HasIndex(a => a.ActionType);
                entity.HasIndex(a => a.Timestamp);
                entity.HasIndex(a => new { a.ActionType, a.Timestamp });
            });

            builder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(n => n.Id);

                entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
                entity.Property(n => n.Message).IsRequired().HasMaxLength(1000);
                entity.Property(n => n.Type).IsRequired().HasMaxLength(50);
                entity.Property(n => n.TargetRole).HasMaxLength(100);
                entity.Property(n => n.ActionUrl).HasMaxLength(500);
                entity.Property(n => n.IsRead).IsRequired().HasDefaultValue(false);
                entity.Property(n => n.Timestamp).IsRequired();

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(n => n.TargetUserGuidId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(n => n.TargetUserGuidId).HasColumnType("char(36)");

                entity.HasIndex(n => n.TargetUserGuidId);
                entity.HasIndex(n => n.TargetRole);
                entity.HasIndex(n => n.Timestamp);
                entity.HasIndex(n => new { n.TargetUserGuidId, n.IsRead });
            });

            builder.Entity<HonorariumMappingRule>(entity =>
            {
                entity.ToTable("HonorariumMappingRules");
                entity.HasKey(h => h.Id);

                entity.Property(h => h.Pattern).IsRequired().HasMaxLength(100);
                entity.Property(h => h.Category).IsRequired().HasMaxLength(50);
                entity.Property(h => h.UsuarioCreo).HasMaxLength(100);

                entity.Property(h => h.MappingRuleType).HasConversion<int>().IsRequired();
                entity.Property(h => h.Priority).IsRequired().HasDefaultValue(0);
                entity.Property(h => h.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(h => h.FechaCreacion).IsRequired();

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioCreoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(h => h.UsuarioCreoId).HasColumnType("char(36)");

                entity.HasIndex(h => h.UsuarioCreoId);
                entity.HasIndex(h => h.Category);
                entity.HasIndex(h => new { h.IsActive, h.Priority });
            });

            builder.Entity<HonorarioMedicoServicio>(entity =>
            {
                entity.ToTable("HonorariosMedicosServicios");
                entity.HasKey(h => h.Id);

                entity.Property(h => h.MontoHonorario).HasPrecision(18, 2).IsRequired();
                entity.Property(h => h.FechaModificacion).IsRequired();

                entity.HasOne(h => h.Servicio)
                    .WithMany(s => s.HonorariosMedicos)
                    .HasForeignKey(h => h.ServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.Medico)
                    .WithMany()
                    .HasForeignKey(h => h.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioModificoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(h => h.UsuarioModificoId).HasColumnType("char(36)");

                entity.HasIndex(h => h.ServicioId);
                entity.HasIndex(h => h.MedicoId);
                entity.HasIndex(h => h.UsuarioModificoId);
                entity.HasIndex(h => h.FechaModificacion);
                entity.HasIndex(h => new { h.ServicioId, h.MedicoId }).IsUnique();
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

                entity.Property(h => h.TipoIngresoAnterior).HasMaxLength(50);
                entity.Property(h => h.TipoIngresoNuevo).HasMaxLength(50);
                entity.Property(h => h.FechaModificacion).IsRequired();

                entity.HasOne<CuentaServicios>()
                    .WithMany()
                    .HasForeignKey(h => h.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(h => h.DetallesModificados)
                    .WithOne(d => d.HistorialModificacionCuenta)
                    .HasForeignKey(d => d.HistorialModificacionCuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

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

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(h => h.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

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

                entity.Property(d => d.PrecioAnterior).HasPrecision(18, 2).IsRequired();
                entity.Property(d => d.PrecioNuevo).HasPrecision(18, 2).IsRequired();
                entity.Property(d => d.HonorarioAnterior).HasPrecision(18, 2).IsRequired();
                entity.Property(d => d.HonorarioNuevo).HasPrecision(18, 2).IsRequired();

                entity.Property(d => d.CantidadAnterior).HasPrecision(18, 4).IsRequired();
                entity.Property(d => d.CantidadNueva).HasPrecision(18, 4).IsRequired();

                entity.HasOne(d => d.HistorialModificacionCuenta)
                    .WithMany(h => h.DetallesModificados)
                    .HasForeignKey(d => d.HistorialModificacionCuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.DetalleServicio)
                    .WithMany()
                    .HasForeignKey(d => d.DetalleServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(d => d.HistorialModificacionCuentaId);
                entity.HasIndex(d => d.DetalleServicioId);
            });

            builder.Entity<Insumo>(entity =>
            {
                entity.ToTable("Insumos");
                entity.HasKey(i => i.Id);

                entity.Ignore(i => i.StockActual);

                entity.Property(i => i.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(i => i.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(i => i.ReactivosCombinados).HasMaxLength(500);
                entity.Property(i => i.Indicaciones).HasMaxLength(1000);

                entity.Property(i => i.CostoUnitarioBaseUSD).HasPrecision(18, 4).IsRequired();

                entity.Property(i => i.PermiteFraccionamiento).IsRequired().HasDefaultValue(true);
                entity.Property(i => i.IsDeleted).IsRequired().HasDefaultValue(false);
                entity.Property(i => i.OcultoEnTraslados).IsRequired().HasDefaultValue(false);

                entity.HasOne(i => i.UnidadMedidaNav)
                    .WithMany()
                    .HasForeignKey(i => i.UnidadMedidaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.CategoriaInsumo)
                    .WithMany()
                    .HasForeignKey(i => i.CategoriaInsumoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(i => i.StocksPorSede)
                    .WithOne(s => s.Insumo)
                    .HasForeignKey(s => s.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(i => i.PrincipiosActivos)
                    .WithOne(p => p.Insumo)
                    .HasForeignKey(p => p.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(i => i.Codigo).IsUnique();
                entity.HasIndex(i => i.Nombre);
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

                entity.Property(u => u.Id).ValueGeneratedNever();

                entity.Property(u => u.Codigo).IsRequired().HasMaxLength(20);
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Simbolo).IsRequired().HasMaxLength(20);

                entity.Property(u => u.EsFraccionable).IsRequired().HasDefaultValue(true);
                entity.Property(u => u.Activo).IsRequired().HasDefaultValue(true);

                entity.HasIndex(u => u.Codigo).IsUnique();
                entity.HasIndex(u => u.Activo);

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

                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Codigo).HasMaxLength(50);

                entity.Property(c => c.Activo).IsRequired().HasDefaultValue(true);
                entity.Property(c => c.FechaCreacion).IsRequired();

                entity.HasIndex(c => c.Nombre).IsUnique();
                entity.HasIndex(c => c.Codigo);
                entity.HasIndex(c => c.Activo);

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
                entity.Property(p => p.Activo).IsRequired().HasDefaultValue(true);

                entity.HasMany(p => p.Insumos)
                    .WithOne(i => i.PrincipioActivo)
                    .HasForeignKey(i => i.PrincipioActivoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.Nombre).IsUnique();
                entity.HasIndex(p => p.Activo);
            });

            builder.Entity<InsumoPrincipioActivo>(entity =>
            {
                entity.ToTable("InsumosPrincipiosActivos");
                entity.HasKey(ipa => new { ipa.InsumoId, ipa.PrincipioActivoId });

                entity.Property(ipa => ipa.Concentracion)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue(string.Empty);

                entity.HasOne(ipa => ipa.Insumo)
                    .WithMany(i => i.PrincipiosActivos)
                    .HasForeignKey(ipa => ipa.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ipa => ipa.PrincipioActivo)
                    .WithMany(pa => pa.Insumos)
                    .HasForeignKey(ipa => ipa.PrincipioActivoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ipa => ipa.InsumoId);
                entity.HasIndex(ipa => ipa.PrincipioActivoId);
            });

            builder.Entity<ServicioInsumoReceta>(entity =>
            {
                entity.ToTable("ServiciosInsumoRecetas");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Cantidad).HasPrecision(18, 4).IsRequired();

                entity.HasOne(r => r.ServicioClinico)
                    .WithMany()
                    .HasForeignKey(r => r.ServicioClinicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Insumo)
                    .WithMany()
                    .HasForeignKey(r => r.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.UnidadMedidaNav)
                    .WithMany()
                    .HasForeignKey(r => r.UnidadMedidaConsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => r.ServicioClinicoId);
                entity.HasIndex(r => r.InsumoId);
                entity.HasIndex(r => r.UnidadMedidaConsumoId);
                entity.HasIndex(r => new { r.ServicioClinicoId, r.InsumoId }).IsUnique();
            });

            builder.Entity<ConsumoServicioRealizado>(entity =>
            {
                entity.ToTable("ConsumosServiciosRealizados");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.CantidadConsumidaBase).HasPrecision(18, 4).IsRequired();
                entity.Property(c => c.CostoTotalUSD).HasPrecision(18, 2).IsRequired();
                entity.Property(c => c.FechaConsumo).IsRequired();

                entity.HasOne(c => c.DetalleServicioCuenta)
                    .WithMany()
                    .HasForeignKey(c => c.DetalleServicioCuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Insumo)
                    .WithMany()
                    .HasForeignKey(c => c.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.DetalleServicioCuentaId);
                entity.HasIndex(c => c.InsumoId);
                entity.HasIndex(c => c.FechaConsumo);
            });

            builder.Entity<MovimientoInsumo>(entity =>
            {
                entity.ToTable("MovimientosInsumo");
                entity.HasKey(m => m.Id);

                entity.Property(m => m.TipoMovimiento).HasConversion<int>().IsRequired();
                entity.Property(m => m.CantidadBase).HasPrecision(18, 4).IsRequired();
                entity.Property(m => m.CantidadOriginal).HasPrecision(18, 4).IsRequired();

                entity.Property(m => m.Motivo).HasMaxLength(500);
                entity.Property(m => m.UsuarioId).HasMaxLength(100);
                entity.Property(m => m.Fecha).IsRequired();

                entity.HasOne(m => m.Insumo)
                    .WithMany()
                    .HasForeignKey(m => m.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Sede)
                    .WithMany()
                    .HasForeignKey(m => m.SedeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.UnidadMedidaNav)
                    .WithMany()
                    .HasForeignKey(m => m.UnidadMedidaOriginalId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(m => m.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(m => m.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(m => m.InsumoId);
                entity.HasIndex(m => m.SedeId);
                entity.HasIndex(m => m.UnidadMedidaOriginalId);
                entity.HasIndex(m => m.UsuarioIdentityId);
                entity.HasIndex(m => m.TipoMovimiento);
                entity.HasIndex(m => m.Fecha);
                entity.HasIndex(m => new { m.InsumoId, m.SedeId, m.Fecha });
            });

            builder.Entity<CierreInventario>(entity =>
            {
                entity.ToTable("CierresInventario");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Observaciones).HasMaxLength(1000);
                entity.Property(c => c.Usuario).HasMaxLength(100);
                entity.Property(c => c.FechaCierre).IsRequired();

                entity.HasOne(c => c.Sede)
                    .WithMany()
                    .HasForeignKey(c => c.SedeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Detalles)
                    .WithOne(d => d.CierreInventario)
                    .HasForeignKey(d => d.CierreInventarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.UsuarioId).HasColumnType("char(36)");

                entity.HasIndex(c => c.SedeId);
                entity.HasIndex(c => c.UsuarioId);
                entity.HasIndex(c => c.FechaCierre);
                entity.HasIndex(c => new { c.SedeId, c.FechaCierre });
            });

            builder.Entity<CierreInventarioDetalle>(entity =>
            {
                entity.ToTable("CierresInventarioDetalles");
                entity.HasKey(d => d.Id);

                entity.Property(d => d.StockTeoricoBase).HasPrecision(18, 4).IsRequired();
                entity.Property(d => d.StockRealBase).HasPrecision(18, 4).IsRequired();
                entity.Property(d => d.CostoBaseUSD).HasPrecision(18, 4).IsRequired();

                entity.HasOne(d => d.CierreInventario)
                    .WithMany(c => c.Detalles)
                    .HasForeignKey(d => d.CierreInventarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Insumo)
                    .WithMany()
                    .HasForeignKey(d => d.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(d => d.CierreInventarioId);
                entity.HasIndex(d => d.InsumoId);
                entity.HasIndex(d => new { d.CierreInventarioId, d.InsumoId }).IsUnique();
            });

            builder.Entity<Sede>(entity =>
            {
                entity.ToTable("Sedes");
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(s => s.EsPrincipal).IsRequired().HasDefaultValue(false);
                entity.Property(s => s.Activo).IsRequired().HasDefaultValue(true);

                entity.HasMany(s => s.AreasClinicas)
                    .WithOne(a => a.Sede)
                    .HasForeignKey(a => a.SedeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.Codigo).IsUnique();
                entity.HasIndex(s => s.Nombre);
                entity.HasIndex(s => s.Activo);
                entity.HasIndex(s => s.EsPrincipal);

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

                entity.Property(c => c.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Descripcion).IsRequired().HasMaxLength(150);

                entity.HasIndex(c => c.Codigo).IsUnique();
                entity.HasIndex(c => c.Descripcion);

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
                    .OnDelete(DeleteBehavior.Restrict);

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
                entity.HasKey(sia => new { sia.AreaClinicaId, sia.ServicioClinicoId });

                entity.Property(sia => sia.Activo).IsRequired().HasDefaultValue(true);

                entity.HasOne(sia => sia.AreaClinica)
                    .WithMany()
                    .HasForeignKey(sia => sia.AreaClinicaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sia => sia.ServicioClinico)
                    .WithMany()
                    .HasForeignKey(sia => sia.ServicioClinicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(sia => sia.AreaClinicaId);
                entity.HasIndex(sia => sia.ServicioClinicoId);
                entity.HasIndex(sia => new { sia.AreaClinicaId, sia.Activo });
            });

            builder.Entity<InsumoCirugiaPaciente>(entity =>
            {
                entity.ToTable("InsumosCirugiaPaciente");
                entity.HasKey(icp => icp.Id);

                entity.Ignore(icp => icp.CantidadConsumida);

                entity.Property(icp => icp.CantidadEntregada).HasPrecision(18, 4).IsRequired();
                entity.Property(icp => icp.CantidadDevuelta).HasPrecision(18, 4).IsRequired().HasDefaultValue(0.0000m);

                entity.HasOne(icp => icp.CuentaServicio)
                    .WithMany()
                    .HasForeignKey(icp => icp.CuentaServicioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(icp => icp.OrdenCirugia)
                    .WithMany()
                    .HasForeignKey(icp => icp.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(icp => icp.Insumo)
                    .WithMany()
                    .HasForeignKey(icp => icp.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(icp => icp.CuentaServicioId);
                entity.HasIndex(icp => icp.OrdenCirugiaId);
                entity.HasIndex(icp => icp.InsumoId);
                entity.HasIndex(icp => new { icp.CuentaServicioId, icp.InsumoId });
            });

            builder.Entity<StockSede>(entity =>
            {
                entity.ToTable("StocksSede");
                entity.HasKey(s => s.Id);

                entity.Property(s => s.StockActual).HasPrecision(18, 4).IsRequired().HasDefaultValue(0.0000m);
                entity.Property(s => s.StockMinimo).HasPrecision(18, 4);
                entity.Property(s => s.StockMaximo).HasPrecision(18, 4);

                entity.Property(s => s.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(s => s.Insumo)
                    .WithMany(i => i.StocksPorSede)
                    .HasForeignKey(s => s.InsumoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Sede)
                    .WithMany()
                    .HasForeignKey(s => s.SedeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.InsumoId);
                entity.HasIndex(s => s.SedeId);
                entity.HasIndex(s => new { s.InsumoId, s.SedeId }).IsUnique();
                entity.HasIndex(s => new { s.SedeId, s.StockActual });
            });

            builder.Entity<PedidoInterSede>(entity =>
            {
                entity.ToTable("PedidosInterSede");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Correlativo).IsRequired().HasMaxLength(50);
                entity.Property(p => p.UsuarioCreador).HasMaxLength(100);
                entity.Property(p => p.Observaciones).HasMaxLength(1000);
                entity.Property(p => p.Estado).HasConversion<int>().IsRequired();
                entity.Property(p => p.FechaCreacion).IsRequired();

                entity.HasOne(p => p.SedeSolicitante)
                    .WithMany()
                    .HasForeignKey(p => p.SedeSolicitanteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.SedeProveedora)
                    .WithMany()
                    .HasForeignKey(p => p.SedeProveedoraId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Detalles)
                    .WithOne(d => d.PedidoInterSede)
                    .HasForeignKey(d => d.PedidoInterSedeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(p => p.UsuarioCreadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.UsuarioCreadorId).HasColumnType("char(36)");

                entity.HasIndex(p => p.Correlativo).IsUnique();
                entity.HasIndex(p => p.SedeSolicitanteId);
                entity.HasIndex(p => p.SedeProveedoraId);
                entity.HasIndex(p => p.UsuarioCreadorId);
                entity.HasIndex(p => p.Estado);
                entity.HasIndex(p => p.FechaCreacion);
                entity.HasIndex(p => new { p.SedeSolicitanteId, p.Estado });
                entity.HasIndex(p => new { p.SedeProveedoraId, p.Estado });
            });

            builder.Entity<PedidoInterSedeDetalle>(entity =>
            {
                entity.ToTable("PedidosInterSedeDetalles");
                entity.HasKey(d => d.Id);

                entity.Property(d => d.CantidadSolicitada).HasPrecision(18, 4).IsRequired();
                entity.Property(d => d.CantidadDespachada).HasPrecision(18, 4).IsRequired().HasDefaultValue(0.0000m);
                entity.Property(d => d.CantidadRecibida).HasPrecision(18, 4).IsRequired().HasDefaultValue(0.0000m);
                entity.Property(d => d.ObservacionDespacho).HasMaxLength(500);

                entity.HasOne(d => d.PedidoInterSede)
                    .WithMany(p => p.Detalles)
                    .HasForeignKey(d => d.PedidoInterSedeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Insumo)
                    .WithMany()
                    .HasForeignKey(d => d.InsumoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(d => d.PedidoInterSedeId);
                entity.HasIndex(d => d.InsumoId);
                entity.HasIndex(d => new { d.PedidoInterSedeId, d.InsumoId }).IsUnique();
            });

            builder.Entity<DetalleServicioMedicoResponsable>(entity =>
            {
                entity.ToTable("DetalleServiciosMedicosResponsables");
                entity.HasKey(dsm => dsm.Id);

                entity.Property(dsm => dsm.Rol).IsRequired().HasMaxLength(100);
                entity.Property(dsm => dsm.MontoHonorario).HasPrecision(18, 2).IsRequired().HasDefaultValue(0.00m);

                entity.HasOne(dsm => dsm.DetalleServicioCuenta)
                    .WithMany(dsc => dsc.MedicosResponsables)
                    .HasForeignKey(dsm => dsm.DetalleServicioCuentaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(dsm => dsm.Medico)
                    .WithMany()
                    .HasForeignKey(dsm => dsm.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(dsm => dsm.DetalleServicioCuentaId);
                entity.HasIndex(dsm => dsm.MedicoId);
                entity.HasIndex(dsm => dsm.Rol);
                entity.HasIndex(dsm => new { dsm.MedicoId, dsm.MontoHonorario });
            });

            builder.Entity<TipoServicio>(entity =>
            {
                entity.ToTable("TiposServicio");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).ValueGeneratedNever();

                entity.Property(t => t.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Nombre).IsRequired().HasMaxLength(100);

                entity.HasIndex(t => t.Codigo).IsUnique();
                entity.HasIndex(t => t.Nombre);

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

                entity.Ignore(o => o.CurrentState);

                entity.HasMany(o => o.Logs)
                    .WithOne(l => l.OrdenCirugia)
                    .HasForeignKey(l => l.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.Logs)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(o => o.Requisitos)
                    .WithOne(r => r.OrdenCirugia)
                    .HasForeignKey(r => r.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.Requisitos)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(o => o.HistorialObservaciones)
                    .WithOne(h => h.OrdenCirugia)
                    .HasForeignKey(h => h.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.HistorialObservaciones)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(o => o.MedicosHonorarios)
                    .WithOne(m => m.OrdenCirugia)
                    .HasForeignKey(m => m.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.MedicosHonorarios)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasMany(o => o.SolicitudesInsumos)
                    .WithOne(s => s.OrdenCirugia)
                    .HasForeignKey(s => s.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(o => o.SolicitudesInsumos)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            builder.Entity<CirugiaMedicoHonorario>(entity =>
            {
                entity.ToTable("CirugiaMedicosHonorarios");
                entity.HasKey(cmh => cmh.Id);

                entity.Property(cmh => cmh.MontoHonorarioUsd).HasPrecision(18, 2).IsRequired().HasDefaultValue(0.00m);
                entity.Property(cmh => cmh.EsCirujanoPrincipal).IsRequired().HasDefaultValue(false);

                entity.HasOne(cmh => cmh.OrdenCirugia)
                    .WithMany(o => o.MedicosHonorarios)
                    .HasForeignKey(cmh => cmh.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cmh => cmh.Medico)
                    .WithMany()
                    .HasForeignKey(cmh => cmh.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cmh => cmh.Especialidad)
                    .WithMany()
                    .HasForeignKey(cmh => cmh.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(cmh => cmh.OrdenCirugiaId);
                entity.HasIndex(cmh => cmh.MedicoId);
                entity.HasIndex(cmh => cmh.EspecialidadId);
                entity.HasIndex(cmh => new { cmh.OrdenCirugiaId, cmh.MedicoId }).IsUnique();
                entity.HasIndex(cmh => new { cmh.MedicoId, cmh.MontoHonorarioUsd });
            });

            builder.Entity<TransferenciaReposicionStock>(entity =>
            {
                entity.ToTable("TransferenciasReposicionStock");
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Motivo).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Observaciones).HasMaxLength(1000);
                entity.Property(t => t.Cantidad).HasPrecision(18, 4).IsRequired();
                entity.Property(t => t.FechaTransferencia).IsRequired();

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

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(t => t.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(t => t.InsumoId);
                entity.HasIndex(t => t.SedeOrigenId);
                entity.HasIndex(t => t.SedeDestinoId);
                entity.HasIndex(t => t.UsuarioIdentityId);
                entity.HasIndex(t => t.FechaTransferencia);
                entity.HasIndex(t => new { t.SedeOrigenId, t.SedeDestinoId, t.FechaTransferencia });
            });

            builder.Entity<CirugiaLog>(entity =>
            {
                entity.ToTable("CirugiaLogs");
                entity.HasKey(cl => cl.Id);

                entity.Property(cl => cl.Evento).IsRequired().HasMaxLength(100);
                entity.Property(cl => cl.Detalle).HasMaxLength(1000);
                entity.Property(cl => cl.UsuarioId).HasMaxLength(100);
                entity.Property(cl => cl.Timestamp).IsRequired();

                entity.HasOne(cl => cl.OrdenCirugia)
                    .WithMany(o => o.Logs)
                    .HasForeignKey(cl => cl.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<UsuarioHospital>()
                    .WithMany()
                    .HasForeignKey(cl => cl.UsuarioIdentityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(cl => cl.UsuarioIdentityId).HasColumnType("char(36)");

                entity.HasIndex(cl => cl.OrdenCirugiaId);
                entity.HasIndex(cl => cl.UsuarioIdentityId);
                entity.HasIndex(cl => cl.Timestamp);
                entity.HasIndex(cl => new { cl.OrdenCirugiaId, cl.Timestamp });
            });

            builder.Entity<RequisitoCirugia>(entity =>
            {
                entity.ToTable("RequisitosCirugia");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(r => r.Descripcion).HasMaxLength(500);
                entity.Property(r => r.EsActivo).IsRequired().HasDefaultValue(true);
                entity.Property(r => r.FechaCreacion).IsRequired();

                entity.HasMany(r => r.OrdenesRequisitos)
                    .WithOne(or => or.RequisitoCirugia)
                    .HasForeignKey(or => or.RequisitoCirugiaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Navigation(r => r.OrdenesRequisitos)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasIndex(r => r.Nombre).IsUnique();
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

                entity.Property(ocr => ocr.VerificadoPor).HasMaxLength(100);
                entity.Property(ocr => ocr.Cumplido).IsRequired().HasDefaultValue(false);

                entity.HasOne(ocr => ocr.OrdenCirugia)
                    .WithMany(o => o.Requisitos)
                    .HasForeignKey(ocr => ocr.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ocr => ocr.RequisitoCirugia)
                    .WithMany(r => r.OrdenesRequisitos)
                    .HasForeignKey(ocr => ocr.RequisitoCirugiaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ocr => ocr.OrdenCirugiaId);
                entity.HasIndex(ocr => ocr.RequisitoCirugiaId);
                entity.HasIndex(ocr => ocr.Cumplido);
                entity.HasIndex(ocr => new { ocr.OrdenCirugiaId, ocr.RequisitoCirugiaId }).IsUnique();
            });

            builder.Entity<CirugiaObservacionHistorial>(entity =>
            {
                entity.ToTable("CirugiaObservacionesHistorial");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Observacion).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Tipo).IsRequired().HasConversion<string>().HasMaxLength(50);
                entity.Property(e => e.FechaRegistro).IsRequired();
                entity.Property(e => e.UsuarioRegistroId).IsRequired(false);

                entity.HasOne(e => e.OrdenCirugia)
                    .WithMany(o => o.HistorialObservaciones)
                    .HasForeignKey(e => e.OrdenCirugiaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.OrdenCirugiaId);
                entity.HasIndex(e => e.UsuarioRegistroId);
            });

            builder.Entity<OrdenCompraInventario>(entity =>
            {
                entity.ToTable("OrdenesCompraInventario");
                entity.HasKey(o => o.Id);

                entity.Property(o => o.NumeroFactura).IsRequired().HasMaxLength(100);
                entity.Property(o => o.FechaEmision).IsRequired();
                entity.Property(o => o.MontoTotalUSD).IsRequired().HasPrecision(18, 2);
                entity.Property(o => o.Estado).IsRequired().HasMaxLength(50);
                entity.Property(o => o.Observaciones).HasMaxLength(1000);

                entity.Ignore(o => o.TotalAbonadoUSD);
                entity.Ignore(o => o.SaldoPendienteUSD);

                entity.HasOne(o => o.Proveedor)
                    .WithMany()
                    .HasForeignKey(o => o.ProveedorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(o => o.Pagos)
                    .WithOne(p => p.OrdenCompra)
                    .HasForeignKey(p => p.OrdenCompraId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Navigation(o => o.Pagos)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                entity.HasIndex(o => o.NumeroFactura);
                entity.HasIndex(o => o.Estado);
                entity.HasIndex(o => o.ProveedorId);
            });

            builder.Entity<PagoProveedor>(entity =>
            {
                entity.ToTable("PagosProveedores");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.FechaPago).IsRequired();
                entity.Property(p => p.MontoAbonadoUSD).IsRequired().HasPrecision(18, 2);
                entity.Property(p => p.TasaCambio).IsRequired().HasPrecision(18, 4);
                entity.Ignore(p => p.MontoAbonadoBs);

                entity.Property(p => p.MetodoPago).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Referencia).HasMaxLength(100);
                entity.Property(p => p.UsuarioIdentityId).IsRequired(false);
                entity.Property(p => p.Observaciones).HasMaxLength(1000);

                entity.HasOne(p => p.OrdenCompra)
                    .WithMany(o => o.Pagos)
                    .HasForeignKey(p => p.OrdenCompraId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.OrdenCompraId);
                entity.HasIndex(p => p.FechaPago);
                entity.HasIndex(p => p.UsuarioIdentityId);
            });

            builder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("Proveedores");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.RIF).IsRequired().HasMaxLength(30);
                entity.Property(p => p.RazonSocial).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Direccion).HasMaxLength(500);
                entity.Property(p => p.Telefono).HasMaxLength(50);
                entity.Property(p => p.Activo).IsRequired().HasDefaultValue(true);
                entity.Property(p => p.FechaRegistro).IsRequired();

                entity.HasIndex(p => p.RIF).IsUnique();
                entity.HasIndex(p => p.RazonSocial);
                entity.HasIndex(p => p.Activo);
            });

             // 🛡️ ESCUDO UNIVERSAL CONTRA COLECCIONES NO RELACIONALES (EF Core 9 / MySQL)
            // =========================================================================
            // Detecta cualquier propiedad que sea una lista/colección en memoria que EF Core
            // haya catalogado erróneamente como columna de tabla en lugar de navegación,
            // e impide que Pomelo MySQL intente llamar a FindCollectionMapping y reviente.
            foreach (var entityType in builder.Model.GetEntityTypes().ToList())
            {
                var coleccionesHuerfanas = entityType.GetProperties()
                    .Where(p => p.ClrType != typeof(string) 
                             && p.ClrType != typeof(byte[]) 
                             && typeof(System.Collections.IEnumerable).IsAssignableFrom(p.ClrType)
                             && p.GetValueConverter() == null)
                    .Select(p => p.Name)
                    .ToList();

                foreach (var propName in coleccionesHuerfanas)
                {
                    builder.Entity(entityType.ClrType).Ignore(propName);
                }
            }
        }
    }
}