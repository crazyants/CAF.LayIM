using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.Hosting;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.Conventions;
using CAF.IM.Core.Data;

namespace CAF.IM.Data
{
    public class MongoDBDataProvider : IDataProvider
    {
        

        #region Methods


        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitDatabase()
        {
            DataSettingsHelper.InitConnectionString();
        }

        /// <summary>
        /// Set database initializer
        /// </summary>
        public virtual void SetDatabaseInitializer()
        {
            //BsonSerializer.RegisterSerializer<decimal>(new DecimalSerializer().WithRepresentation(BsonType.Double));
            BsonSerializer.RegisterSerializer(typeof(DateTime),
             new DateTimeSerializer(DateTimeKind.Utc));
        }

        #endregion
    }
}
