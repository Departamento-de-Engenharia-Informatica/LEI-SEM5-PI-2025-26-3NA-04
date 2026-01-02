using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.PrivacyPolicyAggregate;
using APDL.API.Domain.PrivacyPolicyAggregate.Repos;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.PrivacyPolicyInfrastructure
{
    public class PrivacyPolicyRepository : BaseRepository<PrivacyPolicy, PrivacyPolicyId>, IPrivacyPolicyRepository
    {
        private readonly DDDSample1DbContext _context;

        public PrivacyPolicyRepository(DDDSample1DbContext context) : base(context.PrivacyPolicies)
        {
            _context = context;
        }

        public async Task<PrivacyPolicy> GetActiveAsync()
        {
            return await _context.PrivacyPolicies
                .FirstOrDefaultAsync(p => p.IsActive);
        }

        public async Task<PrivacyPolicy> GetByVersionAsync(int version)
        {
            return await _context.PrivacyPolicies
                .FirstOrDefaultAsync(p => p.Version == version);
        }

        public async Task<List<PrivacyPolicy>> GetAllVersionsAsync()
        {
            return await _context.PrivacyPolicies.ToListAsync();
        }
    }
}

