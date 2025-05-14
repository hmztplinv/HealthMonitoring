using HealthMonitoring.SharedKernel.Results;

namespace HealthMonitoring.ApiGateway.Saga
{
    public abstract class SagaBase<TData, TResult> : ISaga<TData, TResult>
    {
        private readonly List<ISagaStep<TData, TResult>> _steps = new();
        private readonly ILogger<SagaBase<TData, TResult>> _logger;

        protected SagaBase(ILogger<SagaBase<TData, TResult>> logger)
        {
            _logger = logger;
        }

        protected void AddStep(ISagaStep<TData, TResult> step)
        {
            _steps.Add(step);
        }

        public async Task<Result<TResult>> ExecuteAsync(TData data)
        {
            var executedSteps = new List<ISagaStep<TData, TResult>>();
            Result<TResult>? result = null;

            foreach (var step in _steps)
            {
                try
                {
                    _logger.LogInformation($"Executing step {step.GetType().Name}");
                    result = await step.ExecuteAsync(data);
                    
                    if (result.IsFailure)
                    {
                        _logger.LogWarning($"Step {step.GetType().Name} failed: {string.Join(", ", result.Errors)}");
                        await RollbackAsync(executedSteps, data);
                        return result;
                    }
                    
                    executedSteps.Add(step);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception in step {step.GetType().Name}");
                    await RollbackAsync(executedSteps, data);
                    return Result<TResult>.Failure($"An error occurred in step {step.GetType().Name}: {ex.Message}");
                }
            }

            return result ?? Result<TResult>.Failure("No steps were executed");
        }

        private async Task RollbackAsync(List<ISagaStep<TData, TResult>> executedSteps, TData data)
        {
            _logger.LogWarning("Starting saga rollback");
            
            foreach (var step in executedSteps.AsEnumerable().Reverse())
            {
                try
                {
                    _logger.LogInformation($"Rolling back step {step.GetType().Name}");
                    await step.CompensateAsync(data);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during rollback of step {step.GetType().Name}");
                }
            }
        }
    }
}

