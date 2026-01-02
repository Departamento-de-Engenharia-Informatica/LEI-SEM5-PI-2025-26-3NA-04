using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using APDL.API.Domain.PrivacyPolicyAggregate;
using APDL.API.Domain.PrivacyPolicyAggregate.DTO;
using APDL.API.Domain.Shared;
using APDL.API.Domain.UserAggregate;
using APDL.API.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivacyPolicyController : ControllerBase
    {
        private readonly PrivacyPolicyService _service;
        private readonly UserService _userService;

        public PrivacyPolicyController(PrivacyPolicyService service, UserService userService)
        {
            _service = service;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<ActionResult<PrivacyPolicyDto>> GetActivePolicy()
        {
            try
            {
                var policy = await _service.GetActivePolicyAsync();
                if (policy == null)
                {
                    return NotFound(new { message = "No active privacy policy found." });
                }
                return Ok(policy);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving privacy policy", details = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("versions/{version}")]
        public async Task<ActionResult<PrivacyPolicyDto>> GetPolicyByVersion(int version)
        {
            try
            {
                var policy = await _service.GetByVersionAsync(version);
                if (policy == null)
                {
                    return NotFound(new { message = $"Privacy policy version {version} not found." });
                }
                return Ok(policy);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving privacy policy", details = ex.Message });
            }
        }

        [Authorize]
        [RequireRole("Admin")]
        [HttpGet("versions")]
        public async Task<ActionResult> GetAllVersions()
        {
            try
            {
                var policies = await _service.GetAllVersionsAsync();
                return Ok(policies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error retrieving privacy policy versions", details = ex.Message });
            }
        }

        [Authorize]
        [RequireRole("Admin")]
        [HttpPost]
        public async Task<ActionResult<PrivacyPolicyDto>> PublishNewVersion([FromBody] CreatePrivacyPolicyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                    ?? User.FindFirstValue("email")
                    ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
                    ?? User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new { error = "Unable to identify user" });
                }

                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return Unauthorized(new { error = "User not found" });
                }

                var userId = user.Id;
                var newPolicy = await _service.PublishNewVersionAsync(dto, userId);

                try
                {
                    System.Console.WriteLine("Calling SetPrivacyPolicyNotificationForAllUsersAsync after publishing version " + newPolicy.Version);
                    await _userService.SetPrivacyPolicyNotificationForAllUsersAsync();
                    System.Console.WriteLine("Successfully called SetPrivacyPolicyNotificationForAllUsersAsync");
                }
                catch (Exception notificationEx)
                {
                    System.Console.Error.WriteLine($"ERROR: Failed to set privacy policy notifications for users: {notificationEx.Message}");
                    System.Console.Error.WriteLine($"Stack trace: {notificationEx.StackTrace}");
                }

                return Ok(newPolicy);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"Error publishing privacy policy: {ex}");
                System.Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = "Error publishing privacy policy", details = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        [Authorize]
        [RequireRole("Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<PrivacyPolicyDto>> UpdatePolicy(string id, [FromBody] UpdatePrivacyPolicyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var policy = await _service.UpdatePolicyAsync(id, dto);
                if (policy == null)
                {
                    return NotFound(new { message = "Privacy policy not found." });
                }
                return Ok(policy);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error updating privacy policy", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("acknowledge")]
        public async Task<ActionResult> AcknowledgePolicy([FromBody] AcknowledgePrivacyPolicyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                    ?? User.FindFirstValue("email")
                    ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
                    ?? User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new { error = "Unable to identify user" });
                }

                await _userService.AcknowledgePrivacyPolicyAsync(email, dto.Version);
                return Ok(new { message = "Privacy policy acknowledged successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error acknowledging privacy policy", details = ex.Message });
            }
        }
    }
}

