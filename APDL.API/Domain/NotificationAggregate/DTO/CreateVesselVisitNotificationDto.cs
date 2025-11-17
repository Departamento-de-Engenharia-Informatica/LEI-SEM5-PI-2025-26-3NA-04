using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.NotificationAggregate.DTO
{
    public class CreateVesselVisitNotificationDto
    {
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
    }
}
