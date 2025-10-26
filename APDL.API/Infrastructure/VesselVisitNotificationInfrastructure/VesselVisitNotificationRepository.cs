using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.VesselVisitInfrastructure
{
    public class VesselVisitNotificationRepository : BaseRepository<VesselVisitNotification, VesselVisitNotificationId>, IVesselVisitNotificationRepository
    {
        private readonly DDDSample1DbContext _context;

        public VesselVisitNotificationRepository(DDDSample1DbContext context) : base(context.VesselVisitNotifications)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(VesselVisitNotificationId id)
        {
            return await _context.VesselVisitNotifications
                .AnyAsync(v => v.Id == id);
        }

        public async Task<List<VesselVisitNotification>> GetByStatusAsync(string status)
        {
            return await _context.VesselVisitNotifications
                .Where(v => v.Status.Value == status)
                .ToListAsync();
        }

        public async Task<List<VesselVisitNotification>> GetPendingNotificationsAsync()
        {
            return await _context.VesselVisitNotifications
                .Where(v => v.Status.Value == "Submitted")
                .ToListAsync();
        }
    }
}