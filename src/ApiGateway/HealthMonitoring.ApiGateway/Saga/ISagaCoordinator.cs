using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.ApiGateway.Saga
{
    public interface ISagaCoordinator
    {
        Task<Result<TResult>> RunSagaAsync<TSaga, TData, TResult>(TData data)
            where TSaga : ISaga<TData, TResult>
            where TData : class;
    }
}