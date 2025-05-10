using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Authentication;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.SharedKernel.Authentication;
using HealthMonitoring.SharedKernel.Helpers;
using HealthMonitoring.SharedKernel.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HealthMonitoring.Identity.Application.Handlers.Authentication
{
    /// <summary>
    /// Handler for AuthenticateUserCommand
    /// </summary>
    public class AuthenticateUserCommandHandler : ICommandHandler<AuthenticateUserCommand, Result<AuthenticationResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthenticateUserCommandHandler(
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<Result<AuthenticationResult>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            // Get user by username
            var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username, cancellationToken);
            if (user == null)
            {
                return Result<AuthenticationResult>.Failure("Invalid username or password");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Result<AuthenticationResult>.Failure("User account is deactivated");
            }

            // Verify password
            if (!CryptographyHelper.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Result<AuthenticationResult>.Failure("Invalid username or password");
            }

            // Update last login date
            user.SetLastLoginDate(DateTime.UtcNow);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get JWT settings from configuration
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiresMinutes = int.Parse(jwtSettings["ExpiresMinutes"]);

            // Create claims for JWT token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add role claims
            foreach (var userRole in user.UserRoleMappings)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            // Generate JWT token
            var token = JwtTokenHelper.GenerateJwtToken(claims, key, issuer, audience, expiresMinutes);
            var expiration = DateTime.UtcNow.AddMinutes(expiresMinutes);

            // Create authentication result
            var authResult = new AuthenticationResult
            {
                Token = token,
                Expiration = expiration,
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Roles = user.UserRoleMappings.Select(ur => ur.Role.Name).ToList()
            };

            return Result<AuthenticationResult>.Success(authResult);
        }
    }
}
