using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Application.Services;
using DDDSample1.Application.DTOs;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VesselTypesController : ControllerBase
    {
        private readonly VesselTypeService _service;

        public VesselTypesController(VesselTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VesselTypeDto>>> GetAll()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VesselTypeDto>> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<VesselTypeDto>> Create(CreatingVesselTypeDto dto)
        {
            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<VesselTypeDto>> Update(string id, CreatingVesselTypeDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<VesselTypeDto>> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}