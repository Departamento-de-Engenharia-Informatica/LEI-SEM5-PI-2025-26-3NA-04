using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using APDL.API.Infrastructure.Authorization;


namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VesselTypesController : ControllerBase
    {
        private readonly VesselTypeService _service;

        public VesselTypesController(VesselTypeService service)
        {
            _service = service;
        }

        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VesselTypeDto>>> GetAll()
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

        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VesselTypeDto>> GetById(string id)
        {
             try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null) return NotFound();
                return result;
            }
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
            
        }

        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpGet("name")]
        public async Task<ActionResult<IEnumerable<VesselTypeDto>>> GetByName([FromQuery] string name)
        {
            try
            {
                return await _service.SearchAsyncName(name);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }

        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpGet("description")]
        public async Task<ActionResult<IEnumerable<VesselTypeDto>>> GetByDescription([FromQuery] string description)
        {
            try
            {
                return await _service.SearchAsyncDescription(description);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }


        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpPost]
        public async Task<ActionResult<VesselTypeDto>> Create(CreatingVesselTypeDto dto)
        {
            try
            {
                var result = await _service.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }

        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpPut("{id}")]
        public async Task<ActionResult<VesselTypeDto>> Update(string id, CreatingVesselTypeDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }

        [RequireRole("Admin", "Shipping Agent Representative")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<VesselTypeDto>> Delete(string id)
        {
             try
            {
                var result = await _service.DeleteAsync(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
            
        }
    }
}