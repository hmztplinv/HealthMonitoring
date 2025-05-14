using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HealthMonitoring.SharedKernel.Authentication
{
    /// <summary>
    /// Helper for JWT token generation and validation
    /// </summary>
    public static class JwtTokenHelper
    {
        /// <summary>
        /// Generates a JWT token for the specified claims
        /// </summary>
        /// <param name="claims">The claims to include in the token</param>
        /// <param name="key">The secret key used to sign the token</param>
        /// <param name="issuer">The issuer of the token</param>
        /// <param name="audience">The audience of the token</param>
        /// <param name="expiresMinutes">The token expiration time in minutes</param>
        /// <returns>A JWT token string</returns>
        public static string GenerateJwtToken(
            IEnumerable<Claim> claims,
            string key,
            string issuer,
            string audience,
            int expiresMinutes = 60)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a JWT token with standard user claims and additional custom claims
        /// </summary>
        /// <param name="subjectId">The subject ID (user ID)</param>
        /// <param name="name">The user's name</param>
        /// <param name="preferredUsername">The user's preferred username</param>
        /// <param name="additionalClaims">Additional claims to include in the token</param>
        /// <param name="key">The secret key used to sign the token</param>
        /// <param name="issuer">The issuer of the token</param>
        /// <param name="audience">The audience of the token</param>
        /// <param name="expires">The token expiration timespan</param>
        /// <returns>A JWT token string</returns>
        public static string GenerateJwtToken(
            string subjectId,
            string name,
            string preferredUsername,
            Dictionary<string, object> additionalClaims,
            string key,
            string issuer,
            string audience,
            TimeSpan expires)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, subjectId),
                new Claim(JwtRegisteredClaimNames.Name, name),
                new Claim("preferred_username", preferredUsername),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Add additional claims
            if (additionalClaims != null)
            {
                foreach (var claim in additionalClaims)
                {
                    // Handle role claims specially
                    if (claim.Key.Equals("role", StringComparison.OrdinalIgnoreCase))
                    {
                        if (claim.Value is string roleValue)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, roleValue));
                        }
                        else if (claim.Value is string[] roleValues)
                        {
                            foreach (var role in roleValues)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role));
                            }
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(claim.Key, claim.Value.ToString()));
                    }
                }
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(expires),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Configures the token validation parameters
        /// </summary>
        /// <param name="key">The secret key used to validate the token signature</param>
        /// <param name="issuer">The valid issuer of the token</param>
        /// <param name="audience">The valid audience of the token</param>
        /// <returns>Token validation parameters</returns>
        public static TokenValidationParameters GetTokenValidationParameters(
            string key,
            string issuer,
            string audience)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
    }
}