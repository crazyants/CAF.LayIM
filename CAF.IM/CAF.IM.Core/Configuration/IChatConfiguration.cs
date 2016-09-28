using System.Configuration;

namespace CAF.IM.Core
{
    public interface IChatConfiguration
    {
        bool RequireHttps { get; }
        bool MigrateDatabase { get; }

        string DeploymentSha { get; }
        string DeploymentBranch { get; }
        string DeploymentTime { get; }

        string ServiceBusConnectionString { get; }
        string ServiceBusTopicPrefix { get; }

        ConnectionStringSettings SqlConnectionString { get; }
        bool ScaleOutSqlServer { get; }
    }
}