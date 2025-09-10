using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces
{
    public interface IAppDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
