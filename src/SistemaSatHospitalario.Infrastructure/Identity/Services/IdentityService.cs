using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SistemaSatHospitalario.Infrastructure.Identity.Contexts;

namespace SistemaSatHospitalario.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<UsuarioHospital> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SatHospitalarioIdentityDbContext _context;

        public IdentityService(
            UserManager<UsuarioHospital> userManager, 
            RoleManager<IdentityRole<Guid>> roleManager,
            SatHospitalarioIdentityDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    FullName = $"{(user.NombreReal ?? "Usuario")} {(user.ApellidoReal ?? "Hospital")}".Trim(),
                    EsActivo = user.EsActivo,
                    Roles = roles.ToList(),
                    Permissions = (await _userManager.GetClaimsAsync(user))
                        .Where(c => c.Type == PermissionConstants.Type)
                        .Select(c => c.Value)
                        .ToList()
                });
            }

            return userDtos;
        }

        public async Task<List<RoleDto>> GetRolesListAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var dtos = new List<RoleDto>();
            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                dtos.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = claims.Where(c => c.Type == PermissionConstants.Type).Select(c => c.Value).ToList()
                });
            }
            return dtos;
        }

        public async Task<List<string>> GetRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<bool> CreateUserAsync(string username, string email, string password, List<string> roles)
        {
            var user = new UsuarioHospital 
            { 
                UserName = username, 
                Email = email,
                NombreReal = username,
                ApellidoReal = "Personal",
                EsActivo = true,
                EmailConfirmed = true
            };
            
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (roles != null && roles.Any())
                {
                    foreach (var role in roles)
                    {
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                }
                return true;
            }
            
            var errorMsg = string.Join(" | ", result.Errors.Select(e => e.Description));
            throw new Exception(errorMsg);
        }

        public async Task<bool> UpdateUserRolesAsync(Guid userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, roles);
            return true;
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName)) return false;
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            return true;
        }

        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return false;
            await _roleManager.DeleteAsync(role);
            return true;
        }

        public async Task<List<string>> GetPermissionsByRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return new List<string>();
            
            var claims = await _roleManager.GetClaimsAsync(role);
            return claims.Where(c => c.Type == PermissionConstants.Type).Select(c => c.Value).ToList();
        }

        public async Task<bool> UpdateRolePermissionsAsync(string roleName, List<string> permissions)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return false;

            var currentClaims = await _roleManager.GetClaimsAsync(role);
            var permissionClaims = currentClaims.Where(c => c.Type == PermissionConstants.Type);

            foreach (var claim in permissionClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var permission in permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim(PermissionConstants.Type, permission));
            }

            return true;
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new List<string>();

            var claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(c => c.Type == PermissionConstants.Type).Select(c => c.Value).ToList();
        }

        public async Task<bool> UpdateUserPermissionsAsync(Guid userId, List<string> permissions)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var currentClaims = await _userManager.GetClaimsAsync(user);
            var permissionClaims = currentClaims.Where(c => c.Type == PermissionConstants.Type);

            foreach (var claim in permissionClaims)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }

            foreach (var permission in permissions)
            {
                await _userManager.AddClaimAsync(user, new Claim(PermissionConstants.Type, permission));
            }

            return true;
        }

        // Password Reset Workflow Implementation
        public async Task<bool> RequestPasswordResetAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;

            // Check if there is already a pending request
            var existing = await _context.PasswordResetRequests
                .AnyAsync(x => x.UsuarioId == user.Id && x.Estado == "Pendiente");
            
            if (existing) return true; // Already requested

            var request = new PasswordResetRequest
            {
                UsuarioId = user.Id,
                Username = user.UserName!,
                Estado = "Pendiente"
            };

            _context.PasswordResetRequests.Add(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PasswordResetRequestDto>> GetPendingResetRequestsAsync()
        {
            return await _context.PasswordResetRequests
                .Where(x => x.Estado == "Pendiente")
                .OrderByDescending(x => x.FechaSolicitud)
                .Select(x => new PasswordResetRequestDto
                {
                    Id = x.Id,
                    Username = x.Username,
                    FechaSolicitud = x.FechaSolicitud,
                    Estado = x.Estado
                })
                .ToListAsync();
        }

        public async Task<bool> ApprovePasswordResetAsync(Guid requestId, string adminUser)
        {
            var request = await _context.PasswordResetRequests.FindAsync(requestId);
            if (request == null) return false;

            var user = await _userManager.FindByIdAsync(request.UsuarioId.ToString());
            if (user == null) return false;

            request.Estado = "Aprobado";
            request.FechaProcesado = DateTime.UtcNow;
            request.ProcesadoPor = adminUser;

            user.RequirePasswordReset = true;
            await _userManager.UpdateAsync(user);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompletePasswordResetAsync(string username, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;

            // Remove existing password and set new one
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, newPassword);
            
            if (result.Succeeded)
            {
                user.RequirePasswordReset = false;
                await _userManager.UpdateAsync(user);

                // Mark requests as completed
                var requests = await _context.PasswordResetRequests
                    .Where(x => x.UsuarioId == user.Id && x.Estado == "Aprobado")
                    .ToListAsync();

                foreach (var r in requests)
                {
                    r.Estado = "Completado";
                }

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
