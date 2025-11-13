using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using APDL.API.Domain.Storage;
using APDL.API.Domain.Shared;
using APDL.API.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FacilitiesController : ControllerBase
    {
        private readonly FacilityService _service;

        public FacilitiesController(FacilityService service)
        {
            _service = service;
        }

        // GET: api/Products
        [RequireRole("Admin", "Logistics Operator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityDto>>> GetAll()
        {
            try
            {
                return await _service.GetAllAsync();
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // GET: api/Products/id
        [RequireRole("Admin", "Logistics Operator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityDto>> GetById(Guid id)
        {
            try
            {
                var facility = await _service.GetByIdAsync(new FacilityId(id));
                if (facility == null)
                    return NotFound();
                return facility;
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // POST: api/Products
        [RequireRole("Admin", "Logistics Operator")]
        [HttpPost]
        public async Task<ActionResult<FacilityDto>> Create(CreatingFacilityDto dto)
        {
            try
            {
                var facility = await _service.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = facility.Id }, facility);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // PUT: api/Products/id
        [RequireRole("Admin", "Logistics Operator")]
        [HttpPut("{id}")]
        public async Task<ActionResult<FacilityDto>> Update(Guid id, FacilityDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            try
            {
                var updated = await _service.UpdateAsync(dto.CurrentOccupancyTEU, dto);
                if (updated == null)
                    return NotFound();
                return Ok(updated);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/Products/id
        [RequireRole("Admin", "Logistics Operator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<FacilityDto>> SoftDelete(Guid id)
        {
            try
            {
                var facility = await _service.DeleteAsync(new FacilityId(id));
                if (facility == null)
                    return NotFound();
                return Ok(facility);
            }
            catch(BusinessRuleValidationException ex)
            {
               return BadRequest(new {Message = ex.Message});
            }

        }
    }
}