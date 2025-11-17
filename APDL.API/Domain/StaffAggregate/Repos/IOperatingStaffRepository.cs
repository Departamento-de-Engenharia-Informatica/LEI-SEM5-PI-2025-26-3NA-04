using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;
using APDL.API.Domain.StaffAggregate;

namespace APDL.API.Domain.OperatingStaffAggregate
{
    public interface IOperatingStaffRepository : IRepository<OperatingStaff, OperatingStaffId>
    {
        Task<OperatingStaff> GetByMecanographicNumberAsync(string mecanographicNumber);
        Task<bool> MecanographicNumberExistsAsync(string mecanographicNumber);
        Task<List<OperatingStaff>> GetAvailableStaffAsync();
        Task<List<OperatingStaff>> GetStaffByQualificationAsync(QualificationType qualification);
        Task<List<OperatingStaff>> GetAvailableStaffWithQualificationAsync(
            QualificationType qualification
        );
    }
}
