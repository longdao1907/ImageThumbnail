using AuthAPI.Core.Domain.Entities;

namespace AuthAPI.Core.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
    }
}
