namespace SistemaSatHospitalario.Infrastructure.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

/// <summary>
/// Resolves information about the currently authenticated user from the HTTP context.
/// </summary>
public sealed class UserResolverService : IUserResolverService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserResolverService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">
    /// Accessor for the current HTTP context.
    /// </param>
    public UserResolverService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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

        // TODO:
        // Replace this implementation with a query against
        // the application's user repository/table.

        var result = userIds
            .Distinct()
            .ToDictionary(
                id => id,
                id => $"User_{id}");

        return Task.FromResult(result);
    }

    /// <summary>
    /// Gets the current authenticated user.
    /// </summary>
    /// <returns>The authenticated claims principal.</returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when there is no authenticated user.
    /// </exception>
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
}
