using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.EquipmentAggregate;
using APDL.API.Domain.MobileEquipmentAggregate;
using APDL.API.Domain.MobileEquipmentAggregate.ValueObjects;
using APDL.API.Domain.Shared.ValueObjects;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.MobileEquipmentInfrastructure
{
    public class MobileEquipmentRepository
        : BaseRepository<MobileEquipment, MobileEquipmentId>,
            IMobileEquipmentRepository
    {
        private readonly DDDSample1DbContext _context;

        public MobileEquipmentRepository(DDDSample1DbContext context)
            : base(context.MobileEquipment)
        {
            _context = context;
        }

        public async Task<List<MobileEquipment>> GetAvailableEquipmentAsync()
        {
            return await _context
                .MobileEquipment.Where(e => e.Status.Value == ResourceStatusEnum.AVAILABLE)
                .ToListAsync();
        }

        public async Task<List<MobileEquipment>> GetEquipmentByTypeAsync(EquipmentType type)
        {
            return await _context
                .MobileEquipment.Where(e => e.EquipmentType.Value == type.Value)
                .ToListAsync();
        }

        public async Task<List<MobileEquipment>> GetAvailableEquipmentByTypeAsync(
            EquipmentType type
        )
        {
            return await _context
                .MobileEquipment.Where(e =>
                    e.Status.Value == ResourceStatusEnum.AVAILABLE
                    && e.EquipmentType.Value == type.Value
                )
                .ToListAsync();
        }

        public async Task<MobileEquipment> GetByNameAsync(string equipmentName)
        {
            return await _context.MobileEquipment.FirstOrDefaultAsync(e =>
                e.EquipmentName == equipmentName.Trim()
            );
        }

        public async Task<bool> EquipmentNameExistsAsync(string equipmentName)
        {
            return await _context.MobileEquipment.AnyAsync(e =>
                e.EquipmentName == equipmentName.Trim()
            );
        }
    }
}
