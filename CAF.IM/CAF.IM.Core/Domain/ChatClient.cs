using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CAF.IM.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class ChatClient : BaseEntity
    {
        public string Id { get; set; }
        public ChatUser User { get; set; }

        public string UserAgent { get; set; }
        public string Name { get; set; }

        public DateTimeOffset LastActivity { get; set; }
        public DateTimeOffset LastClientActivity { get; set; }

        public string UserKey { get; set; }
    }
}