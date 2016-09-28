using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAF.IM.Services.FormsAuthentication
{
    public class AuthenticationResult
    {
        public string ProviderName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}