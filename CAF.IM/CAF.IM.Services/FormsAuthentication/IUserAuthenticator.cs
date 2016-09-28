using System.Collections.Generic;
using System.Security.Claims;

namespace CAF.IM.Services.FormsAuthentication
{
    public interface IUserAuthenticator
    {
        bool TryAuthenticateUser(string username, string password, out IList<Claim> claims);
    }
}