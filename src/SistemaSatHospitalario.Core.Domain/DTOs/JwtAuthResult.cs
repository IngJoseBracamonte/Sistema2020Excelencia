using System;

namespace SistemaSatHospitalario.Core.Domain.DTOs
{
    public class JwtAuthResult
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public System.Collections.Generic.List<string> Permissions { get; set; } = new();
    }
}
