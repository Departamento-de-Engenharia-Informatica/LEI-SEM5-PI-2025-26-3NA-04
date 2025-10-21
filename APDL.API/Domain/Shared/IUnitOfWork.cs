using System.Threading.Tasks;

namespace APDL.API.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}