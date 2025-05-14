using System.Reflection;
using FluentValidation;
using HealthMonitoring.Identity.Application.Handlers;
using HealthMonitoring.Identity.Application.Handlers.Authentication;
using HealthMonitoring.Identity.Application.Handlers.Commands;
using HealthMonitoring.Identity.Application.Handlers.Queries;
using HealthMonitoring.Identity.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace HealthMonitoring.Identity.Application
{
    /// <summary>
    /// Extension methods for registering application services
    /// </summary>
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register command handlers
            services.AddScoped<ICommandHandler<Authentication.AuthenticateUserCommand, HealthMonitoring.SharedKernel.Results.Result<Authentication.AuthenticationResult>>, AuthenticateUserCommandHandler>();
            services.AddScoped<ICommandHandler<Commands.CreateUserCommand, HealthMonitoring.SharedKernel.Results.Result<System.Guid>>, CreateUserCommandHandler>();
            services.AddScoped<ICommandHandler<Commands.UpdateUserCommand, HealthMonitoring.SharedKernel.Results.Result>, UpdateUserCommandHandler>();
            services.AddScoped<ICommandHandler<Commands.ChangePasswordCommand, HealthMonitoring.SharedKernel.Results.Result>, ChangePasswordCommandHandler>();
            services.AddScoped<ICommandHandler<Commands.AssignRoleCommand, HealthMonitoring.SharedKernel.Results.Result>, AssignRoleCommandHandler>();
            services.AddScoped<ICommandHandler<Commands.RemoveRoleCommand, HealthMonitoring.SharedKernel.Results.Result>, RemoveRoleCommandHandler>();
            services.AddScoped<ICommandHandler<Commands.DeleteUserCommand, HealthMonitoring.SharedKernel.Results.Result>, DeleteUserCommandHandler>();

            // Register query handlers
            services.AddScoped<IQueryHandler<Queries.GetUserByIdQuery, HealthMonitoring.SharedKernel.Results.Result<DTOs.UserDto>>, GetUserByIdQueryHandler>();
            services.AddScoped<IQueryHandler<Queries.GetUserByUsernameQuery, HealthMonitoring.SharedKernel.Results.Result<DTOs.UserDto>>, GetUserByUsernameQueryHandler>();
            services.AddScoped<IQueryHandler<Queries.GetAllUsersQuery, HealthMonitoring.SharedKernel.Results.Result<System.Collections.Generic.List<DTOs.UserDto>>>, GetAllUsersQueryHandler>();
            services.AddScoped<IQueryHandler<Queries.GetAllRolesQuery, HealthMonitoring.SharedKernel.Results.Result<System.Collections.Generic.List<DTOs.RoleDto>>>, GetAllRolesQueryHandler>();

            return services;
        }
    }
}