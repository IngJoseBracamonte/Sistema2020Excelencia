namespace SistemaSatHospitalario.Infrastructure.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Infrastructure.Identity; // <-- Asegura la referencia a tu ApplicationUser

/// <summary>
/// Resolves information about the currently authenticated user from the HTTP context.
/// </summary>
public sealed class UserResolverService : IUserResolverService
{  private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<IdentityUser> _userManager; // <-- Cambiado a IdentityUser

    public UserResolverService(
        IHttpContextAccessor httpContextAccessor,
        UserManager<IdentityUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _userManager = userManager
            ?? throw new ArgumentNullException(nameof(userManager));
    }

    /// <inheritdoc />
    public Guid GetCurrentUserId()
    {
        var user = GetAuthenticatedUser();

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || string.IsNullOrWhiteSpace(userIdClaim.Value))
        {
            throw new UnauthorizedAccessException(
                "The authenticated user does not have a user ID claim.");
        }

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "The authenticated user has an invalid user ID.");
        }

        return userId;
    }

    /// <inheritdoc />
    public string GetCurrentUserName()
    {
        var user = GetAuthenticatedUser();

        var userName = user.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userName))
        {
            var nameClaim = user.FindFirst(ClaimTypes.Name);

            if (nameClaim is not null && !string.IsNullOrWhiteSpace(nameClaim.Value))
            {
                return nameClaim.Value;
            }
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new UnauthorizedAccessException(
                "The authenticated user does not have a valid user name.");
        }

        return userName;
    }

    /// <inheritdoc />
    public bool IsAdmin
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.Identity?.IsAuthenticated == true
                   && user.IsInRole(AuthorizationConstants.Admin);
        }
    }

    /// <inheritdoc />
    public Task<Dictionary<Guid, string>> GetDisplayNameMapAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userIds);

        var result = userIds
            .Distinct()
            .ToDictionary(
                id => id,
                id => $"User_{id}");

        return Task.FromResult(result);
    }

    private ClaimsPrincipal GetAuthenticatedUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException(
                "No authenticated user was found.");
        }

        return user;
    }

    /// <inheritdoc />
    public Task<Guid?> ResolveUserIdByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Task.FromResult<Guid?>(GetCurrentUserId());
        }

        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserName = currentUser?.Identity?.Name;

        if (string.Equals(currentUserName, username, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult<Guid?>(GetCurrentUserId());
        }

        throw new InvalidOperationException("No se puede resolver un usuario diferente al autenticado sin consultar la base de datos.");
    }

    /// <inheritdoc />
    public async Task<string> ResolveUserIdAsync(Guid? targetUserId, CancellationToken cancellationToken)
    {
        // Si no se envía un ID específico, retorna el ID del usuario actualmente autenticado
        if (!targetUserId.HasValue || targetUserId.Value == Guid.Empty)
        {
            return GetCurrentUserId().ToString();
        }

        var user = await _userManager.FindByIdAsync(targetUserId.Value.ToString());

        if (user == null)
        {
            throw new KeyNotFoundException($"El usuario con ID '{targetUserId}' no existe en el sistema.");
        }

        return user.UserName ?? user.Id;
    }
}