using System.Security.Claims;
using Face_Recognition_Demo.Models;

namespace Face_Recognition_Demo.Services
{
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT token for the specified user with their roles
        /// </summary>
        /// <param name="user">The user to generate token for</param>
        /// <param name="roles">List of roles assigned to the user</param>
        /// <returns>JWT token as string</returns>
        string GenerateToken(User user, IList<string> roles);

        /// <summary>
        /// Generates a secure refresh token
        /// </summary>
        /// <returns>Base64 encoded refresh token</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Gets the ClaimsPrincipal from an expired token
        /// </summary>
        /// <param name="token">The expired JWT token</param>
        /// <returns>ClaimsPrincipal containing user claims</returns>
        /// <exception cref="SecurityTokenException">Thrown when token is invalid</exception>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        /// <summary>
        /// Validates a JWT token without checking expiration
        /// </summary>
        /// <param name="token">The JWT token to validate</param>
        /// <returns>True if token is valid, false otherwise</returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Gets the expiration time of a JWT token
        /// </summary>
        /// <param name="token">The JWT token</param>
        /// <returns>DateTime of token expiration</returns>
        DateTime? GetTokenExpiration(string token);

        /// <summary>
        /// Extracts user ID from a JWT token
        /// </summary>
        /// <param name="token">The JWT token</param>
        /// <returns>User ID if found, null otherwise</returns>
        string? GetUserIdFromToken(string token);
    }
}