using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.ApiGateway.Saga
{
    public interface ISaga<TData, TResult>
    {
        Task<Result<TResult>> ExecuteAsync(TData data);
    }
}