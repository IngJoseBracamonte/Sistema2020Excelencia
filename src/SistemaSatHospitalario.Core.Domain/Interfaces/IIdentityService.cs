using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IIdentityService
    {
        Task<List<UserDto>> GetUsersAsync();
        Task<List<RoleDto>> GetRolesListAsync();
        Task<List<string>> GetRolesAsync(); // legacy compatibility
        Task<bool> CreateUserAsync(string username, string email, string password, List<string> roles);
        Task<bool> UpdateUserRolesAsync(Guid userId, List<string> roles);
        
        // RBAC Management
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleName);
        Task<List<string>> GetPermissionsByRoleAsync(string roleName);
        Task<bool> UpdateRolePermissionsAsync(string roleName, List<string> permissions);

        // User-Level Permission Management (Granular Overrides)
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
        Task<bool> UpdateUserPermissionsAsync(Guid userId, List<string> permissions);

        // Password Reset Workflow
        Task<bool> RequestPasswordResetAsync(string username);
        Task<List<PasswordResetRequestDto>> GetPendingResetRequestsAsync();
        Task<bool> ApprovePasswordResetAsync(Guid requestId, string adminUser);
        Task<bool> CompletePasswordResetAsync(string username, string newPassword);
    }

    public class PasswordResetRequestDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; }
    }

    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool EsActivo { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
