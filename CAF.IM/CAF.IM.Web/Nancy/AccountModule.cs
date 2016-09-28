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

namespace CAF.IM.Web.Nancy
{
    public class AccountModule : ChatModule
    {
        public AccountModule(ApplicationSettings applicationSettings,
                           IMembershipService membershipService,
                              IUserAuthenticator authenticator,
                            IRepository<ChatUser> repository
                           //IChatNotificationService notificationService
                           )
          : base("/account")
        {
            Get["/"] = _ =>
            {
                if (!IsAuthenticated)
                {
                    return HttpStatusCode.Forbidden;
                }

                ChatUser user = repository.GetById(Principal.GetUserId());

                return GetProfileView(user);
            };

            Get["/login"] = _ =>
            {
                if (IsAuthenticated)
                {
                    return this.AsRedirectQueryStringOrDefault("~/");
                }

                return View["login", GetLoginViewModel(applicationSettings, repository)];
            };

            Post["/login"] = param =>
            {
                if (!HasValidCsrfTokenOrSecHeader)
                {
                    return HttpStatusCode.Forbidden;
                }

                if (IsAuthenticated)
                {
                    return this.AsRedirectQueryStringOrDefault("~/");
                }

                string username = Request.Form.username;
                string password = Request.Form.password;

                if (String.IsNullOrEmpty(username))
                {
                    this.AddValidationError("username", LanguageResources.Authentication_NameRequired);
                }

                if (String.IsNullOrEmpty(password))
                {
                    this.AddValidationError("password", LanguageResources.Authentication_PassRequired);
                }

                try
                {
                    if (ModelValidationResult.IsValid)
                    {
                        IList<Claim> claims;
                        if (authenticator.TryAuthenticateUser(username, password, out claims))
                        {
                            return this.SignIn(claims);
                        }
                    }
                }
                catch
                {
                    // Swallow the exception    
                }

                this.AddValidationError("_FORM", LanguageResources.Authentication_GenericFailure);

                return View["login", GetLoginViewModel(applicationSettings, repository)];
            };

            Post["/logout"] = _ =>
            {
                if (!IsAuthenticated)
                {
                    return HttpStatusCode.Forbidden;
                }

                var response = Response.AsJson(new { success = true });

                this.SignOut();

                return response;
            };

            Get["/register"] = _ =>
            {
                if (IsAuthenticated)
                {
                    return this.AsRedirectQueryStringOrDefault("~/");
                }

                bool requirePassword = !Principal.Identity.IsAuthenticated;

                if (requirePassword &&
                    !applicationSettings.AllowUserRegistration)
                {
                    return HttpStatusCode.NotFound;
                }

                ViewBag.requirePassword = requirePassword;

                return View["register"];
            };

            Post["/create"] = _ =>
            {
                if (!HasValidCsrfTokenOrSecHeader)
                {
                    return HttpStatusCode.Forbidden;
                }

                bool requirePassword = !Principal.Identity.IsAuthenticated;

                if (requirePassword &&
                    !applicationSettings.AllowUserRegistration)
                {
                    return HttpStatusCode.NotFound;
                }

                if (IsAuthenticated)
                {
                    return this.AsRedirectQueryStringOrDefault("~/");
                }

                ViewBag.requirePassword = requirePassword;

                string username = Request.Form.username;
                string email = Request.Form.email;
                string password = Request.Form.password;
                string confirmPassword = Request.Form.confirmPassword;

                if (String.IsNullOrEmpty(username))
                {
                    this.AddValidationError("username", LanguageResources.Authentication_NameRequired);
                }

                if (String.IsNullOrEmpty(email))
                {
                    this.AddValidationError("email", LanguageResources.Authentication_EmailRequired);
                }

                try
                {
                    if (requirePassword)
                    {
                        ValidatePassword(password, confirmPassword);
                    }

                    if (ModelValidationResult.IsValid)
                    {
                        if (requirePassword)
                        {
                            ChatUser user = membershipService.AddUser(username, email, password);

                            return this.SignIn(user);
                        }
                        else
                        {
                            // Add the required claims to this identity
                            var identity = Principal.Identity as ClaimsIdentity;

                            if (!Principal.HasClaim(ClaimTypes.Name))
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Name, username));
                            }

                            if (!Principal.HasClaim(ClaimTypes.Email))
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Email, email));
                            }

                            return this.SignIn(Principal.Claims);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.AddValidationError("_FORM", ex.Message);
                }

                return View["register"];
            };
        }

        private void ValidatePassword(string password, string confirmPassword)
        {
            if (String.IsNullOrEmpty(password))
            {
                this.AddValidationError("password", LanguageResources.Authentication_PassRequired);
            }

            if (!String.Equals(password, confirmPassword))
            {
                this.AddValidationError("confirmPassword", LanguageResources.Authentication_PassNonMatching);
            }
        }

        private void ValidateUsername(string username, string confirmUsername)
        {
            if (String.IsNullOrEmpty(username))
            {
                this.AddValidationError("username", LanguageResources.Authentication_NameRequired);
            }

            if (!String.Equals(username, confirmUsername))
            {
                this.AddValidationError("confirmUsername", LanguageResources.Authentication_NameNonMatching);
            }
        }

        private dynamic GetProfileView(ChatUser user)
        {
            return View["index", new ProfilePageViewModel(user)];
        }

        private LoginViewModel GetLoginViewModel(ApplicationSettings applicationSettings,
                                                 IRepository<ChatUser> repository)
        {
            ChatUser user = null;

            if (IsAuthenticated)
            {
                user = repository.GetById(Principal.GetUserId());
            }

            var viewModel = new LoginViewModel(applicationSettings, user != null ? user.Identities : null);
            return viewModel;
        }
    }
}