using System.Collections.Generic;

namespace DAMA.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(int userId, string email, IEnumerable<string> roles);
    }

}
