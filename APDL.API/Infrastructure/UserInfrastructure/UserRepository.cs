
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

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser != null)
            {
                _context.Users.Attach(user);
                _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> SetPrivacyPolicyNotificationForAllUsersAsync()
        {
            return await _context.Database.ExecuteSqlRawAsync(
                "UPDATE \"Users\" SET \"PrivacyPolicyNotificationPending\" = true"
            );
        }


    }
}
