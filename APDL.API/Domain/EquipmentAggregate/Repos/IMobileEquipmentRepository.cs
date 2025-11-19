using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.EquipmentAggregate;
using APDL.API.Domain.MobileEquipmentAggregate.ValueObjects;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.MobileEquipmentAggregate
{
    public interface IMobileEquipmentRepository : IRepository<MobileEquipment, MobileEquipmentId>
    {
        Task<List<MobileEquipment>> GetAvailableEquipmentAsync();
        Task<List<MobileEquipment>> GetEquipmentByTypeAsync(EquipmentType type);
        Task<List<MobileEquipment>> GetAvailableEquipmentByTypeAsync(EquipmentType type);
        Task<MobileEquipment> GetByNameAsync(string equipmentName);
        Task<bool> EquipmentNameExistsAsync(string equipmentName);
    }
}
