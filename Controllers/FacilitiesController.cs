using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DDDSample1.Domain.Storage;
using DDDSample1.Application.DTOs;
using DDDSample1.Application.Services;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilitiesController : ControllerBase
    {
        private readonly FacilityService _service;

        public FacilitiesController(FacilityService service)
        {
            _service = service;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityDto>>> GetAll()
        {
            try
            {
                return await _service.GetAllAsync();
            }
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
        }

        // GET: api/Products/id
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
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
        }

        // POST: api/Products
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