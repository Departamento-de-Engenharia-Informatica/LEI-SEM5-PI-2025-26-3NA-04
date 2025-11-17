using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.QualificationsAggregate;
using APDL.API.Domain.Shared.ValueObjects;
using APDL.API.Domain.StaffQualificationAggregate;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.StaffQualificationInfrastructure
{
    public class StaffQualificationRepository
        : BaseRepository<StaffQualification, StaffQualificationId>,
            IStaffQualificationRepository
    {
        private readonly DDDSample1DbContext _context;

        public StaffQualificationRepository(DDDSample1DbContext context)
            : base(context.StaffQualifications)
        {
            _context = context;
        }

        public async Task<List<StaffQualification>> GetActiveQualificationsAsync()
        {
            return await _context.StaffQualifications.Where(q => q.IsActive).ToListAsync();
        }

        public async Task<StaffQualification> GetByTypeAsync(QualificationType type)
        {
            return await _context.StaffQualifications.FirstOrDefaultAsync(q =>
                q.QualificationType.Value == type.Value
            );
        }

        public async Task<StaffQualification> GetByNameAsync(string qualificationName)
        {
            return await _context.StaffQualifications.FirstOrDefaultAsync(q =>
                q.QualificationName == qualificationName.Trim()
            );
        }

        public async Task<bool> QualificationTypeExistsAsync(QualificationType type)
        {
            return await _context.StaffQualifications.AnyAsync(q =>
                q.QualificationType.Value == type.Value
            );
        }
    }
}
