using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CAF.IM.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class Attachment : BaseEntity
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }

        public string RoomKey { get; set; }
        public string OwnerKey { get; set; }
        public DateTimeOffset When { get; set; }

        public virtual ChatRoom Room { get; set; }
        public virtual ChatUser Owner { get; set; }
    }
}