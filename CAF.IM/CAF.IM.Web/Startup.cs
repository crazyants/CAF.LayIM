using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CAF.IM.Web.Startup))]
namespace CAF.IM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
