// src/ApiGateway/HealthMonitoring.ApiGateway/Controllers/UserManagementController.cs
using System.Net.Http.Json;
using HealthMonitoring.ApiGateway.Models;
using HealthMonitoring.ApiGateway.Saga;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthMonitoring.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/gateway/users")]
    public class UserManagementController : ControllerBase
    {
        private readonly ISagaCoordinator _sagaCoordinator;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            ISagaCoordinator sagaCoordinator,
            ILogger<UserManagementController> logger)
        {
            _sagaCoordinator = sagaCoordinator;
            _logger = logger;
        }

        [HttpPost("staff")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateStaffWithUser([FromBody] CreateStaffUserRequest request)
        {
            try
            {
                _logger.LogInformation("Initiating staff user creation process");
                
                // CreateUserWithRoleSaga örneğini kullanarak süreci başlat
                var result = await _sagaCoordinator.RunSagaAsync<CreateUserWithRoleSaga, CreateStaffUserRequest, Guid>(request);
                
                if (result.IsSuccess)
                {
                    return Ok(new { userId = result.Value, message = "User and staff creation completed successfully" });
                }
                else
                {
                    return BadRequest(new { errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in staff user creation process");
                return StatusCode(500, new { error = "An error occurred while processing the request" });
            }
        }

        [HttpPost("patient")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> CreatePatientWithUser([FromBody] CreatePatientUserRequest request)
        {
            try
            {
                _logger.LogInformation("Initiating patient user creation process");
                
                // CreatePatientWithUserSaga örneğini kullanarak süreci başlat
                var result = await _sagaCoordinator.RunSagaAsync<CreatePatientWithUserSaga, CreatePatientUserRequest, Guid>(request);
                
                if (result.IsSuccess)
                {
                    return Ok(new { userId = result.Value, message = "User and patient creation completed successfully" });
                }
                else
                {
                    return BadRequest(new { errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in patient user creation process");
                return StatusCode(500, new { error = "An error occurred while processing the request" });
            }
        }
    }
}