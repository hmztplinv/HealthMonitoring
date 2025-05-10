using System;
using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;
using HealthMonitoring.Identity.Domain.Models;
using HealthMonitoring.Identity.Domain.Services;
using HealthMonitoring.SharedKernel.Helpers;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Handlers.Commands
{
    /// <summary>
    /// Handler for CreateUserCommand
    /// </summary>
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if username already exists
            if (await _unitOfWork.Users.ExistsByUsernameAsync(request.UserName, cancellationToken))
            {
                return Result<Guid>.Failure($"Username '{request.UserName}' is already taken");
            }

            // Check if email already exists
            if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                return Result<Guid>.Failure($"Email '{request.Email}' is already registered");
            }

            // Create new user
            var user = new User(request.UserName, request.Email, request.FirstName, request.LastName);

            // Set password
            var (passwordHash, passwordSalt) = CryptographyHelper.HashPassword(request.Password);
            user.SetPassword(passwordHash, passwordSalt);

            // Add roles if specified
            if (request.RoleIds != null && request.RoleIds.Length > 0)
            {
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(roleId, cancellationToken);
                    if (role != null)
                    {
                        user.AddRole(role);
                    }
                }
            }

            // Add user to database
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}