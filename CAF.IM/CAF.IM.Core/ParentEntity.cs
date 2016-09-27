using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CAF.IM.Core
{
    public abstract class ParentEntity
    {
        public ParentEntity()
        {
            _key = ObjectId.GenerateNewId().ToString();
        }

        public string Key
        {
            get { return _key; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _key = ObjectId.GenerateNewId().ToString();
                else
                    _key = value;
            }
        }

        private string _key;

    }
}
