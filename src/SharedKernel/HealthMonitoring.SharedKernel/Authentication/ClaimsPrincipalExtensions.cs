using System;
using System.Security.Claims;

namespace HealthMonitoring.SharedKernel.Authentication
{
    /// <summary>
    /// Extensions for ClaimsPrincipal
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the user ID from the claims principal
        /// </summary>
        /// <param name="principal">The claims principal</param>
        /// <returns>The user ID as a GUID</returns>
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            
            if (claim == null)
                throw new InvalidOperationException("User ID claim not found");

            return Guid.Parse(claim.Value);
        }

        /// <summary>
        /// Gets the username from the claims principal
        /// </summary>
        /// <param name="principal">The claims principal</param>
        /// <returns>The username</returns>
        public static string GetUsername(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.Name);
            
            if (claim == null)
                throw new InvalidOperationException("Username claim not found");

            return claim.Value;
        }

        /// <summary>
        /// Gets the user's email from the claims principal
        /// </summary>
        /// <param name="principal">The claims principal</param>
        /// <returns>The user's email</returns>
        public static string GetEmail(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.Email);
            
            if (claim == null)
                throw new InvalidOperationException("Email claim not found");

            return claim.Value;
        }

        /// <summary>
        /// Checks if the user has the specified role
        /// </summary>
        /// <param name="principal">The claims principal</param>
        /// <param name="role">The role to check</param>
        /// <returns>True if the user has the role, false otherwise</returns>
        public static bool HasRole(this ClaimsPrincipal principal, string role)
        {
            return principal.HasClaim(ClaimTypes.Role, role);
        }
    }
}