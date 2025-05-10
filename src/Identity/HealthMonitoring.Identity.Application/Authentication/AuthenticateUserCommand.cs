using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.Identity.Application.Authentication
{
    /// <summary>
    /// Command for authenticating a user
    /// </summary>
    public class AuthenticateUserCommand : Commands.ICommand<Result<AuthenticationResult>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}