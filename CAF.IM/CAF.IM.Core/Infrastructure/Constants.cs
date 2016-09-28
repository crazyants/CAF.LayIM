using System;

namespace CAF.IM.Core.Infrastructure
{
    public static class Constants
    {
        public static readonly string AuthResultCookie = "Chat.authResult";
        public static readonly Version ChatVersion = typeof(Constants).Assembly.GetName().Version;
        public static readonly string ChatAuthType = "Chat";
    }

    public static class ChatClaimTypes
    {
        public const string Identifier = "urn:Chat:id";
        public const string Admin = "urn:Chat:admin";
        public const string PartialIdentity = "urn:Chat:partialid";
    }

    public static class AcsClaimTypes
    {
        public static readonly string IdentityProvider = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/IdentityProvider";
    }

    public static class ContentTypes
    {
        public const string Html = "text/html";
        public const string Text = "text/plain";
    }
}