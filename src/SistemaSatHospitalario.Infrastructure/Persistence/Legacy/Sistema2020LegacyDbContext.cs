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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrdenLegacy>(entity =>
            {
                entity.ToTable("Orden");
                entity.HasKey(e => e.IdOrden);
            });

            modelBuilder.Entity<ResultadosPacienteLegacy>(entity =>
            {
                entity.ToTable("ResultadosPaciente");
                entity.HasKey(e => new { e.IdOrden, e.IdPaciente, e.IdAnalisis }); 
            });

            modelBuilder.Entity<DatosPersonalesLegacy>(entity =>
            {
                entity.ToTable("datospersonales");
                entity.HasKey(e => e.IdPersona);
            });

            modelBuilder.Entity<PerfilLegacy>(entity =>
            {
                entity.ToTable("Perfiles"); // El usuario indicó que la tabla se llama 'Perfiles' pero su entidad singular es 'Perfil'
                entity.HasKey(e => e.IdPerfil);
            });

            modelBuilder.Entity<PerfilesAnalisisLegacy>(entity =>
            {
                entity.ToTable("PerfilesAnalisis");
                entity.HasKey(e => e.IdDetalle); 
            });

            modelBuilder.Entity<PerfilesFacturadosLegacy>(entity =>
            {
                entity.ToTable("PerfilesFacturados");
                entity.HasKey(e => e.IdFacturado);
            });
        }
    }
}
