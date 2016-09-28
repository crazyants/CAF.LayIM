using System;
using System.Configuration;

namespace CAF.IM.Core
{
    public class ChatConfiguration : IChatConfiguration
    {
        public bool RequireHttps
        {
            get
            {
                string requireHttpsValue = ConfigurationManager.AppSettings["Chat:requireHttps"];
                bool requireHttps;
                if (Boolean.TryParse(requireHttpsValue, out requireHttps))
                {
                    return requireHttps;
                }
                return false;
            }
        }

        public bool MigrateDatabase
        {
            get
            {
                string migrateDatabaseValue = ConfigurationManager.AppSettings["Chat:migrateDatabase"];
                bool migrateDatabase;
                if (Boolean.TryParse(migrateDatabaseValue, out migrateDatabase))
                {
                    return migrateDatabase;
                }
                return false;
            }
        }

        public string DeploymentSha
        {
            get
            {
                return ConfigurationManager.AppSettings["Chat:releaseSha"];
            }
        }

        public string DeploymentBranch
        {
            get
            {
                return ConfigurationManager.AppSettings["Chat:releaseBranch"];
            }
        }

        public string DeploymentTime
        {
            get
            {
                return ConfigurationManager.AppSettings["Chat:releaseTime"];
            }
        }

        public string ServiceBusConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["Chat:serviceBusConnectionString"];
            }
        }

        public string ServiceBusTopicPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["Chat:serviceBusTopicPrefix"];
            }
        }

        public bool ScaleOutSqlServer
        {
            get
            {
                string scaleOutSqlServerValue = ConfigurationManager.AppSettings["Chat:scaleOutSqlServer"];
                bool scaleOutSqlServer;
                if (Boolean.TryParse(scaleOutSqlServerValue, out scaleOutSqlServer))
                {
                    return scaleOutSqlServer;
                }
                return false;
            }
        }

        public ConnectionStringSettings SqlConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Chat"];
            }
        }
    }
}