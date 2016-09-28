using CAF.IM.Core.Domain;
using CAF.IM.Core.Infrastructure;
using System.Collections.Generic;
using System.Security.Claims;

namespace CAF.IM.Services.FormsAuthentication
{
    /// <summary>
    /// The default authenticator uses the username/password system in Chat
    /// </summary>
    public class DefaultUserAuthenticator : IUserAuthenticator
    {
        private readonly IMembershipService _service;

        public DefaultUserAuthenticator(IMembershipService service)
        {
            _service = service;
        }

        public bool TryAuthenticateUser(string username, string password, out IList<Claim> claims)
        {
            claims = new List<Claim>();

            ChatUser user;
            if (_service.TryAuthenticateUser(username, password, out user))
            {
                if (user.IsBanned)
                {
                    return false;
                }

                claims.Add(new Claim(ChatClaimTypes.Identifier, user.Id));

                // Add the admin claim if the user is an Administrator
                if (user.IsAdmin)
                {
                    claims.Add(new Claim(ChatClaimTypes.Admin, "true"));
                }

                return true;
            }

            return false;
        }
    }
}