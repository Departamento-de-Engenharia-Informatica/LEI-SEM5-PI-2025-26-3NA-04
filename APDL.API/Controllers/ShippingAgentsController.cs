using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.DTO;
using Microsoft.AspNetCore.Authorization;
using APDL.API.Infrastructure.Authorization;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShippingAgentsController : ControllerBase
    {
        private readonly ShippingAgentService _service;

        public ShippingAgentsController(ShippingAgentService service)
        {
            _service = service;
        }

        // GET: api/ShippingAgent
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingAgentDto>>> GetAll()
        {
            var agents = await _service.GetAllAsync();
            return Ok(agents);
        }

        // GET: api/ShippingAgent/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingAgentDto>> GetById(string id)
        {
            var agent = await _service.GetByIdAsync(new ShippingAgentId(id));
            if (agent == null)
                return NotFound();

            return Ok(agent);
        }

        // POST: api/ShippingAgent
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost]
        public async Task<ActionResult<ShippingAgentDto>> Create(CreateShippingAgentDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/ShippingAgent/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ShippingAgentDto>> Update(Guid id, ShippingAgentDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var updated = await _service.UpdateAsync(dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        // DELETE: api/ShippingAgent/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new ShippingAgentId(id));
            if (deleted == null) return NotFound();

            return NoContent();
        }

        // POST
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost("{agentId}/representatives")]
        public async Task<IActionResult> AddRepresentativeToAgent(Guid agentId, [FromBody] string representativeEmail)
        {
            var repDto = await _service.AddRepresentativeToAgentAsync(agentId, representativeEmail);
            return Ok(repDto);
        }

    }
}
