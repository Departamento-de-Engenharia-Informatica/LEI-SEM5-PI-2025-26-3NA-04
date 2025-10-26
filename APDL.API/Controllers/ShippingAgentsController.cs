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
    public class ShippingAgentsController : ControllerBase
    {
        private readonly ShippingAgentService _service;

        public ShippingAgentsController(ShippingAgentService service)
        {
            _service = service;
        }

        // GET: api/ShippingAgent
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingAgentDto>>> GetAll()
        {
            var agents = await _service.GetAllAsync();
            return Ok(agents);
        }

        // GET: api/ShippingAgent/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingAgentDto>> GetById(string id)
        {
            var agent = await _service.GetByIdAsync(new ShippingAgentId(id));
            if (agent == null)
                return NotFound();

            return Ok(agent);
        }

        // POST: api/ShippingAgent
        [HttpPost]
        public async Task<ActionResult<ShippingAgentDto>> Create(CreateShippingAgentDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/ShippingAgent/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ShippingAgentDto>> Update(Guid id, ShippingAgentDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var updated = await _service.UpdateAsync(dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE: api/ShippingAgent/{id}
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new ShippingAgentId(id));
            if (deleted == null) return NotFound();

            return NoContent();
        }

        // POST
        [HttpPost("{agentId}/representatives")]
        public async Task<IActionResult> AddRepresentativeToAgent(Guid agentId, [FromBody] string representativeEmail)
        {
            var repDto = await _service.AddRepresentativeToAgentAsync(agentId, representativeEmail);
            return Ok(repDto);
        }

    }
}
