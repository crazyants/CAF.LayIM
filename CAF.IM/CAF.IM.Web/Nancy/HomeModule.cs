using CAF.IM.Core;
using CAF.IM.Core.Data;
using CAF.IM.Core.Domain;
using CAF.IM.Services;
using CAF.IM.Services.Nancy;
using CAF.IM.Services.ViewModels;
using CAF.IM.Core.Infrastructure;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using CAF.IM.Services.FormsAuthentication;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;
using System.Globalization;
using System.Collections;

namespace CAF.IM.Web.Nancy
{
    public class HomeModule : ChatModule
    {
        private static readonly Regex clientSideResourceRegex = new Regex("^(Client_.*|Chat_.*|Content_.*|Create_.*|LoadingMessage|Room.*)$");


        public HomeModule(ApplicationSettings settings,
                             IMembershipService membershipService,
                                IUserAuthenticator authenticator,
                              IRepository<ChatUser> repository,
                             IChatConfiguration configuration
                             )
        {
            Get["/"] = _ =>
            {
                if (IsAuthenticated)
                {
                    var viewModel = new SettingsViewModel
                    {
                        GoogleAnalytics = settings.GoogleAnalytics,
                        AppInsights = settings.AppInsights,
                        Sha = configuration.DeploymentSha,
                        Branch = configuration.DeploymentBranch,
                        Time = configuration.DeploymentTime,
                        DebugMode = (bool)Context.Items["_debugMode"],
                        Version = Constants.ChatVersion,
                        IsAdmin = Principal.HasClaim(ChatClaimTypes.Admin),
                        ClientLanguageResources = BuildClientResources(),
                        MaxMessageLength = settings.MaxMessageLength,
                        AllowRoomCreation = settings.AllowRoomCreation || Principal.HasClaim(ChatClaimTypes.Admin)
                    };

                    return View["index", viewModel];
                }

                if (Principal != null && Principal.HasPartialIdentity())
                {
                    // If the user is partially authenticated then take them to the register page
                    return Response.AsRedirect("~/account/register");
                }

                return HttpStatusCode.Unauthorized;
            };
        }
        private static string BuildClientResources()
        {
            var resourceSet = LanguageResources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            var invariantResourceSet = LanguageResources.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);

            var resourcesToEmbed = new Dictionary<string, string>();
            foreach (DictionaryEntry invariantResource in invariantResourceSet)
            {
                var resourceKey = (string)invariantResource.Key;

                if (clientSideResourceRegex.IsMatch(resourceKey))
                {
                    try
                    {
                        resourcesToEmbed.Add(resourceKey, resourceSet.GetString(resourceKey));
                    }
                    catch (InvalidOperationException)
                    {
                        // The resource specified by name is not a String.
                    }
                }
            }

            return String.Join(",", resourcesToEmbed.Select(e => string.Format("'{0}': {1}", e.Key, Encoder.JavaScriptEncode(e.Value))));
        }
    }
}