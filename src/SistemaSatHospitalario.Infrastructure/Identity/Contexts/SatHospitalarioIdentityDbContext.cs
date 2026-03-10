using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Identity.Models;

namespace SistemaSatHospitalario.Infrastructure.Identity.Contexts
{
    public class SatHospitalarioIdentityDbContext : IdentityDbContext<UsuarioHospital, IdentityRole<Guid>, Guid>
    {
        public SatHospitalarioIdentityDbContext(DbContextOptions<SatHospitalarioIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Personalización opcional del esquema genérico de Identity
            builder.HasDefaultSchema("Identity");

            builder.Entity<UsuarioHospital>(entity =>
            {
                entity.ToTable("Usuarios");
            });

            builder.Entity<IdentityRole<Guid>>(entity =>
            {
                entity.ToTable("Roles");
            });

            builder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UsuarioRoles");
            });

            builder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UsuarioClaims");
            });

            builder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UsuarioLogins");
            });

            builder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UsuarioTokens");
            });
        }
    }
}
