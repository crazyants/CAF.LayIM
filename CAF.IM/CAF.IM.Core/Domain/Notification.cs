using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CAF.IM.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class Notification : BaseEntity
    {
        
 
        public string UserKey { get; set; }
        public virtual ChatUser User { get; set; }

        public string MessageKey { get; set; }
        public virtual ChatMessage Message { get; set; }

        public string RoomKey { get; set; }
        public virtual ChatRoom Room { get; set; }

        public bool Read { get; set; }
    }
}