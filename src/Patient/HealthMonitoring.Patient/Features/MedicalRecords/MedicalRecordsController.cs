using System;
using System.Threading.Tasks;
using HealthMonitoring.Patient.Features.MedicalRecords.Commands;
using HealthMonitoring.Patient.Features.MedicalRecords.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Patient.Features.MedicalRecords
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MedicalRecordsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> GetAll([FromQuery] Guid? patientId)
        {
            var query = new GetAllMedicalRecordsQuery { PatientId = patientId };
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
            var query = new GetMedicalRecordByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> Create([FromBody] CreateMedicalRecordCommand command)
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicalRecordCommand command)
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

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteMedicalRecordCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }
    }
}