using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Cookies;
using Nancy.Bootstrappers.Ninject;
using Newtonsoft.Json;
using Ninject;
using Microsoft.AspNet.SignalR;
using CAF.IM.Core.Cache;
using CAF.IM.Services;
using CAF.IM.Core.Domain;
using CAF.IM.Core.Infrastructure;
using CAF.IM.Core;
using CAF.IM.Services.Cache;
using CAF.IM.Data;
using CAF.IM.Core.Data;
using CAF.IM.Services.Logger;
using MongoDB.Driver;
using CAF.IM.Services.FormsAuthentication;
using CAF.IM.Web.Infrastructure;

namespace CAF.IM.Web
{
    public partial class Startup
    {
        private static KernelBase SetupNinject(ChatConfiguration configuration)
        {
            var kernel = new StandardKernel(new[] { new FactoryModule() });


            kernel.Bind<IRecentMessageCache>()
                  .To<NoopCache>()
                  .InSingletonScope();

            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();

            kernel.Bind<DataSettings>().ToMethod(context =>
            {
                return dataSettingsManager.LoadSettings();
            });
            kernel.Bind<BaseDataProviderManager>().ToMethod(context =>
            {
                var recentDataSettings = context.Kernel.Get<DataSettings>();
                return new MongoDBDataProviderManager(recentDataSettings);
            });
            kernel.Bind<IDataProvider>().ToMethod(context =>
            {
                var recentBaseDataProviderManager = context.Kernel.Get<BaseDataProviderManager>();
                return recentBaseDataProviderManager.LoadDataProvider();
            });
            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var mongoDBDataProviderManager = new MongoDBDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = mongoDBDataProviderManager.LoadDataProvider();
                kernel.Bind<IMongoClient>().ToMethod(context =>
                {
                    return new MongoClient(dataProviderSettings.DataConnectionString);
                });
            }

            kernel.Bind(typeof(IRepository<>)).To(typeof(MongoDBRepository<>)).InSingletonScope();

            kernel.Bind<IChatService>()
                  .To<ChatService>();

            kernel.Bind<IDataProtector>()
                  .To<ChatDataProtection>();

            kernel.Bind<ICookieAuthenticationProvider>()
                .To<ChatFormsAuthenticationProvider>();

            kernel.Bind<ILogger>()
                .To<RealtimeLogger>();

            //kernel.Bind<IUserIdProvider>()
            //      .To<ChatUserIdProvider>();

            kernel.Bind<IChatConfiguration>()
                .ToConstant(configuration);

            // We're doing this manually since we want the chat repository to be shared
            // between the chat service and the chat hub itself
            kernel.Bind<Chat>()
                  .ToMethod(context =>
                  {
                      //   var resourceProcessor = context.Kernel.Get<ContentProviderProcessor>();
                      var recentMessageCache = context.Kernel.Get<IRecentMessageCache>();
                      //  var repository = context.Kernel.Get<IChatRepository>();
                      var cache = context.Kernel.Get<ICache>();
                      var logger = context.Kernel.Get<ILogger>();
                      var settings = context.Kernel.Get<ApplicationSettings>();
                      IRepository<ChatUser> repository = context.Kernel.Get<IRepository<ChatUser>>();
                      IRepository<ChatUserIdentity> chatUserIdentityRepository = context.Kernel.Get<IRepository<ChatUserIdentity>>();
                      IRepository<Attachment> attachmentRepository = context.Kernel.Get<IRepository<Attachment>>();
                      IRepository<ChatClient> chatClientRepository = context.Kernel.Get<IRepository<ChatClient>>();
                      IRepository<ChatMessage> chatMessagerepository = context.Kernel.Get<IRepository<ChatMessage>>();
                      IRepository<ChatRoom> chatRoomRepository = context.Kernel.Get<IRepository<ChatRoom>>();
                      IRepository<Notification> notificationRepository = context.Kernel.Get<IRepository<Notification>>();
                      IRepository<Settings> settingsRepository = context.Kernel.Get<IRepository<Settings>>();
                      var service = new ChatService(cache, repository, chatUserIdentityRepository, attachmentRepository, chatClientRepository
                  , chatMessagerepository, chatRoomRepository, notificationRepository, settingsRepository);

                      return new Chat(repository, chatUserIdentityRepository, attachmentRepository, chatClientRepository
                  , chatMessagerepository, chatRoomRepository, notificationRepository, settingsRepository,
                                      settings,
                                      service,
                                      cache,
                                      logger
                                      );
                  });

            kernel.Bind<ICryptoService>()
                .To<CryptoService>();

            kernel.Bind<IJavaScriptMinifier>()
                  .To<AjaxMinMinifier>()
                  .InSingletonScope();

            kernel.Bind<IMembershipService>()
                  .To<MembershipService>();

            kernel.Bind<ApplicationSettings>()
                  .ToMethod(context =>
                  {
                      return context.Kernel.Get<ISettingsManager>().Load();
                  });

            kernel.Bind<ISettingsManager>()
                  .To<SettingsManager>();


            kernel.Bind<IUserAuthenticator>()
                  .To<DefaultUserAuthenticator>();

            kernel.Bind<ICache>()
                  .To<DefaultCache>()
                  .InSingletonScope();

            //kernel.Bind<IChatNotificationService>()
            //      .To<ChatNotificationService>();

            kernel.Bind<IKeyProvider>()
                      .To<SettingsKeyProvider>();


            RegisterContentProviders(kernel);

            var serializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            });

            kernel.Bind<JsonSerializer>()
                  .ToConstant(serializer);


            return kernel;
        }

        private static void RegisterContentProviders(IKernel kernel)
        {
            //kernel.Bind<IContentProvider>().To<AudioContentProvider>();
            //kernel.Bind<IContentProvider>().To<BashQDBContentProvider>();
            //kernel.Bind<IContentProvider>().To<BBCContentProvider>();
            //kernel.Bind<IContentProvider>().To<DictionaryContentProvider>();
            //kernel.Bind<IContentProvider>().To<GitHubIssueCommentsContentProvider>();
            //kernel.Bind<IContentProvider>().To<GitHubIssuesContentProvider>();
            //kernel.Bind<IContentProvider>().To<GoogleDocsFormProvider>();
            //kernel.Bind<IContentProvider>().To<GoogleDocsPresentationsContentProvider>();
            //kernel.Bind<IContentProvider>().To<GoogleMapsContentProvider>();
            //kernel.Bind<IContentProvider>().To<ImageContentProvider>();
            //kernel.Bind<IContentProvider>().To<ImgurContentProvider>();
            //kernel.Bind<IContentProvider>().To<NerdDinnerContentProvider>();
            //kernel.Bind<IContentProvider>().To<NugetNuggetContentProvider>();
            //kernel.Bind<IContentProvider>().To<ScreencastContentProvider>();
            //kernel.Bind<IContentProvider>().To<SlideShareContentProvider>();
            //kernel.Bind<IContentProvider>().To<SoundCloudContentProvider>();
            //kernel.Bind<IContentProvider>().To<SpotifyContentProvider>();
            //kernel.Bind<IContentProvider>().To<UserVoiceContentProvider>();
            //kernel.Bind<IContentProvider>().To<UStreamContentProvider>();
            //kernel.Bind<IContentProvider>().To<YouTubeContentProvider>();
            //kernel.Bind<IContentProvider>().To<ConfiguredContentProvider>();
            //kernel.Bind<IContentProvider>().To<XkcdContentProvider>();
            //kernel.Bind<IContentProvider>().To<UrbanDictionaryContentProvider>();
        }
    }
}