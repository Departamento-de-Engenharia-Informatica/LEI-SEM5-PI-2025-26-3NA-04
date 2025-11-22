
using System.Threading.Tasks;
using APDL.API.Domain.Storage;
using APDL.API.Domain.UserAggregate;
using APDL.API.Infrastructure.Shared;
using APDL.API.Infrastructure.UserInfrastructure;
using Microsoft.EntityFrameworkCore;
using System;

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

        
        public async Task<User> GetUserByActivationTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ActivationToken == token);
        }


        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> ActivateUserAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return null;

            user.IsActive = true;
            user.ActivationToken = null;
            user.ActivationTokenExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

    }
}
