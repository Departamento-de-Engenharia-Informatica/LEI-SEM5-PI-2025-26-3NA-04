using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DDDSample1.Application.DTOs;
using DDDSample1.Application.Services;
using DDDSample1.Domain.Vessels;
using DDDSample1.Domain.Shared;

namespace DDDSample1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VesselsController : ControllerBase
    {
        private readonly VesselService _service;

        public VesselsController(VesselService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VesselDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VesselDto>> GetById(Guid id)
        {
            try
            {

            }
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
            var result = await _service.GetByIdAsync(new VesselId(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("imo/{imoNumber}")]
        public async Task<ActionResult<VesselDto>> GetByImo(string imoNumber)
        {
            try
            {
                var result = await _service.GetByImoAsync(imoNumber);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
            
        }

        [HttpGet("search/name")]
        public async Task<ActionResult<IEnumerable<VesselDto>>> SearchByName([FromQuery] string name)
        {
            try
            {
                var result = await _service.SearchByNameAsync(name);
                return Ok(result);
            }
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
            
        }

        [HttpGet("search/operator")]
        public async Task<ActionResult<IEnumerable<VesselDto>>> SearchByOperator([FromQuery] string operatorName)
        {
            try
            {
                var result = await _service.SearchByOperatorAsync(operatorName);
                return Ok(result);
            }
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<VesselDto>> Create([FromBody] CreatingVesselDto dto)
        {
            try
            {
                var result = await _service.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch(BusinessRuleValidationException ex)
            {
                return BadRequest(new {Message = ex.Message});
            }
            
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<VesselDto>> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(new VesselId(id));
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