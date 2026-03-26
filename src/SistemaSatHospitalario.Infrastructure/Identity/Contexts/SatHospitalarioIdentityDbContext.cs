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
            // MySQL no soporta esquemas, se ignora para compatibilidad multi-proveedor
            // builder.HasDefaultSchema("Identity");

            builder.Entity<UsuarioHospital>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.Property(u => u.Id).HasColumnType("char(36)");
            });

            builder.Entity<IdentityRole<Guid>>(entity =>
            {
                entity.ToTable("Roles");
                entity.Property(r => r.Id).HasColumnType("char(36)");
            });

            builder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UsuarioRoles");
                entity.Property(ur => ur.UserId).HasColumnType("char(36)");
                entity.Property(ur => ur.RoleId).HasColumnType("char(36)");
            });

            builder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UsuarioClaims");
                entity.Property(uc => uc.UserId).HasColumnType("char(36)");
            });

            builder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UsuarioLogins");
                entity.Property(ul => ul.UserId).HasColumnType("char(36)");
            });

            builder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims");
                entity.Property(rc => rc.RoleId).HasColumnType("char(36)");
            });

            builder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UsuarioTokens");
                entity.Property(ut => ut.UserId).HasColumnType("char(36)");
            });
        }
    }
}
