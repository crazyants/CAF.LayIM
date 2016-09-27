using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CAF.IM.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class ChatUserIdentity : BaseEntity
    {
        

        public string UserKey { get; set; }
        public virtual ChatUser User { get; set; }

        public string Email { get; set; }
        public string Identity { get; set; }
        public string ProviderName { get; set; }
    }
}