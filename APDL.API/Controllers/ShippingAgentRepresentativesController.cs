using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.DTO;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingAgentRepresentativesController : ControllerBase
    {
        private readonly ShippingAgentRepresentativeService _service;

        public ShippingAgentRepresentativesController(ShippingAgentRepresentativeService service)
        {
            _service = service;
        }

        // GET: api/ShippingAgentRepresentatives
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepresentativeDto>>> GetAll()
        {
            var reps = await _service.GetAllAsync();
            return Ok(reps);
        }

        // GET: api/ShippingAgentRepresentatives/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RepresentativeDto>> GetById(Guid id)
        {
            var rep = await _service.GetByIdAsync(new ShippingAgentRepresentativeId(id));
            if (rep == null) return NotFound();
            return Ok(rep);
        }

        // POST: api/ShippingAgentRepresentatives
        [HttpPost]
        public async Task<ActionResult<RepresentativeDto>> Create(CreateRepresentativeDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/ShippingAgentRepresentatives/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<RepresentativeDto>> Update(Guid id, RepresentativeDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var updated = await _service.UpdateAsync(dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE: api/ShippingAgentRepresentatives/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new ShippingAgentRepresentativeId(id));
            if (deleted == null) return NotFound();

            return NoContent();
        }

        // PATCH: api/ShippingAgentRepresentatives/{id}/deactivate
        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            var rep = await _service.GetByIdAsync(new ShippingAgentRepresentativeId(id));
            if (rep == null) return NotFound();

            await _service.DeactivateAsync(new ShippingAgentRepresentativeId(id));

            return NoContent();
        }

        // PATCH: api/ShippingAgentRepresentatives/{id}/reactivate
        [HttpPatch("{id}/reactivate")]
        public async Task<ActionResult> Reactivate(Guid id)
        {
            var rep = await _service.GetByIdAsync(new ShippingAgentRepresentativeId(id));
            if (rep == null) return NotFound();

            await _service.ReactivateAsync(new ShippingAgentRepresentativeId(id));

            return NoContent();
        }
    }
}
