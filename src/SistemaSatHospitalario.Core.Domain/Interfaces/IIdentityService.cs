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
        public List<string> Roles { get; set; }
    }
}
