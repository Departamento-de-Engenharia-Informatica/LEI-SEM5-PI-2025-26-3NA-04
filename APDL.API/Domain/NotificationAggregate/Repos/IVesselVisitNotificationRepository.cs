using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.VesselVisitAggregate;

namespace APDL.API.Domain.NotificationAggregate.Repos
{
        public interface IVesselVisitNotificationRepository : IRepository<VesselVisitNotification, VesselVisitNotificationId>
    {
        Task<bool> ExistsAsync(VesselVisitNotificationId id);
        Task<List<VesselVisitNotification>> GetByStatusAsync(string status);
        Task<List<VesselVisitNotification>> GetPendingNotificationsAsync();
    }
}