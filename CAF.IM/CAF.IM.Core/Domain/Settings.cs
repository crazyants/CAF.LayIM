using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CAF.IM.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class Settings : BaseEntity
    {

        public string RawSettings { get; set; }
    }
}