using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.QualificationsAggregate;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;

namespace APDL.API.Domain.StaffQualificationAggregate
{
    public interface IStaffQualificationRepository
        : IRepository<StaffQualification, StaffQualificationId>
    {
        Task<List<StaffQualification>> GetActiveQualificationsAsync();
        Task<StaffQualification> GetByTypeAsync(QualificationType type);
        Task<StaffQualification> GetByNameAsync(string qualificationName);
        Task<bool> QualificationTypeExistsAsync(QualificationType type);
    }
}
