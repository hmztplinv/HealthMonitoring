using System;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Devices.Commands;
using HealthMonitoring.Patient.Features.Devices.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Patient.Features.Devices
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientDevicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientDevicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? patientId,
            [FromQuery] bool? activeOnly)
        {
            var query = new GetAllPatientDevicesQuery 
            { 
                PatientId = patientId,
                ActiveOnly = activeOnly
            };
            
            var result = await _mediator.Send(query);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetPatientDeviceByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> AssignDevice([FromBody] AssignDeviceCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        [HttpPost("{id:guid}/unassign")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> Unassign(Guid id, [FromBody] UnassignDeviceCommand command)
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

        [HttpPut("{id:guid}/notes")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdateNotes(Guid id, [FromBody] UpdateDeviceNotesCommand command)
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
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var command = new ActivateDeviceCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }

        [HttpPost("{id:guid}/deactivate")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var command = new DeactivateDeviceCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }
    }
}