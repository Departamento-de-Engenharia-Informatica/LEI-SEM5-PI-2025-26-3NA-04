using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.UserAggregate.DTO
{
    public class ActivateUserRequest
    {
        public string Email { get; set; }
    }
}