using System;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.Medications.Commands;
using HealthMonitoring.Patient.Features.Medications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Patient.Features.Medications
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? patientId,
            [FromQuery] bool? activeOnly)
        {
            var query = new GetAllMedicationsQuery 
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
            var query = new GetMedicationByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> Create([FromBody] CreateMedicationCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationCommand command)
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

        [HttpPost("{id:guid}/discontinue")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> Discontinue(Guid id, [FromBody] DiscontinueMedicationCommand command)
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
            var command = new ActivateMedicationCommand { Id = id };
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
            var command = new DeactivateMedicationCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }
    }
}