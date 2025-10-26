using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.DTO;
using APDL.API.Domain.ContainerAggregate.ValueObjects;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        private readonly ContainerService _service;

        public ContainersController(ContainerService service)
        {
            _service = service;
        }

        // GET: api/Containers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContainerDto>>> GetAll()
        {
            var containers = await _service.GetAllAsync();
            return Ok(containers);
        }

        // GET: api/Containers/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ContainerDto>> GetById(Guid id)
        {
            var container = await _service.GetByIdAsync(new ContainerId(id));
            if (container == null)
                return NotFound();

            return Ok(container);
        }

        // GET: api/Containers/number/{containerNumber}
        // Get container by its unique container number (e.g., MSCU1234567)
        [HttpGet("number/{containerNumber}")]
        public async Task<ActionResult<ContainerDto>> GetByContainerNumber(string containerNumber)
        {
            var container = await _service.GetByContainerNumberAsync(containerNumber);
            if (container == null)
                return NotFound();

            return Ok(container);
        }

        // POST: api/Containers
        [HttpPost]
        public async Task<ActionResult<ContainerDto>> Create(CreateContainerDto dto)
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

        // PUT: api/Containers/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ContainerDto>> Update(Guid id, UpdateContainerDto dto)
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

        // DELETE: api/Containers/{id}
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new ContainerId(id));
            if (deleted == null)
                return NotFound();

            return NoContent();
        }
    }
}