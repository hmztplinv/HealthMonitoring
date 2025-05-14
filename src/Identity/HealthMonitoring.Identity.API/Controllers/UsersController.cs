using System;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;
using HealthMonitoring.Identity.Application.Handlers;
using HealthMonitoring.Identity.Application.Queries;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandHandler<CreateUserCommand, HealthMonitoring.SharedKernel.Results.Result<Guid>> _createUserHandler;
        private readonly ICommandHandler<UpdateUserCommand, HealthMonitoring.SharedKernel.Results.Result> _updateUserHandler;
        private readonly ICommandHandler<ChangePasswordCommand, HealthMonitoring.SharedKernel.Results.Result> _changePasswordHandler;
        private readonly ICommandHandler<AssignRoleCommand, HealthMonitoring.SharedKernel.Results.Result> _assignRoleHandler;
        private readonly ICommandHandler<RemoveRoleCommand, HealthMonitoring.SharedKernel.Results.Result> _removeRoleHandler;
        private readonly IQueryHandler<GetUserByIdQuery, HealthMonitoring.SharedKernel.Results.Result<Application.DTOs.UserDto>> _getUserByIdHandler;
        private readonly IQueryHandler<GetUserByUsernameQuery, HealthMonitoring.SharedKernel.Results.Result<Application.DTOs.UserDto>> _getUserByUsernameHandler;
        private readonly IQueryHandler<GetAllUsersQuery, HealthMonitoring.SharedKernel.Results.Result<System.Collections.Generic.List<Application.DTOs.UserDto>>> _getAllUsersHandler;
        private readonly ICommandHandler<DeleteUserCommand, HealthMonitoring.SharedKernel.Results.Result> _deleteUserHandler;


        public UsersController(
            ICommandHandler<CreateUserCommand, HealthMonitoring.SharedKernel.Results.Result<Guid>> createUserHandler,
            ICommandHandler<UpdateUserCommand, HealthMonitoring.SharedKernel.Results.Result> updateUserHandler,
            ICommandHandler<ChangePasswordCommand, HealthMonitoring.SharedKernel.Results.Result> changePasswordHandler,
            ICommandHandler<AssignRoleCommand, HealthMonitoring.SharedKernel.Results.Result> assignRoleHandler,
            ICommandHandler<RemoveRoleCommand, HealthMonitoring.SharedKernel.Results.Result> removeRoleHandler,
            IQueryHandler<GetUserByIdQuery, HealthMonitoring.SharedKernel.Results.Result<Application.DTOs.UserDto>> getUserByIdHandler,
            IQueryHandler<GetUserByUsernameQuery, HealthMonitoring.SharedKernel.Results.Result<Application.DTOs.UserDto>> getUserByUsernameHandler,
            IQueryHandler<GetAllUsersQuery, HealthMonitoring.SharedKernel.Results.Result<System.Collections.Generic.List<Application.DTOs.UserDto>>> getAllUsersHandler,
            ICommandHandler<DeleteUserCommand, HealthMonitoring.SharedKernel.Results.Result> deleteUserHandler)
        {
            _createUserHandler = createUserHandler;
            _updateUserHandler = updateUserHandler;
            _changePasswordHandler = changePasswordHandler;
            _assignRoleHandler = assignRoleHandler;
            _removeRoleHandler = removeRoleHandler;
            _getUserByIdHandler = getUserByIdHandler;
            _getUserByUsernameHandler = getUserByUsernameHandler;
            _getAllUsersHandler = getAllUsersHandler;
            _deleteUserHandler = deleteUserHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getAllUsersHandler.Handle(new GetAllUsersQuery(), default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _getUserByIdHandler.Handle(new GetUserByIdQuery(id), default);
            
            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var result = await _getUserByUsernameHandler.Handle(new GetUserByUsernameQuery(username), default);
            
            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var result = await _createUserHandler.Handle(command, default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new { error = "ID in URL does not match ID in request body" });
            }

            var result = await _updateUserHandler.Handle(command, default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPost("{id:guid}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand command)
        {
            if (id != command.UserId)
            {
                return BadRequest(new { error = "ID in URL does not match ID in request body" });
            }

            var result = await _changePasswordHandler.Handle(command, default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPost("{userId:guid}/roles/{roleId:guid}")]
        public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
        {
            var command = new AssignRoleCommand
            {
                UserId = userId,
                RoleId = roleId
            };

            var result = await _assignRoleHandler.Handle(command, default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
        public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
        {
            var command = new RemoveRoleCommand
            {
                UserId = userId,
                RoleId = roleId
            };

            var result = await _removeRoleHandler.Handle(command, default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _deleteUserHandler.Handle(command, default);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }
    }
}