using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Commands;

namespace HealthMonitoring.Identity.Application.Handlers
{
    /// <summary>
    /// Interface for command handlers
    /// </summary>
    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
    }
}