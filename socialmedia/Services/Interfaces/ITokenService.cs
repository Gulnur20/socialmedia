using socialmedia.Models;

namespace socialmedia.Services.Interfaces
{
    public interface ITokenService
    {
       string GenerateToken(Users user);
        
    }
}
