using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Authentication;
using HealthMonitoring.Identity.Application.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICommandHandler<AuthenticateUserCommand, HealthMonitoring.SharedKernel.Results.Result<AuthenticationResult>> _authenticateUserHandler;

        public AuthController(
            ICommandHandler<AuthenticateUserCommand, HealthMonitoring.SharedKernel.Results.Result<AuthenticationResult>> authenticateUserHandler)
        {
            _authenticateUserHandler = authenticateUserHandler;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserCommand command)
        {
            var result = await _authenticateUserHandler.Handle(command, default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }
    }
}