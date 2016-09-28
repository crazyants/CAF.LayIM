using CAF.IM.Core.Domain;
using System.Collections.Generic;


namespace CAF.IM.Services.ViewModels
{
    public class LoginViewModel
    {
        public LoginViewModel(ApplicationSettings settings, IEnumerable<ChatUserIdentity> userIdentities)
        {
    
            AllowUserRegistration = settings.AllowUserRegistration;
            AllowUserResetPassword = settings.AllowUserResetPassword;
            HasEmailSender = !string.IsNullOrWhiteSpace(settings.EmailSender);
        }

        public bool AllowUserRegistration { get; set; }
        public bool AllowUserResetPassword { get; set; }
        public bool HasEmailSender { get; set; }

    }
}