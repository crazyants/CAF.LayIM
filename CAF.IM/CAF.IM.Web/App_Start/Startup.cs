using CAF.IM.Web;
using Microsoft.Owin;
using Owin;
using System.IO;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Nancy.Owin;
using Newtonsoft.Json.Serialization;
using Ninject;
using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using CAF.IM.Core;
using CAF.IM.Core.Infrastructure;
using CAF.IM.Services.Hubs;
using CAF.IM.Services;
using CAF.IM.ServicesMiddleware;
using CAF.IM.Services.Nancy;
using CAF.IM.Core.Data;

[assembly: OwinStartup(typeof(Startup), "Configuration")]
namespace CAF.IM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // So that squishit works
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            var configuration = new ChatConfiguration();

          
            var kernel = SetupNinject(configuration);
        
            if (configuration.MigrateDatabase&& !DataSettingsHelper.DatabaseIsInstalled())
            {
                // Perform the required migrations
                var migrations = new StartupDoMigrations(configuration, kernel);
                migrations.DoMigrations();
            }
            app.Use(typeof(DetectSchemeHandler));

            if (configuration.RequireHttps)
            {
                app.Use(typeof(RequireHttpsHandler));
            }

            app.UseErrorPage();

            SetupAuth(app, kernel);
            SetupSignalR(configuration, kernel, app);
            SetupWebApi(kernel, app);
            SetupMiddleware(kernel, app);        
            SetupNancy(kernel, app);
            SetupErrorHandling();
         
        }

        private static void SetupAuth(IAppBuilder app, IKernel kernel)
        {
            var ticketDataFormat = new TicketDataFormat(kernel.Get<IDataProtector>());

            var type = typeof(CookieAuthenticationOptions)
                .Assembly.GetType("Microsoft.Owin.Security.Cookies.CookieAuthenticationMiddleware");

            app.Use(type, app, new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/account/login"),
                LogoutPath = new PathString("/account/logout"),
                CookieHttpOnly = true,
                AuthenticationType = Constants.ChatAuthType,
                CookieName = "Chat.id",
                ExpireTimeSpan = TimeSpan.FromDays(30),
                TicketDataFormat = ticketDataFormat,
                Provider = kernel.Get<ICookieAuthenticationProvider>()
            });

            app.Use(typeof(CustomAuthHandler));

            app.Use(typeof(WindowsPrincipalHandler));
        }

        private static void SetupSignalR(IChatConfiguration ChatConfig, IKernel kernel, IAppBuilder app)
        {
            var resolver = new NinjectSignalRDependencyResolver(kernel);
            var connectionManager = resolver.Resolve<IConnectionManager>();
            var heartbeat = resolver.Resolve<ITransportHeartbeat>();
            var hubPipeline = resolver.Resolve<IHubPipeline>();
            var configuration = resolver.Resolve<IConfigurationManager>();

            // Enable service bus scale out
            if (!String.IsNullOrEmpty(ChatConfig.ServiceBusConnectionString) &&
                !String.IsNullOrEmpty(ChatConfig.ServiceBusTopicPrefix))
            {
                var sbConfig = new ServiceBusScaleoutConfiguration(ChatConfig.ServiceBusConnectionString,
                                                                   ChatConfig.ServiceBusTopicPrefix)
                {
                    TopicCount = 5
                };

                resolver.UseServiceBus(sbConfig);
            }

            if (ChatConfig.ScaleOutSqlServer)
            {
                resolver.UseSqlServer(ChatConfig.SqlConnectionString.ConnectionString);
            }

            kernel.Bind<IConnectionManager>()
                  .ToConstant(connectionManager);

            // We need to extend this since the inital connect might take a while
            configuration.TransportConnectTimeout = TimeSpan.FromSeconds(30);

            var config = new HubConfiguration
            {
                Resolver = resolver
            };

            hubPipeline.AddModule(kernel.Get<LoggingHubPipelineModule>());

            app.MapSignalR(config);
            //心跳定时任务
            var monitor = new PresenceMonitor(kernel, connectionManager, heartbeat);
            monitor.Start();
        }

        private static void SetupWebApi(IKernel kernel, IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var jsonFormatter = new JsonMediaTypeFormatter();

            config.Formatters.Clear();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Add(jsonFormatter);
            config.DependencyResolver = new NinjectWebApiDependencyResolver(kernel);

            config.Routes.MapHttpRoute(
                name: "MessagesV1",
                routeTemplate: "api/v1/{controller}/{room}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api",
                defaults: new { controller = "ApiFrontPage" }
            );

            app.UseWebApi(config);
        }

        private static void SetupMiddleware(IKernel kernel, IAppBuilder app)
        {
            app.UseStaticFiles();
        }

        private static void SetupNancy(IKernel kernel, IAppBuilder app)
        {
            var bootstrapper = new ChatNinjectNancyBootstrapper(kernel);
            app.UseNancy(new NancyOptions { Bootstrapper = bootstrapper });
        }


    }
}
