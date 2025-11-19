using System;
using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.NotificationAggregate.DTO
{
    public class ApproveNotificationDto
    {
        public Guid AssignedDockId { get; set; }
    }
}
