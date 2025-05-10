using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Handlers;
using HealthMonitoring.Identity.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class RolesController : ControllerBase
    {
        private readonly IQueryHandler<GetAllRolesQuery, HealthMonitoring.SharedKernel.Results.Result<System.Collections.Generic.List<Application.DTOs.RoleDto>>> _getAllRolesHandler;

        public RolesController(
            IQueryHandler<GetAllRolesQuery, HealthMonitoring.SharedKernel.Results.Result<System.Collections.Generic.List<Application.DTOs.RoleDto>>> getAllRolesHandler)
        {
            _getAllRolesHandler = getAllRolesHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getAllRolesHandler.Handle(new GetAllRolesQuery(), default);
            
            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }
    }
}