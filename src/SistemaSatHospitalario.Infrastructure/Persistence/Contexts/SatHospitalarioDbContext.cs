using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Contexts
{
    public class SatHospitalarioDbContext : DbContext
    {
        public DbSet<CajaDiaria> CajasDiarias { get; set; }
        public DbSet<ReciboFactura> RecibosFactura { get; set; }
        public DbSet<DetallePago> DetallesPago { get; set; }
        public DbSet<SeguroConvenio> SegurosConvenio { get; set; }
        public DbSet<PacienteAdmision> PacientesAdmision { get; set; }
        public DbSet<OrdenDeServicio> OrdenesDeServicio { get; set; }
        public DbSet<OrdenRX> OrdenesRX { get; set; } // TPH o TPT

        public DbSet<TurnoMedico> TurnosMedicos { get; set; }
        public DbSet<IncidenciaHorario> IncidenciasHorario { get; set; }
        public DbSet<RegistroAuditoriaIncidencia> RegistrosAuditoriaIncidencia { get; set; }

        public DbSet<CuentaServicios> CuentasServicios { get; set; }
        public DbSet<DetalleServicioCuenta> DetallesServicioCuenta { get; set; }
        public DbSet<CitaMedica> CitasMedicas { get; set; }

        public SatHospitalarioDbContext(DbContextOptions<SatHospitalarioDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("Admision");

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
                entity.Property(s => s.PorcentajeCobertura).HasPrecision(5, 2);
            });

            builder.Entity<PacienteAdmision>(entity =>
            {
                entity.ToTable("PacientesAdmision");
                entity.HasKey(p => p.Id);
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
        }
    }
}
