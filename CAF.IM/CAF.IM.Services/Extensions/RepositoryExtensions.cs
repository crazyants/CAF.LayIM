using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.SignalR;
using CAF.IM.Core.Domain;
using CAF.IM.Core.Data;
using CAF.IM.Core.Infrastructure;
using CAF.IM.Core;
using CAF.IM.Core.Cache;

namespace CAF.IM.Services
{
    public static class RepositoryExtensions
    {
        public static ChatUser GetLoggedInUser(this IRepository<ChatUser> repository, ClaimsPrincipal principal)
        {
            return repository.GetById(principal.GetUserId());
        }

        public static ChatUser GetUser(this IRepository<ChatUserIdentity> repository, ClaimsPrincipal principal)
        {
            string identity = principal.GetClaimValue(ClaimTypes.NameIdentifier);
            string providerName = principal.GetIdentityProvider();


            ChatUserIdentity chatUserIdentity = repository.Table.FirstOrDefault(u => u.Identity == identity && u.ProviderName == providerName);
            if (chatUserIdentity != null)
            {
                return chatUserIdentity.User;
            }
            return null;

        }

        public static IQueryable<ChatUser> Online(this IQueryable<ChatUser> source)
        {
            return source.Where(u => u.Status != (int)UserStatus.Offline);
        }

        public static IEnumerable<ChatUser> Online(this IEnumerable<ChatUser> source)
        {
            return source.Where(u => u.Status != (int)UserStatus.Offline);
        }

        public static IEnumerable<ChatRoom> Allowed(this IEnumerable<ChatRoom> rooms, string userId)
        {
            return from r in rooms
                   where !r.Private ||
                         r.Private && r.AllowedUsers.Any(u => u.Id == userId)
                   select r;
        }

        public static ChatRoom VerifyUserRoom(this IRepository<ChatRoom> repository, IRepository<ChatUser> urepository, ICache cache, ChatUser user, string roomName)
        {
            if (String.IsNullOrEmpty(roomName))
            {
                throw new HubException(LanguageResources.RoomJoinMessage);
            }

            roomName = ChatService.NormalizeRoomName(roomName);
            ChatRoom room = repository.Table.FirstOrDefault(r => r.Name == roomName);

            if (room == null)
            {
                throw new HubException(String.Format(LanguageResources.RoomMemberButNotExists, roomName));
            }

            if (!urepository.IsUserInRoom(repository, cache, user, room))
            {
                throw new HubException(String.Format(LanguageResources.RoomNotMember, roomName));
            }

            return room;
        }

        public static bool IsUserInRoom(this IRepository<ChatUser> repository, IRepository<ChatRoom> crrepository, ICache cache, ChatUser user, ChatRoom room)
        {
            bool? cached = cache.IsUserInRoom(user, room);

            if (cached == null)
            {
                var query = from ur in repository.Table
                            from cr in crrepository.Table
                            where cr.Key == room.Key
                            select new
                            {
                                ur.Name
                            };

                cached = query.FirstOrDefault() != null;

                cache.SetUserInRoom(user, room, cached.Value);
            }

            return cached.Value;
        }

        public static ChatUser VerifyUserId(this IRepository<ChatUser> repository, string userId)
        {
            ChatUser user = repository.GetById(userId);

            if (user == null)
            {
                // The user isn't logged in 
                throw new HubException(LanguageResources.Authentication_NotLoggedIn);
            }

            return user;
        }

        public static ChatRoom VerifyRoom(this IRepository<ChatRoom> repository, string roomName, bool mustBeOpen = true)
        {
            if (String.IsNullOrWhiteSpace(roomName))
            {
                throw new HubException(LanguageResources.RoomNameCannotBeBlank);
            }

            roomName = ChatService.NormalizeRoomName(roomName);

            var room = repository.Table.FirstOrDefault(r => r.Name == roomName);

            if (room == null)
            {
                throw new HubException(String.Format(LanguageResources.RoomNotFound, roomName));
            }

            if (room.Closed && mustBeOpen)
            {
                throw new HubException(String.Format(LanguageResources.RoomClosed, roomName));
            }

            return room;
        }

        public static ChatUser VerifyUser(this IRepository<ChatUser> repository, string userName)
        {
            userName = MembershipService.NormalizeUserName(userName);

      
            ChatUser user = repository.Table.FirstOrDefault(r => r.Name == userName);

            if (user == null)
            {
                throw new HubException(String.Format(LanguageResources.UserNotFound, userName));
            }

            return user;
        }

        public static int GetUnreadNotificationsCount(this IRepository<Notification> repository, ChatUser user)
        {
            return repository.Table.Where(n => n.UserKey == user.Key).Unread().Count();
        }

        public static IQueryable<Notification> Unread(this IQueryable<Notification> source)
        {
            return source.Where(n => !n.Read);
        }

        public static IQueryable<Notification> ByRoom(this IQueryable<Notification> source, string roomName)
        {
            return source.Where(n => n.Room.Name == roomName);
        }

        public static IList<string> GetAllowedClientIds(this IRepository<ChatClient> repository, ChatRoom room)
        {
            string[] allowedUserKeys = room.AllowedUsers.Select(u => u.Key).ToArray();
            return repository.Table.Where(c => allowedUserKeys.Contains(c.UserKey)).Select(c => c.Id).ToList();
        }
    }
}