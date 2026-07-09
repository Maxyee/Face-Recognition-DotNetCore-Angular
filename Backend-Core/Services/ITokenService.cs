using Face_Recognition_Demo.Models;

namespace Face_Recognition_Demo.Services
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, string authMethod);
    }
}

