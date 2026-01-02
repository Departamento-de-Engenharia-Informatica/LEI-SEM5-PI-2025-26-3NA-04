using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.PrivacyPolicyAggregate.Repos
{
    public interface IPrivacyPolicyRepository : IRepository<PrivacyPolicy, PrivacyPolicyId>
    {
        Task<PrivacyPolicy> GetActiveAsync();
        Task<PrivacyPolicy> GetByVersionAsync(int version);
        Task<List<PrivacyPolicy>> GetAllVersionsAsync();
    }
}

