using System.Threading.Tasks;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ContainerAggregate.Repos
{
    public interface IContainerRepository : IRepository<Container, ContainerId>
    {
        Task<Container> GetByContainerNumberAsync(string containerNumber);
        Task<bool> ContainerNumberExistsAsync(string containerNumber);
    }
}