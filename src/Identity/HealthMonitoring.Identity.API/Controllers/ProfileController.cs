using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;
using HealthMonitoring.Identity.Application.Handlers;
using HealthMonitoring.Identity.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ICommandHandler<UpdateUserCommand, HealthMonitoring.SharedKernel.Results.Result> _updateUserHandler;
        private readonly ICommandHandler<ChangePasswordCommand, HealthMonitoring.SharedKernel.Results.Result> _changePasswordHandler;
        private readonly IQueryHandler<GetUserByIdQuery, HealthMonitoring.SharedKernel.Results.Result<Application.DTOs.UserDto>> _getUserByIdHandler;

        public ProfileController(
            ICommandHandler<UpdateUserCommand, HealthMonitoring.SharedKernel.Results.Result> updateUserHandler,
            ICommandHandler<ChangePasswordCommand, HealthMonitoring.SharedKernel.Results.Result> changePasswordHandler,
            IQueryHandler<GetUserByIdQuery, HealthMonitoring.SharedKernel.Results.Result<Application.DTOs.UserDto>> getUserByIdHandler)
        {
            _updateUserHandler = updateUserHandler;
            _changePasswordHandler = changePasswordHandler;
            _getUserByIdHandler = getUserByIdHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            {
                return Unauthorized();
            }

            var result = await _getUserByIdHandler.Handle(new GetUserByIdQuery(id), default);
            
            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            {
                return Unauthorized();
            }

            if (id != command.Id)
            {
                return Forbid();
            }

            var result = await _updateUserHandler.Handle(command, default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            {
                return Unauthorized();
            }

            if (id != command.UserId)
            {
                return Forbid();
            }

            var result = await _changePasswordHandler.Handle(command, default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }
    }
}