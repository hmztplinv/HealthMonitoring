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