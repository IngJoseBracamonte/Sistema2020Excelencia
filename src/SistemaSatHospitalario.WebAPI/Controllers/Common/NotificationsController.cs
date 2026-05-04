using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.WebAPI.Controllers.Common
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public NotificationsController(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var userId = _currentUser.UserId?.ToString();
            var role = _currentUser.Role;

            var notifications = await _context.Notifications
                .Where(n => (n.TargetUserId == null && n.TargetRole == null) || 
                            n.TargetUserId == userId || 
                            n.TargetRole == role)
                .OrderByDescending(n => n.Timestamp)
                .Take(20)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.MarkAsRead();
            await _context.SaveChangesAsync(default);

            return NoContent();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = _currentUser.UserId?.ToString();
            var role = _currentUser.Role;

            var unread = await _context.Notifications
                .Where(n => !n.IsRead && 
                            ((n.TargetUserId == null && n.TargetRole == null) || 
                             n.TargetUserId == userId || 
                             n.TargetRole == role))
                .ToListAsync();

            foreach (var n in unread)
            {
                n.MarkAsRead();
            }

            await _context.SaveChangesAsync(default);

            return NoContent();
        }
    }
}
