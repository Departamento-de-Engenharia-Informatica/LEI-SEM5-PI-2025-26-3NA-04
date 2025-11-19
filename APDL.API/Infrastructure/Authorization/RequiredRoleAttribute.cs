using System;
using Microsoft.AspNetCore.Mvc;

namespace APDL.API.Infrastructure.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireRoleAttribute : TypeFilterAttribute
    {
        public RequireRoleAttribute(params string[] roles)
            : base(typeof(RoleAuthorizationFilter))
        {
            Arguments = [roles];
        }
    }
}
