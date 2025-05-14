using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.ApiGateway.Saga
{
    public interface ISagaStep<TData, TResult>
    {
        Task<Result<TResult>> ExecuteAsync(TData data);
        Task<Result> CompensateAsync(TData data);
    }
}