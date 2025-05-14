using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HealthMonitoring.ApiGateway.Models;
using HealthMonitoring.SharedKernel.Authentication;
using HealthMonitoring.SharedKernel.Results;
using Microsoft.Extensions.Configuration;

namespace HealthMonitoring.ApiGateway.Saga.Steps
{
    public class CreateUserStep : ISagaStep<CreateStaffUserRequest, Guid>
    {
        private readonly HttpClient _identityHttpClient;
        private readonly ILogger<CreateUserStep> _logger;
        private readonly IConfiguration _configuration;
        private Guid _createdUserId;

        public CreateUserStep(
            IHttpClientFactory httpClientFactory,
            ILogger<CreateUserStep> logger,
            IConfiguration configuration)
        {
            _identityHttpClient = httpClientFactory.CreateClient("IdentityService");
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Result<Guid>> ExecuteAsync(CreateStaffUserRequest data)
        {
            try
            {
                _logger.LogInformation("Creating user in Identity service");
                
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
                _identityHttpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                // Identity servisine gönderilecek komut
                var createUserCommand = new
                {
                    data.UserName,
                    data.Email,
                    data.Password,
                    data.FirstName,
                    data.LastName,
                    data.RoleIds
                };
                
                // Identity servisine HTTP isteği gönder
                var content = new StringContent(
                    JsonSerializer.Serialize(createUserCommand),
                    Encoding.UTF8,
                    "application/json");
                
                var response = await _identityHttpClient.PostAsync("api/users", content);
                
                if (response.IsSuccessStatusCode)
                {
                    // Başarılı yanıtı oku
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _createdUserId = JsonSerializer.Deserialize<Guid>(responseContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    // Oluşturulan kullanıcı ID'sini data nesnesine ata
                    data.UserId = _createdUserId;
                    
                    _logger.LogInformation($"User created successfully with ID: {_createdUserId}");
                    return Result<Guid>.Success(_createdUserId);
                }
                else
                {
                    // Hata mesajını oku
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Failed to create user. Status: {response.StatusCode}, Error: {errorContent}");
                    return Result<Guid>.Failure($"Failed to create user: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return Result<Guid>.Failure($"Error creating user: {ex.Message}");
            }
        }

        public async Task<Result> CompensateAsync(CreateStaffUserRequest data)
        {
            try
            {
                // Eğer kullanıcı oluşturulmuşsa, silme işlemi yap
                if (_createdUserId != Guid.Empty)
                {
                    _logger.LogInformation($"Compensating by deleting user with ID: {_createdUserId}");
                    
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
                    _identityHttpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    
                    // Identity servisine silme isteği gönder
                    var response = await _identityHttpClient.DeleteAsync($"api/users/{_createdUserId}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"User deleted successfully during compensation");
                        return Result.Success();
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning($"Failed to delete user during compensation. Status: {response.StatusCode}, Error: {errorContent}");
                        return Result.Failure($"Failed to delete user during compensation: {errorContent}");
                    }
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during compensation for user creation");
                return Result.Failure($"Error during compensation: {ex.Message}");
            }
        }
    }
}