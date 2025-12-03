using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using APDL.API.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace APDL.API.Infrastructure.Authorization
{
    public class RoleAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string[] _allowedRoles;
        private readonly UserService _userService;
        private readonly ILogger<RoleAuthorizationFilter> _logger;

        public RoleAuthorizationFilter(
            string[] allowedRoles,
            UserService userService,
            ILogger<RoleAuthorizationFilter> logger
        )
        {
            _allowedRoles = allowedRoles;
            _userService = userService;
            _logger = logger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var email =
                context.HttpContext.User.FindFirstValue(ClaimTypes.Email)
                ?? context.HttpContext.User.FindFirstValue("email");

            if (string.IsNullOrEmpty(email))
            {
                context.Result = new UnauthorizedObjectResult(
                    new { error = "No email found in token" }
                );
                return;
            }

            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("Authorization failed - User not found: {Email}", email);
                context.Result = new ObjectResult(
                    new { error = "Forbidden", message = "User not registered or inactive" }
                )
                {
                    StatusCode = 403,
                };
                return;
            }

            if (!_allowedRoles.Contains(user.Role, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Unauthorized access - User: {Email}, Role: {UserRole}, Required: {RequiredRoles}, Endpoint: {Endpoint}",
                    email,
                    user.Role,
                    string.Join(", ", _allowedRoles),
                    context.HttpContext.Request.Path
                );

                context.Result = new ObjectResult(
                    new
                    {
                        error = "Forbidden",
                        message = "You do not have permission to access this resource",
                    }
                )
                {
                    StatusCode = 403,
                };
            }
        }
    }
}
