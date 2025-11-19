using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APDL.API.Domain.Shared;
using APDL.API.Domain.MobileEquipmentAggregate;
using APDL.API.Domain.MobileEquipmentAggregate.DTO;
using APDL.API.Domain.MobileEquipmentAggregate.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using APDL.API.Infrastructure.Authorization;
using APDL.API.Domain.EquipmentAggregate;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MobileEquipmentController : ControllerBase
    {
        private readonly MobileEquipmentService _service;

        public MobileEquipmentController(MobileEquipmentService service)
        {
            _service = service;
        }

        // GET: api/MobileEquipment
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobileEquipmentDto>>> GetAll()
        {
            var equipment = await _service.GetAllAsync();
            return Ok(equipment);
        }

        // GET: api/MobileEquipment/available
        [RequireRole("Admin", "Logistics Operator")]
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<MobileEquipmentDto>>> GetAvailable()
        {
            var equipment = await _service.GetAvailableEquipmentAsync();
            return Ok(equipment);
        }

        // GET: api/MobileEquipment/type/{equipmentType}
        [RequireRole("Admin", "Logistics Operator")]
        [HttpGet("type/{equipmentType}")]
        public async Task<ActionResult<IEnumerable<MobileEquipmentDto>>> GetByType(string equipmentType)
        {
            try
            {
                var equipment = await _service.GetEquipmentByTypeAsync(equipmentType);
                return Ok(equipment);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/MobileEquipment/{id}
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MobileEquipmentDto>> GetById(Guid id)
        {
            var equipment = await _service.GetByIdAsync(new MobileEquipmentId(id));
            if (equipment == null)
                return NotFound();

            return Ok(equipment);
        }

        // POST: api/MobileEquipment
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost]
        public async Task<ActionResult<MobileEquipmentDto>> Create(CreateMobileEquipmentDto dto)
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

        // PUT: api/MobileEquipment/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MobileEquipmentDto>> Update(Guid id, UpdateMobileEquipmentDto dto)
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

        // PUT: api/MobileEquipment/{id}/status
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpPut("{id:guid}/status")]
        public async Task<ActionResult<MobileEquipmentDto>> UpdateStatus(Guid id, UpdateEquipmentStatusDto dto)
        {
            try
            {
                var updated = await _service.UpdateStatusAsync(new MobileEquipmentId(id), dto.Status);
                if (updated == null)
                    return NotFound();

                return Ok(updated);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/MobileEquipment/{id}
        [RequireRole("Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new MobileEquipmentId(id));
            if (deleted == null)
                return NotFound();

            return NoContent();
        }
    }
}