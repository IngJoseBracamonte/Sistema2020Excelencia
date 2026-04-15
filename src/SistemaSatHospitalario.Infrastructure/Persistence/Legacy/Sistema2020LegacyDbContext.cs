using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Legacy
{
    public class Sistema2020LegacyDbContext : DbContext
    {
        public Sistema2020LegacyDbContext(DbContextOptions<Sistema2020LegacyDbContext> options) : base(options) { }

        public DbSet<OrdenLegacy> Orden { get; set; }
        public DbSet<ResultadosPacienteLegacy> ResultadosPaciente { get; set; }
        public DbSet<DatosPersonalesLegacy> DatosPersonales { get; set; }
        public DbSet<PerfilLegacy> Perfil { get; set; }
        public DbSet<PerfilesAnalisisLegacy> PerfilesAnalisis { get; set; }
        public DbSet<PerfilesFacturadosLegacy> PerfilesFacturados { get; set; }
        public DbSet<ConvenioEmpresaLegacy> ConveniosEmpresa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrdenLegacy>(entity =>
            {
                entity.ToTable("ordenes");
                entity.HasKey(e => e.IdOrden);
                entity.Property(e => e.IdOrden).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ResultadosPacienteLegacy>(entity =>
            {
                entity.ToTable("resultadospaciente");
                entity.HasKey(e => e.IdResultadoPaciente);
                entity.Property(e => e.IdResultadoPaciente).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DatosPersonalesLegacy>(entity =>
            {
                entity.ToTable("datospersonales");
                entity.HasKey(e => e.IdPersona);
                entity.Property(e => e.IdPersona).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PerfilLegacy>(entity =>
            {
                entity.ToTable("perfil");
                entity.HasKey(e => e.IdPerfil);
                entity.Property(e => e.IdPerfil).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PerfilesAnalisisLegacy>(entity =>
            {
                entity.ToTable("perfilesanalisis");
                entity.HasKey(e => e.IdDetalle);
                entity.Property(e => e.IdDetalle).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PerfilesFacturadosLegacy>(entity =>
            {
                entity.ToTable("perfilesfacturados");
                entity.HasKey(e => e.IdFacturado);
                entity.Property(e => e.IdFacturado).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ConvenioEmpresaLegacy>(entity =>
            {
                entity.ToTable("convenios");
                entity.HasKey(e => e.IDConvenio);
                entity.Property(e => e.IDConvenio).ValueGeneratedOnAdd();
            });
        }
    }
}
