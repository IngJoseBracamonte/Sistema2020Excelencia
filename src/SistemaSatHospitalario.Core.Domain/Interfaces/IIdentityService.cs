using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Domain.Interfaces
{
    public interface IIdentityService
    {
        Task<List<UserDto>> GetUsersAsync();
        Task<List<string>> GetRolesAsync();
        Task<bool> CreateUserAsync(string username, string email, string password, List<string> roles);
        Task<bool> UpdateUserRolesAsync(Guid userId, List<string> roles);
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
