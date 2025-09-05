using AuthAPI.Core.Domain.Entities;

namespace AuthAPI.Core.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
