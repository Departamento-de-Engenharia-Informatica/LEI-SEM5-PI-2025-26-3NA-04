using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.QualificationsAggregate;
using APDL.API.Domain.Shared;
using APDL.API.Domain.StaffQualificationAggregate;
using APDL.API.Domain.StaffQualificationAggregate.DTO;
using APDL.API.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class StaffQualificationsController : ControllerBase
    {
        private readonly StaffQualificationService _service;

        public StaffQualificationsController(StaffQualificationService service)
        {
            _service = service;
        }

        // GET: api/StaffQualifications
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffQualificationDto>>> GetAll()
        {
            var qualifications = await _service.GetAllAsync();
            return Ok(qualifications);
        }

        // GET: api/StaffQualifications/active
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<StaffQualificationDto>>> GetActive()
        {
            var qualifications = await _service.GetActiveQualificationsAsync();
            return Ok(qualifications);
        }

        // GET: api/StaffQualifications/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StaffQualificationDto>> GetById(Guid id)
        {
            var qualification = await _service.GetByIdAsync(new StaffQualificationId(id));
            if (qualification == null)
                return NotFound();

            return Ok(qualification);
        }

        // POST: api/StaffQualifications
        //[RequireRole("Admin", "Port Authority Officer")]
        [HttpPost]
        public async Task<ActionResult<StaffQualificationDto>> Create(
            CreateStaffQualificationDto dto
        )
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

        // PUT: api/StaffQualifications/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<StaffQualificationDto>> Update(
            Guid id,
            UpdateStaffQualificationDto dto
        )
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

        // POST: api/StaffQualifications/{id}/activate
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost("{id:guid}/activate")]
        public async Task<ActionResult<StaffQualificationDto>> Activate(Guid id)
        {
            var activated = await _service.ActivateAsync(new StaffQualificationId(id));
            if (activated == null)
                return NotFound();

            return Ok(activated);
        }

        // POST: api/StaffQualifications/{id}/deactivate
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost("{id:guid}/deactivate")]
        public async Task<ActionResult<StaffQualificationDto>> Deactivate(Guid id)
        {
            var deactivated = await _service.DeactivateAsync(new StaffQualificationId(id));
            if (deactivated == null)
                return NotFound();

            return Ok(deactivated);
        }

        // DELETE: api/StaffQualifications/{id}
        [RequireRole("Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new StaffQualificationId(id));
            if (deleted == null)
                return NotFound();

            return NoContent();
        }
    }
}
