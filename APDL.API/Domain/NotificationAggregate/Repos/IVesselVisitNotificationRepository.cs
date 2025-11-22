using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselVisitAggregate;

namespace APDL.API.Domain.NotificationAggregate.Repos
{
    public interface IVesselVisitNotificationRepository
        : IRepository<VesselVisitNotification, VesselVisitNotificationId>
    {
        Task<bool> ExistsAsync(VesselVisitNotificationId id);
        Task<List<VesselVisitNotification>> GetByStatusAsync(string status);
        Task<List<VesselVisitNotification>> GetPendingNotificationsAsync();
        Task<List<VesselVisitNotification>> GetByVesselIdAsync(VesselId vesselId);
        Task<List<VesselVisitNotification>> GetByShippingAgentIdAsync(ShippingAgentId agentId);
        Task<List<VesselVisitNotification>> GetByDockIdAsync(DockId dockId);
        Task<List<VesselVisitNotification>> GetApprovedNotificationsAsync();
    }
}
