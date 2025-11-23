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
using System;
using Microsoft.Extensions.Configuration;


namespace APDL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DDDSample1DbContext _context;
        private readonly IConfiguration _config;


        public AuthController(UserService userService, DDDSample1DbContext context, IConfiguration config)
        {
            _userService = userService;
            _context = context;
            _config = config;
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


        [HttpGet("/callback")]
        public async Task<IActionResult> HandleActivationCallback([FromQuery] string userId)
        {
            // 1. Validar se o ID do utilizador veio na query string (enviado pelo UserService)
            if (string.IsNullOrEmpty(userId))
            {
                // Redireciona para o login com uma mensagem de erro
                return RedirectPermanent("~/login?status=activation_failed");
            }

            try
            {
                // 2. ATIVAR O UTILIZADOR NA BASE DE DADOS LOCAL
                // Este método deve procurar o utilizador pelo ID do Auth0 e mudar o estado (ex: IsActive = true).
                //await _userService.ActivateUserByAuth0IdAsync(userId); 

                // 3. REDIRECIONAR PARA A PÁGINA DE LOGIN
                // O tilde (~) refere-se à raiz da aplicação (o seu frontend, assumindo que está no mesmo domínio, 
                // ou deve ser o URL completo se o frontend for um domínio separado).
                return RedirectPermanent("~/login?status=activated");
            }
            catch (Exception ex)
            {
                // Em caso de erro (ex: utilizador não encontrado na BD local, falha de BD, etc.)
                // Redireciona para o login com uma mensagem de erro genérica para o utilizador
                return RedirectPermanent("~/login?status=activation_error");
            }
        }




    }
}
