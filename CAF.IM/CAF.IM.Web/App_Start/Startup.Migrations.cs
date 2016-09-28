using CAF.IM.Core;
using CAF.IM.Core.Data;
using CAF.IM.Core.Domain;
using CAF.IM.Data;
using MongoDB.Driver;
using Ninject;
using System;


namespace CAF.IM.Web
{
    public partial class StartupDoMigrations
    {
        private const string SqlClient = "System.Data.SqlClient";
        private readonly IKernel _kernel;
        private readonly IChatConfiguration _config;
        public StartupDoMigrations(IChatConfiguration config, IKernel kernel)
        {
            _kernel = kernel;
            _config = config;
        }

        public void DoMigrations()
        {
            //if (String.IsNullOrEmpty(_config.SqlConnectionString.ProviderName) ||
            //    !_config.SqlConnectionString.ProviderName.Equals(SqlClient, StringComparison.OrdinalIgnoreCase))
            //{
            //    return;
            //}
            var settingsManager = new DataSettingsManager();
            try
            {
                //save settings
                //  var   connectionString = "mongodb://" + userNameandPassword + model.MongoDBServerName + "/" + model.MongoDBDatabaseName;
                var settings = new DataSettings
                {
                    DataProvider = "mongodb",
                    DataConnectionString = " mongodb://localhost:27017/chatdb"
                };
                settingsManager.SaveSettings(settings);

                var dataProviderInstance = new MongoDBDataProviderManager(settingsManager.LoadSettings()).LoadDataProvider();
                dataProviderInstance.InitDatabase();
                //now resolve installation service
                var mongoDBDataProviderManager = new MongoDBDataProviderManager(settingsManager.LoadSettings());
                var dataProviderInstall = mongoDBDataProviderManager.LoadDataProvider();
                CreateIndexes();
                InstallVersion();
            }
            catch (Exception exception)
            {

            }
        }

        private void CreateIndexes()
        {
            var _repositoryChatClient = _kernel.Get<IRepository<ChatClient>>();
            _repositoryChatClient.Collection.Indexes.CreateOneAsync(Builders<ChatClient>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Name = "Key", Unique = true });
            var _repositoryAttachment = _kernel.Get<IRepository<Attachment>>();
            _repositoryAttachment.Collection.Indexes.CreateOneAsync(Builders<Attachment>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Name = "Key", Unique = true });
            var _repositoryChatMessage = _kernel.Get<IRepository<ChatMessage>>();
            _repositoryChatMessage.Collection.Indexes.CreateOneAsync(Builders<ChatMessage>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Name = "Key", Unique = true });
            var _repositoryChatRoom = _kernel.Get<IRepository<ChatRoom>>();
            _repositoryChatRoom.Collection.Indexes.CreateOneAsync(Builders<ChatRoom>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Name = "Key", Unique = true });
            var _repositoryChatUser = _kernel.Get<IRepository<ChatUser>>();
            _repositoryChatUser.Collection.Indexes.CreateOneAsync(Builders<ChatUser>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Name = "Key", Unique = true });
            var _repositoryChatUserIdentity = _kernel.Get<IRepository<ChatUserIdentity>>();
            _repositoryChatUserIdentity.Collection.Indexes.CreateOneAsync(Builders<ChatUserIdentity>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Name = "Key", Unique = true });
            var _repositoryNotification = _kernel.Get<IRepository<Notification>>();
            _repositoryNotification.Collection.Indexes.CreateOneAsync(Builders<Notification>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Name = "Key", Unique = true });
            var _repositorySettings = _kernel.Get<IRepository<Settings>>();
            _repositorySettings.Collection.Indexes.CreateOneAsync(Builders<Settings>.IndexKeys.Ascending(x => x.Key), new CreateIndexOptions() { Name = "Key", Unique = true });

        }

        protected virtual void InstallVersion()
        {
            var _repositorySettings = _kernel.Get<IRepository<Settings>>();
            var version = new Settings
            {
                  RawSettings=""
            };
            _repositorySettings.Insert(version);
        }
    }
}