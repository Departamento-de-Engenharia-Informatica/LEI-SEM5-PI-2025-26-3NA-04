using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.DockAggregate.DTO;
using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocksController : ControllerBase
    {
        private readonly DockService _service;

        public DocksController(DockService service)
        {
            _service = service;
        }

        // GET: api/Docks
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DockDto>>> GetAll()
        {
            var docks = await _service.GetAllAsync();
            return Ok(docks);
        }

        // GET: api/Docks/{id}
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DockDto>> GetById(Guid id)
        {
            var dock = await _service.GetByIdAsync(new DockId(id));
            if (dock == null)
                return NotFound();

            return Ok(dock);
        }

        // GET: api/Docks/name/{dockName}
        [RequireRole("Admin", "Port Authority Officer", "Logistics Operator")]
        [HttpGet("name/{dockName}")]
        public async Task<ActionResult<DockDto>> GetByName(string dockName)
        {
            var dock = await _service.GetByNameAsync(dockName);
            if (dock == null)
                return NotFound();

            return Ok(dock);
        }

        // GET: api/Docks/capable?vesselLength=350&vesselDraft=15
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpGet("capable")]
        public async Task<ActionResult<IEnumerable<DockDto>>> GetDocksCapableOfVessel(
            [FromQuery] int vesselLength,
            [FromQuery] int vesselDraft
        )
        {
            var docks = await _service.GetDocksCapableOfVesselAsync(vesselLength, vesselDraft);
            return Ok(docks);
        }

        // POST: api/Docks
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPost]
        public async Task<ActionResult<DockDto>> Create(CreateDockDto dto)
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

        // PUT: api/Docks/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<DockDto>> Update(Guid id, UpdateDockDto dto)
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

        // DELETE: api/Docks/{id}
        [RequireRole("Admin", "Port Authority Officer")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new DockId(id));
            if (deleted == null)
                return NotFound();

            return NoContent();
        }
    }
}
