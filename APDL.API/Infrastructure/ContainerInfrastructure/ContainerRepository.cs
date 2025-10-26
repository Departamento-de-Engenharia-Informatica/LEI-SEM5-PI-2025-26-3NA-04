using System.Threading.Tasks;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.Repos;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.ContainerInfrastructure
{
    public class ContainerRepository : BaseRepository<Container, ContainerId>, IContainerRepository
    {
        private readonly DDDSample1DbContext _context;

        public ContainerRepository(DDDSample1DbContext context) : base(context.Containers)
        {
            _context = context;
        }

        public async Task<Container> GetByContainerNumberAsync(string containerNumber)
        {
            return await _context.Containers
                .FirstOrDefaultAsync(c => c.ContainerNumber.Value == containerNumber.ToUpper().Trim());
        }

        public async Task<bool> ContainerNumberExistsAsync(string containerNumber)
        {
            return await _context.Containers
                .AnyAsync(c => c.ContainerNumber.Value == containerNumber.ToUpper().Trim());
        }
    }
}