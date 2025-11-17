using System;
using System.Collections.Generic;

namespace APDL.API.Domain.NotificationAggregate.DTO
{
    public class VesselVisitNotificationDto
    {
        public Guid Id { get; set; }
        public Guid VesselId { get; set; }
        public Guid ShippingAgentId { get; set; }

        public DateTime ExpectedArrival { get; set; }
        public DateTime ExpectedDeparture { get; set; }

        public string CargoType { get; set; }
        public int CargoVolume { get; set; }
        public string SpecialHandlingRequirements { get; set; }

        public string CaptainName { get; set; }
        public int TotalCrewCount { get; set; }
        public List<string> SafetyCrewOfficers { get; set; }

        public string Status { get; set; }
        public Guid? AssignedDockId { get; set; }
        public string RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
