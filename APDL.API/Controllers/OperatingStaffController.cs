using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.OperatingStaffAggregate;
using APDL.API.Domain.OperatingStaffAggregate.DTO;
using APDL.API.Domain.Shared;
using APDL.API.Domain.StaffAggregate;
using APDL.API.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OperatingStaffController : ControllerBase
    {
        private readonly OperatingStaffService _service;

        public OperatingStaffController(OperatingStaffService service)
        {
            _service = service;
        }

        // GET: api/OperatingStaff
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperatingStaffDto>>> GetAll()
        {
            var staff = await _service.GetAllAsync();
            return Ok(staff);
        }

        // GET: api/OperatingStaff/available
        [RequireRole("Admin", "Logistics Operator")]
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<OperatingStaffDto>>> GetAvailable()
        {
            var staff = await _service.GetAvailableStaffAsync();
            return Ok(staff);
        }

        // GET: api/OperatingStaff/qualification/{qualificationType}
        [RequireRole("Admin", "Logistics Operator")]
        [HttpGet("qualification/{qualificationType}")]
        public async Task<ActionResult<IEnumerable<OperatingStaffDto>>> GetByQualification(
            string qualificationType
        )
        {
            try
            {
                var staff = await _service.GetStaffByQualificationAsync(qualificationType);
                return Ok(staff);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/OperatingStaff/{id}
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OperatingStaffDto>> GetById(Guid id)
        {
            var staff = await _service.GetByIdAsync(new OperatingStaffId(id));
            if (staff == null)
                return NotFound();

            return Ok(staff);
        }

        // GET: api/OperatingStaff/mecanographic/{mecanographicNumber}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpGet("mecanographic/{mecanographicNumber}")]
        public async Task<ActionResult<OperatingStaffDto>> GetByMecanographicNumber(
            string mecanographicNumber
        )
        {
            var staff = await _service.GetByMecanographicNumberAsync(mecanographicNumber);
            if (staff == null)
                return NotFound();

            return Ok(staff);
        }

        // POST: api/OperatingStaff
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost]
        public async Task<ActionResult<OperatingStaffDto>> Create(CreateOperatingStaffDto dto)
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

        // PUT: api/OperatingStaff/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<OperatingStaffDto>> Update(
            Guid id,
            UpdateOperatingStaffDto dto
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

        // POST: api/OperatingStaff/{id}/qualifications
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost("{id:guid}/qualifications")]
        public async Task<ActionResult<OperatingStaffDto>> AddQualification(
            Guid id,
            AddQualificationDto dto
        )
        {
            try
            {
                var updated = await _service.AddQualificationAsync(
                    new OperatingStaffId(id),
                    dto.QualificationType
                );
                if (updated == null)
                    return NotFound();

                return Ok(updated);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/OperatingStaff/{id}/qualifications/{qualificationType}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpDelete("{id:guid}/qualifications/{qualificationType}")]
        public async Task<ActionResult<OperatingStaffDto>> RemoveQualification(
            Guid id,
            string qualificationType
        )
        {
            try
            {
                var updated = await _service.RemoveQualificationAsync(
                    new OperatingStaffId(id),
                    qualificationType
                );
                if (updated == null)
                    return NotFound();

                return Ok(updated);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/OperatingStaff/{id}/status
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpPut("{id:guid}/status")]
        public async Task<ActionResult<OperatingStaffDto>> UpdateStatus(
            Guid id,
            UpdateStaffStatusDto dto
        )
        {
            try
            {
                var updated = await _service.UpdateStatusAsync(
                    new OperatingStaffId(id),
                    dto.Status
                );
                if (updated == null)
                    return NotFound();

                return Ok(updated);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/OperatingStaff/{id}
        [RequireRole("Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new OperatingStaffId(id));
            if (deleted == null)
                return NotFound();

            return NoContent();
        }
    }
}
