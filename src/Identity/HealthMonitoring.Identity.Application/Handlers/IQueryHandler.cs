using System.Threading;
using System.Threading.Tasks;
using HealthMonitoring.Identity.Application.Queries;

namespace HealthMonitoring.Identity.Application.Handlers
{
    /// <summary>
    /// Interface for query handlers
    /// </summary>
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
    }
}