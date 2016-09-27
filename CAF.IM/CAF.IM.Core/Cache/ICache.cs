using System;

namespace CAF.IM.Core.Cache
{
    public interface ICache
    {
        object Get(string key);
        void Set(string key, object value, TimeSpan expiresIn);
        void Remove(string key);
    }
}