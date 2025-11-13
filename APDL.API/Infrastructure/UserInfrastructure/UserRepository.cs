
using System.Threading.Tasks;
using APDL.API.Domain.Storage;
using APDL.API.Domain.UserAggregate;
using APDL.API.Infrastructure.Shared;
using APDL.API.Infrastructure.UserInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.UserInfrastructure
{
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository
    {

        private readonly DDDSample1DbContext _context;

        public UserRepository(DDDSample1DbContext context) : base(context.Users)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
