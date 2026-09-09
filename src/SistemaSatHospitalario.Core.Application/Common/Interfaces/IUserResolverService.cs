namespace SistemaSatHospitalario.Core.Application.Common.Interfaces;

/// <summary>
/// Service for resolving the current user context from HTTP context or claims principal.
/// </summary>
public interface IUserResolverService
{
    /// <summary>
    /// Gets the current user's identifier.
    /// </summary>
    Guid GetCurrentUserId();

    /// <summary>
    /// Gets the current user's name/identifier string.
    /// </summary>
    string GetCurrentUserName();

    /// <summary>
    /// Checks if the current user has administrative privileges.
    /// </summary>
    bool IsAdmin { get; }
    /// <summary>
    /// Gets a mapping of user IDs to their display names.
    /// </summary>
    /// <param name="userIds">The list of user IDs to resolve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A dictionary mapping user IDs to display names.</returns>
    Task<Dictionary<Guid, string>> GetDisplayNameMapAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken);

    Task<Guid?> ResolveUserIdByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<string?> ResolveUserIdAsync(Guid? targetUserId, CancellationToken cancellationToken);

}