using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Owin.Security.DataProtection;
using CAF.IM.Services;

namespace CAF.IM.Web.Infrastructure
{
    public class ChatDataProtection : IDataProtector
    {
        private readonly ICryptoService _cryptoService;
        public ChatDataProtection(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        public byte[] Protect(byte[] userData)
        {
            return _cryptoService.Protect(userData);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return _cryptoService.Unprotect(protectedData);
        }
    }
}