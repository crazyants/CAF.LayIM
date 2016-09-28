using CAF.IM.Core.Infrastructure;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CAF.IM.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class ChatMessage : BaseEntity
    {
        
        public string Content { get; set; }
        public string Id { get; set; }        
        public virtual ChatRoom Room { get; set; }
        public virtual ChatUser User { get; set; }
        public DateTimeOffset When { get; set; }
        public bool HtmlEncoded { get; set; }
        public int MessageType { get; set; }

        // After content providers run this is updated with the content
        public string HtmlContent { get; set; }

        public string RoomKey { get; set; }
        public string UserKey { get; set; }

        // Notifications
        public string ImageUrl { get; set; }
        public string Source { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }

        public ChatMessage()
        {
            Notifications = new SafeCollection<Notification>();
        }
    }
}