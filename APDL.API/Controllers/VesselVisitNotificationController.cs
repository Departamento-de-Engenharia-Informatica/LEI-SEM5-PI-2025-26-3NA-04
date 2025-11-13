using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APDL.API.Domain.Shared;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.DTO;
using Microsoft.AspNetCore.Authorization;
using APDL.API.Infrastructure.Authorization;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VesselVisitNotificationsController : ControllerBase
    {
        private readonly VesselVisitNotificationService _service;

        public VesselVisitNotificationsController(VesselVisitNotificationService service)
        {
            _service = service;
        }

        [RequireRole("Admin", "Shipping Agent Representative", "Logistics Operator", "Port Authority Officer")]
        // GET: api/VesselVisitNotifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VesselVisitNotificationDto>>> GetAll()
        {
            var notifications = await _service.GetAllAsync();
            return Ok(notifications);
        }

        // GET: api/VesselVisitNotifications/{id}
        [RequireRole("Admin", "Shipping Agent Representative", "Logistics Operator", "Port Authority Officer")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VesselVisitNotificationDto>> GetById(Guid id)
        {
            var notification = await _service.GetByIdAsync(new VesselVisitNotificationId(id));
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        // GET: api/VesselVisitNotifications/status/{status}
        [RequireRole("Admin", "Shipping Agent Representative", "Logistics Operator", "Port Authority Officer")]
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<VesselVisitNotificationDto>>> GetByStatus(string status)
        {
            try
            {
                var notifications = await _service.GetByStatusAsync(status);
                return Ok(notifications);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/VesselVisitNotifications/pending
        [RequireRole("Admin", "Shipping Agent Representative", "Logistics Operator", "Port Authority Officer")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<VesselVisitNotificationDto>>> GetPending()
        {
            var notifications = await _service.GetPendingNotificationsAsync();
            return Ok(notifications);
        }

        // POST: api/VesselVisitNotifications
        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpPost]
        public async Task<ActionResult<VesselVisitNotificationDto>> Create(CreateVesselVisitNotificationDto dto)
        {
            try
            {
                var created = await _service.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/VesselVisitNotifications/{id}
        [RequireRole("Admin", "Shipping Agent Representative", "Port Authority Officer")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<VesselVisitNotificationDto>> Update(Guid id, UpdateVesselVisitNotificationDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            try
            {
                var updated = await _service.UpdateAsync(dto);
                if (updated == null)
                    return NotFound();

                return Ok(updated);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/VesselVisitNotifications/{id}/submit
        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpPost("{id:guid}/submit")]
        public async Task<ActionResult> Submit(Guid id)
        {
            try
            {
                await _service.SubmitAsync(id);
                return NoContent();
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/VesselVisitNotifications/{id}/approve
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost("{id:guid}/approve")]
        public async Task<ActionResult> Approve(Guid id)
        {
            try
            {
                await _service.ApproveAsync(id);
                return NoContent();
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/VesselVisitNotifications/{id}/reject
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost("{id:guid}/reject")]
        public async Task<ActionResult> Reject(Guid id)
        {
            try
            {
                await _service.RejectAsync(id);
                return NoContent();
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/VesselVisitNotifications/{id}
        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new VesselVisitNotificationId(id));
            if (deleted == null)
                return NotFound();

            return NoContent();
        }
    }
}