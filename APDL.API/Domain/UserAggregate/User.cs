using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.UserAggregate
{
    public class User : Entity<UserId>, IAggregateRoot
    {
        public Guid Id { get; set; } 
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;
        public Boolean IsActivated { get; set; } = false;     

    }
}