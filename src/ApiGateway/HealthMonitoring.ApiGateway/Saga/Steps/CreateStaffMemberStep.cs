using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HealthMonitoring.ApiGateway.Models;
using HealthMonitoring.SharedKernel.Authentication;
using HealthMonitoring.SharedKernel.Results;
using Microsoft.Extensions.Configuration;

namespace HealthMonitoring.ApiGateway.Saga.Steps
{
    public class CreateStaffMemberStep : ISagaStep<CreateStaffUserRequest, Guid>
    {
        private readonly HttpClient _organisationHttpClient;
        private readonly ILogger<CreateStaffMemberStep> _logger;
        private readonly IConfiguration _configuration;
        private Guid _createdStaffId;

        public CreateStaffMemberStep(
            IHttpClientFactory httpClientFactory,
            ILogger<CreateStaffMemberStep> logger,
            IConfiguration configuration)
        {
            _organisationHttpClient = httpClientFactory.CreateClient("OrganisationService");
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Result<Guid>> ExecuteAsync(CreateStaffUserRequest data)
        {
            try
            {
                _logger.LogInformation("Creating staff member in Organisation service");
                
                // Generate an admin token for service-to-service communication
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = jwtSettings["Key"];
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];
                
                // Create a token with Administrator role
                var adminClaims = new Dictionary<string, object>
                {
                    { "role", "Administrator" }
                };
                
                var token = JwtTokenHelper.GenerateJwtToken(
                    Guid.NewGuid().ToString(),
                    "Service Account",
                    "ApiGateway",
                    adminClaims,
                    key,
                    issuer,
                    audience,
                    TimeSpan.FromMinutes(5)
                );
                
                // Set the authorization header
                _organisationHttpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                // Organisation servisine gönderilecek komut
                var createStaffCommand = new
                {
                    UserId = data.UserId, // Önceki adımdan gelen kullanıcı ID'si
                    data.FirstName,
                    data.LastName,
                    data.Title,
                    data.DepartmentId,
                    StaffRole = data.StaffRole,
                    data.LicenseNumber
                };
                
                // Organisation servisine HTTP isteği gönder
                var content = new StringContent(
                    JsonSerializer.Serialize(createStaffCommand),
                    Encoding.UTF8,
                    "application/json");
                
                var response = await _organisationHttpClient.PostAsync("api/staff", content);
                
                if (response.IsSuccessStatusCode)
                {
                    // Başarılı yanıtı oku
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _createdStaffId = JsonSerializer.Deserialize<Guid>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    _logger.LogInformation($"Staff member created successfully with ID: {_createdStaffId}");
                    return Result<Guid>.Success(data.UserId); // User ID'yi döndür
                }
                else
                {
                    // Hata mesajını oku
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Failed to create staff member. Status: {response.StatusCode}, Error: {errorContent}");
                    return Result<Guid>.Failure($"Failed to create staff member: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member");
                return Result<Guid>.Failure($"Error creating staff member: {ex.Message}");
            }
        }

        public async Task<Result> CompensateAsync(CreateStaffUserRequest data)
        {
            try
            {
                // Eğer personel oluşturulmuşsa, silme işlemi yap
                if (_createdStaffId != Guid.Empty)
                {
                    _logger.LogInformation($"Compensating by deleting staff member with ID: {_createdStaffId}");
                    
                    // Generate a new admin token for compensation
                    var jwtSettings = _configuration.GetSection("JwtSettings");
                    var key = jwtSettings["Key"];
                    var issuer = jwtSettings["Issuer"];
                    var audience = jwtSettings["Audience"];
                    
                    // Create a token with Administrator role
                    var adminClaims = new Dictionary<string, object>
                    {
                        { "role", "Administrator" }
                    };
                    
                    var token = JwtTokenHelper.GenerateJwtToken(
                        Guid.NewGuid().ToString(),
                        "Service Account",
                        "ApiGateway",
                        adminClaims,
                        key,
                        issuer,
                        audience,
                        TimeSpan.FromMinutes(5)
                    );
                    
                    // Set the authorization header
                    _organisationHttpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    
                    // Organisation servisine silme isteği gönder
                    var response = await _organisationHttpClient.DeleteAsync($"api/staff/{_createdStaffId}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Staff member deleted successfully during compensation");
                        return Result.Success();
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning($"Failed to delete staff member during compensation. Status: {response.StatusCode}, Error: {errorContent}");
                        return Result.Failure($"Failed to delete staff member during compensation: {errorContent}");
                    }
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during compensation for staff member creation");
                return Result.Failure($"Error during compensation: {ex.Message}");
            }
        }
    }
}