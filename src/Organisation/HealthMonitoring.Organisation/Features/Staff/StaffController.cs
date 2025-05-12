// StaffController.cs
using System;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Staff.Commands;
using HealthMonitoring.Organisation.Features.Staff.Queries;
using HealthMonitoring.SharedKernel.DomainModels.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Organisation.Features.Staff
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StaffController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool? activeOnly,
            [FromQuery] Guid? departmentId,
            [FromQuery] Guid? hospitalId,
            [FromQuery] UserRole? staffRole)
        {
            var query = new GetAllStaffMembersQuery
            {
                ActiveOnly = activeOnly,
                DepartmentId = departmentId,
                HospitalId = hospitalId,
                StaffRole = staffRole
            };
            
            var result = await _mediator.Send(query);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetStaffMemberByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateStaffMemberCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffMemberCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new { error = "ID in URL does not match ID in request body" });
            }

            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPost("{id:guid}/transfer")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Transfer(Guid id, [FromBody] TransferStaffMemberCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new { error = "ID in URL does not match ID in request body" });
            }

            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPost("{id:guid}/activate")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var command = new ActivateStaffMemberCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPost("{id:guid}/deactivate")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var command = new DeactivateStaffMemberCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }
    }
}