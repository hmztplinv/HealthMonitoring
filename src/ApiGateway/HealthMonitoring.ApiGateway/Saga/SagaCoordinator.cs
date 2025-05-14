using HealthMonitoring.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace HealthMonitoring.ApiGateway.Saga
{
    public class SagaCoordinator : ISagaCoordinator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SagaCoordinator> _logger;

        public SagaCoordinator(
            IServiceProvider serviceProvider,
            ILogger<SagaCoordinator> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<Result<TResult>> RunSagaAsync<TSaga, TData, TResult>(TData data)
            where TSaga : ISaga<TData, TResult>
            where TData : class
        {
            try
            {
                _logger.LogInformation($"Starting saga {typeof(TSaga).Name}");
                
                // DI container'dan saga örneğini al
                var saga = _serviceProvider.GetRequiredService<TSaga>();
                
                // Saga'yı çalıştır
                var result = await saga.ExecuteAsync(data);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Saga {typeof(TSaga).Name} completed successfully");
                }
                else
                {
                    _logger.LogWarning($"Saga {typeof(TSaga).Name} failed: {string.Join(", ", result.Errors)}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error running saga {typeof(TSaga).Name}");
                return Result<TResult>.Failure($"An error occurred during the saga: {ex.Message}");
            }
        }
    }
}