using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.UserAggregate;
using System.Linq;
using System;

namespace APDL.API.Domain.UserAggregate
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;

        public UserService(IUnitOfWork unitOfWork, IUserRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _repo.GetByEmailAsync(email);
        }
        
        public async Task<User?> GetUserByActivationTokenAsync(string token)
        {
            var users = await _repo.GetAllAsync(); // Traz todos os Users
            return users.FirstOrDefault(u =>
                u.ActivationToken == token &&
                u.ActivationTokenExpiry.HasValue &&
                u.ActivationTokenExpiry > DateTime.UtcNow);
        }
  
        public async Task ActivateUserAsync(Guid userId)
        {
            var user = await _repo.GetByIdAsync(new UserId(userId));
            if (user == null) throw new Exception("User not found");

            user.IsActive = true;
            user.ActivationToken = null;
            user.ActivationTokenExpiry = null;

            await _repo.UpdateAsync(user);
            await _unitOfWork.CommitAsync();
        }     

    }
}