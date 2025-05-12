// DepartmentsController.cs
using System;
using System.Threading.Tasks;
using HealthMonitoring.Organisation.Features.Departments.Commands;
using HealthMonitoring.Organisation.Features.Departments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Organisation.Features.Departments
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? activeOnly, [FromQuery] Guid? hospitalId)
        {
            var query = new GetAllDepartmentsQuery { ActiveOnly = activeOnly, HospitalId = hospitalId };
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
            var query = new GetDepartmentByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentCommand command)
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
            var command = new ActivateDepartmentCommand { Id = id };
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
            var command = new DeactivateDepartmentCommand { Id = id };
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return NoContent();
        }
    }
}