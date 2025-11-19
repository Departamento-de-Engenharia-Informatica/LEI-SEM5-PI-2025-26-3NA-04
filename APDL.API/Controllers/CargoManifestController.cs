using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.ManifestAggregate.DTO;
using APDL.API.Domain.CargoManifestAggregate.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using APDL.API.Infrastructure.Authorization;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CargoManifestsController : ControllerBase
    {
        private readonly CargoManifestService _service;

        public CargoManifestsController(CargoManifestService service)
        {
            _service = service;
        }

        // GET: api/CargoManifests
        [RequireRole("Admin", "Shipping Agent Representative", "Logistics Operator", "Port Authority Officer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CargoManifestDto>>> GetAll()
        {
            var manifests = await _service.GetAllAsync();
            return Ok(manifests);
        }

        // GET: api/CargoManifests/{id}
        [RequireRole("Admin", "Shipping Agent Representative", "Logistics Operator", "Port Authority Officer")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CargoManifestDto>> GetById(Guid id)
        {
            var manifest = await _service.GetByIdAsync(new CargoManifestId(id));
            if (manifest == null)
                return NotFound();

            return Ok(manifest);
        }

        // GET: api/CargoManifests/vessel/{vesselVisitNotificationId}
        [RequireRole("Admin", "Shipping Agent Representative", "Logistics Operator", "Port Authority Officer")]
        [HttpGet("vessel/{vesselVisitNotificationId:guid}")]
        public async Task<ActionResult<IEnumerable<CargoManifestDto>>> GetByVesselVisit(Guid vesselVisitNotificationId)
        {
            var manifests = await _service.GetByVesselVisitAsync(vesselVisitNotificationId);
            return Ok(manifests);
        }

        // POST: api/CargoManifests
        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpPost]
        public async Task<ActionResult<CargoManifestDto>> Create(CreateCargoManifestDto dto)
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

        // PUT: api/CargoManifests/{id}
        [RequireRole("Admin", "Shipping Agent Representative", "Port Authority Officer")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CargoManifestDto>> Update(Guid id, UpdateCargoManifestDto dto)
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

        // POST: api/CargoManifests/{manifestId}/containers/{containerId}
        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpPost("{manifestId:guid}/containers/{containerId:guid}")]
        public async Task<ActionResult> AddContainer(Guid manifestId, Guid containerId)
        {
            try
            {
                await _service.AddContainerAsync(manifestId, containerId);
                return NoContent();
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/CargoManifests/{manifestId}/containers/{containerId}
        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpDelete("{manifestId:guid}/containers/{containerId:guid}")]
        public async Task<ActionResult> RemoveContainer(Guid manifestId, Guid containerId)
        {
            try
            {
                await _service.RemoveContainerAsync(manifestId, containerId);
                return NoContent();
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/CargoManifests/{id}
        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(new CargoManifestId(id));
            if (deleted == null) 
                return NotFound();

            return NoContent();
        }
    }
}