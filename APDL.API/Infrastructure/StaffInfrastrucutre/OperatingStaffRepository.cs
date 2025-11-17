using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.OperatingStaffAggregate;
using APDL.API.Domain.OperatingStaffAggregate.ValueObjects;
using APDL.API.Domain.Shared.ValueObjects;
using APDL.API.Domain.StaffAggregate;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.OperatingStaffInfrastructure
{
    public class OperatingStaffRepository
        : BaseRepository<OperatingStaff, OperatingStaffId>,
            IOperatingStaffRepository
    {
        private readonly DDDSample1DbContext _context;

        public OperatingStaffRepository(DDDSample1DbContext context)
            : base(context.OperatingStaff)
        {
            _context = context;
        }

        public async Task<OperatingStaff> GetByMecanographicNumberAsync(string mecanographicNumber)
        {
            return await _context.OperatingStaff.FirstOrDefaultAsync(s =>
                s.MecanographicNumber.Value == mecanographicNumber.ToUpper().Trim()
            );
        }

        public async Task<bool> MecanographicNumberExistsAsync(string mecanographicNumber)
        {
            return await _context.OperatingStaff.AnyAsync(s =>
                s.MecanographicNumber.Value == mecanographicNumber.ToUpper().Trim()
            );
        }

        public async Task<List<OperatingStaff>> GetAvailableStaffAsync()
        {
            return await _context
                .OperatingStaff.Where(s => s.Status.Value == StaffStatusEnum.AVAILABLE)
                .ToListAsync();
        }

        public async Task<List<OperatingStaff>> GetStaffByQualificationAsync(
            QualificationType qualification
        )
        {
            return await _context
                .OperatingStaff.Where(s =>
                    s.Qualifications.Any(q => q.Value == qualification.Value)
                )
                .ToListAsync();
        }

        public async Task<List<OperatingStaff>> GetAvailableStaffWithQualificationAsync(
            QualificationType qualification
        )
        {
            return await _context
                .OperatingStaff.Where(s =>
                    s.Status.Value == StaffStatusEnum.AVAILABLE
                    && s.Qualifications.Any(q => q.Value == qualification.Value)
                )
                .ToListAsync();
        }
    }
}
