using APDL.API.Domain.Shared;
using System.Threading.Tasks;

namespace APDL.API.Domain.ShippingAgentAggregate
{
    public interface IShippingAgentRepresentativeRepository
        : IRepository<ShippingAgentRepresentative, ShippingAgentRepresentativeId>
    {
        Task<ShippingAgentRepresentative?> GetByCitizenIdAsync(string citizenId);
        Task<ShippingAgentRepresentative?> GetByEmailAsync(string email);
    }
}
