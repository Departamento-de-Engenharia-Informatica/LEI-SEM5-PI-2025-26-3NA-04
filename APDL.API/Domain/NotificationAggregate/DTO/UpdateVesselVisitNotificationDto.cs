using System;
using System.Collections.Generic;

namespace APDL.API.Domain.NotificationAggregate.DTO
{
    public class UpdateVesselVisitNotificationDto
    {
        public Guid Id { get; set; }
        public DateTime ExpectedArrival { get; set; }
        public DateTime ExpectedDeparture { get; set; }
        public List<string> SafetyCrewOfficers { get; set; } = new();
    }
}