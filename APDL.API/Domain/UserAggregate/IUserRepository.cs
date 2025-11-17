using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using System;

namespace APDL.API.Domain.UserAggregate
{
    public interface IUserRepository : IRepository<User, UserId>
    {
        Task<User> GetByEmailAsync(string email);

        Task<User> GetUserByActivationTokenAsync(string token);

        Task UpdateAsync(User user);

        Task<User> ActivateUserAsync(Guid id);
    }
}