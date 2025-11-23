using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.UserAggregate;
using APDL.API.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            return Ok(new { userCount = count, emails = allEmails });
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
    }
}
