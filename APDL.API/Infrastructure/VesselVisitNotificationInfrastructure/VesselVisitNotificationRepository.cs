using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.VesselVisitInfrastructure
{
    public class VesselVisitNotificationRepository
        : BaseRepository<VesselVisitNotification, VesselVisitNotificationId>,
            IVesselVisitNotificationRepository
    {
        private readonly DDDSample1DbContext _context;

        public VesselVisitNotificationRepository(DDDSample1DbContext context)
            : base(context.VesselVisitNotifications)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(VesselVisitNotificationId id)
        {
            return await _context.VesselVisitNotifications.AnyAsync(v => v.Id == id);
        }

        public async Task<List<VesselVisitNotification>> GetByStatusAsync(string status)
        {
            return await _context
                .VesselVisitNotifications.Where(v => v.Status.Value == status)
                .ToListAsync();
        }

        public async Task<List<VesselVisitNotification>> GetPendingNotificationsAsync()
        {
            return await _context
                .VesselVisitNotifications.Where(v => v.Status.Value == "Submitted")
                .ToListAsync();
        }

        public async Task<List<VesselVisitNotification>> GetApprovedNotificationsAsync()
        {
            return await _context
                .VesselVisitNotifications.Where(v => v.Status.Value == "Approved")
                .ToListAsync();
        }

        public async Task<List<VesselVisitNotification>> GetByVesselIdAsync(VesselId vesselId)
        {
            return await _context
                .VesselVisitNotifications.Where(v => v.VesselId == vesselId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<VesselVisitNotification>> GetByShippingAgentIdAsync(
            ShippingAgentId agentId
        )
        {
            return await _context
                .VesselVisitNotifications.Where(v => v.ShippingAgentId == agentId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<VesselVisitNotification>> GetByDockIdAsync(DockId dockId)
        {
            return await _context
                .VesselVisitNotifications.Where(v =>
                    v.AssignedDockId == dockId && v.Status.Value == "Approved"
                )
                .OrderBy(v => v.ExpectedArrival.Value)
                .ToListAsync();
        }

    }
}
