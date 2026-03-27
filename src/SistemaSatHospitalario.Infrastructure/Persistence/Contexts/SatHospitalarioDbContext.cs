using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

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
        public DbSet<PrecioServicioConvenio> PreciosServicioConvenio { get; set; }
        public DbSet<CuentaPorCobrar> CuentasPorCobrar { get; set; }
        public DbSet<ReservaTemporal> ReservasTemporales { get; set; }
        public DbSet<BloqueoHorario> BloqueosHorarios { get; set; }
        public DbSet<ErrorTicket> ErrorTickets { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<ConfiguracionGeneral> ConfiguracionGeneral { get; set; }
        public DbSet<ConvenioPerfilPrecio> ConvenioPerfilPrecios { get; set; }

        public SatHospitalarioDbContext(DbContextOptions<SatHospitalarioDbContext> options) : base(options) { }
        public Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) => Database.BeginTransactionAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // MySQL no soporta esquemas, se ignora para compatibilidad multi-proveedor
            // builder.HasDefaultSchema("Admision");

            builder.Entity<CajaDiaria>(entity =>
            {
                entity.ToTable("CajasDiarias");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.MontoInicialDivisa).HasPrecision(18, 2);
                entity.Property(c => c.MontoInicialBs).HasPrecision(18, 2);
            });



            builder.Entity<ReciboFactura>(entity =>
            {
                entity.ToTable("RecibosFacturas");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.TasaCambioDia).HasPrecision(18, 4);
                
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

                entity.HasOne(d => d.ReciboFactura)
                      .WithMany(r => r.DetallesPago)
                      .HasForeignKey(d => d.ReciboFacturaId)
                      .OnDelete(DeleteBehavior.Cascade);
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
                      .WithOne()
                      .HasForeignKey(d => d.CuentaServicioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<DetalleServicioCuenta>(entity =>
            {
                entity.ToTable("DetallesServicioCuenta");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Precio).HasPrecision(18, 2);
            });

            builder.Entity<CitaMedica>(entity =>
            {
                entity.ToTable("CitasMedicas");
                entity.HasKey(c => c.Id);
            });

            builder.Entity<Medico>(entity =>
            {
                entity.ToTable("Medicos");
                entity.HasKey(m => m.Id);
            });

            builder.Entity<ServicioClinico>(entity =>
            {
                entity.ToTable("ServiciosClinicos");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.PrecioBase).HasPrecision(18, 2);
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
            });

            builder.Entity<TasaCambio>(entity =>
            {
                entity.ToTable("tasacambio"); // Forzar minúsculas para compatibilidad MySQL/Linux
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
            });
        }
    }
}
