using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.UserAggregate;
using APDL.API.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APDL.API.Domain.UserAggregate.DTO;
namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DDDSample1DbContext _context;


        public AuthController(UserService userService, DDDSample1DbContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpGet("count-users")]
        [AllowAnonymous]
        public async Task<IActionResult> CountUsers()
        {
            var count = await _context.Users.CountAsync();
            var allEmails = await _context.Users.Select(u => u.Email).ToListAsync();

            return Ok(new
            {
                userCount = count,
                emails = allEmails
            });
        }

        [HttpGet("whoami")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email =
                User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
                )
                ?? User.FindFirstValue("email")
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/emailaddress"))?.Value
                ?? User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            System.Console.WriteLine("=== TOKEN CLAIMS ===");
            foreach (var claim in User.Claims)
            {
                System.Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            System.Console.WriteLine("===================");

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(
                    new
                    {
                        error = "No email claim found in token",
                        availableClaims = User.Claims.Select(c => c.Type).ToList(),
                    }
                );
            }

            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound(
                    new
                    {
                        error = "User not registered",
                        message = "Your account has not been activated. Please contact Port Authority.",
                        email = email,
                    }
                );
            }

            return Ok(
                new
                {
                    id = user.Id.ToString(),
                    email = user.Email,
                    name = user.Name,
                    role = user.Role,
                    isAuthenticated = true,
                }
            );
        }

        
        
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            /*var callerRole =
                User.FindFirstValue("role")
                ?? User.Claims.FirstOrDefault(c => c.Type.EndsWith("/role"))?.Value;

            if (string.IsNullOrWhiteSpace(callerRole) || !callerRole.Equals("ADMIN", System.StringComparison.OrdinalIgnoreCase))
            {
                return Forbid(); // 403
            }*/

            try
            {
                var created = await _userService.CreateUserAsync(dto);

                return Ok(
                new
                {
                    id = created.Id.ToString(),
                    email = created.Email,
                    name = created.Name,
                    role = created.Role,
                    isAuthenticated = true,
                }
            );
            }
            catch (System.Exception ex)
            {
                System.Console.Error.WriteLine($"Error creating user: {ex}");
                return StatusCode(500, new
                {
                    error = "Error creating user",
                    details = ex.Message
                });
            }
        }

        
        [HttpGet("activate")]
        public IActionResult Activate([FromQuery] string token)
        {
            try
            {
                // Chama o serviço para validar token e gerar URL do Auth0
                var redirectUrl = _userService.ProcessActivationToken(token);
                return Redirect(redirectUrl);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = "Invalid or expired activation link", details = ex.Message });
            }
        }




    }
}
