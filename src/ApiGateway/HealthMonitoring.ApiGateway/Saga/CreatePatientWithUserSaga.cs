using HealthMonitoring.ApiGateway.Models;
using HealthMonitoring.ApiGateway.Saga.Steps;

namespace HealthMonitoring.ApiGateway.Saga
{
    public class CreatePatientWithUserSaga : SagaBase<CreatePatientUserRequest, Guid>
    {
        public CreatePatientWithUserSaga(
            ILogger<CreatePatientWithUserSaga> logger,
            CreatePatientUserStep createPatientUserStep,
            CreatePatientStep createPatientStep) 
            : base(logger)
        {
            // Saga adımlarını ekle
            AddStep(createPatientUserStep);
            AddStep(createPatientStep);
        }
    }
}