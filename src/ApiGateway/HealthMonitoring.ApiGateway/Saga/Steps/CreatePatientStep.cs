using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HealthMonitoring.ApiGateway.Models;
using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.ApiGateway.Saga.Steps
{
    public class CreatePatientStep : ISagaStep<CreatePatientUserRequest, Guid>
    {
        private readonly HttpClient _patientHttpClient;
        private readonly ILogger<CreatePatientStep> _logger;
        private Guid _createdPatientId;

        public CreatePatientStep(
            IHttpClientFactory httpClientFactory,
            ILogger<CreatePatientStep> logger)
        {
            _patientHttpClient = httpClientFactory.CreateClient("PatientService");
            _logger = logger;
        }

        public async Task<Result<Guid>> ExecuteAsync(CreatePatientUserRequest data)
        {
            try
            {
                _logger.LogInformation("Creating patient in Patient service");
                
                // Patient servisine gönderilecek komut
                var createPatientCommand = new
                {
                    UserId = data.UserId, // Önceki adımdan gelen kullanıcı ID'si
                    data.FirstName,
                    data.LastName,
                    data.IdentificationNumber,
                    data.DateOfBirth,
                    data.Gender,
                    data.BloodType,
                    data.Address,
                    data.PhoneNumber,
                    data.Email,
                    data.EmergencyContactName,
                    data.EmergencyContactPhone
                };
                
                // Patient servisine HTTP isteği gönder
                var content = new StringContent(
                    JsonSerializer.Serialize(createPatientCommand),
                    Encoding.UTF8,
                    "application/json");
                
                var response = await _patientHttpClient.PostAsync("api/patients", content);
                
                if (response.IsSuccessStatusCode)
                {
                    // Başarılı yanıtı oku
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _createdPatientId = JsonSerializer.Deserialize<Guid>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    _logger.LogInformation($"Patient created successfully with ID: {_createdPatientId}");
                    return Result<Guid>.Success(data.UserId); // User ID'yi döndür
                }
                else
                {
                    // Hata mesajını oku
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Failed to create patient. Status: {response.StatusCode}, Error: {errorContent}");
                    return Result<Guid>.Failure($"Failed to create patient: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return Result<Guid>.Failure($"Error creating patient: {ex.Message}");
            }
        }

        public async Task<Result> CompensateAsync(CreatePatientUserRequest data)
        {
            try
            {
                // Eğer hasta oluşturulmuşsa, silme işlemi yap
                if (_createdPatientId != Guid.Empty)
                {
                    _logger.LogInformation($"Compensating by deleting patient with ID: {_createdPatientId}");
                    
                    // Patient servisine silme isteği gönder
                    var response = await _patientHttpClient.DeleteAsync($"api/patients/{_createdPatientId}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Patient deleted successfully during compensation");
                        return Result.Success();
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning($"Failed to delete patient during compensation. Status: {response.StatusCode}, Error: {errorContent}");
                        return Result.Failure($"Failed to delete patient during compensation: {errorContent}");
                    }
                }
                
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during compensation for patient creation");
                return Result.Failure($"Error during compensation: {ex.Message}");
            }
        }
    }
}