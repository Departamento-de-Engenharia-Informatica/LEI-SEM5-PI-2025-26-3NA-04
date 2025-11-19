using System.Threading.Tasks;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.UserAggregate
{
    public interface IUserRepository : IRepository<User, UserId>
    {
        Task<User> GetByEmailAsync(string email);
    }
}