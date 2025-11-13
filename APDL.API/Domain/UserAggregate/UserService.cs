using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.UserAggregate;

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
    }
}