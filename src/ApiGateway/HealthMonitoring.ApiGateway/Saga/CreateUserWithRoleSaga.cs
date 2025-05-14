using HealthMonitoring.ApiGateway.Models;
using HealthMonitoring.ApiGateway.Saga.Steps;

namespace HealthMonitoring.ApiGateway.Saga
{
    public class CreateUserWithRoleSaga : SagaBase<CreateStaffUserRequest, Guid>
    {
        public CreateUserWithRoleSaga(
            ILogger<CreateUserWithRoleSaga> logger,
            CreateUserStep createUserStep,
            CreateStaffMemberStep createStaffMemberStep) 
            : base(logger)
        {
            // Saga adımlarını ekle
            AddStep(createUserStep);
            AddStep(createStaffMemberStep);
        }
    }
}
