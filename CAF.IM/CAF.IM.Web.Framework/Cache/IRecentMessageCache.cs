using CAF.IM.Core.Domain;
using CAF.IM.Web.Framework.ViewModels;
using System.Collections.Generic;

namespace CAF.IM.Web.Framework.Cache
{
    public interface IRecentMessageCache
    {
        void Add(ChatMessage message);

        void Add(string room, ICollection<MessageViewModel> messages);

        ICollection<MessageViewModel> GetRecentMessages(string roomName);
    }
}
