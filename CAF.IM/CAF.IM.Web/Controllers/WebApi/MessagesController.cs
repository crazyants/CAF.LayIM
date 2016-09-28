using CAF.IM.Core;
using CAF.IM.Core.Data;
using CAF.IM.Core.Domain;
using CAF.IM.Web.Infrastructure;
using CAF.IM.Web.WebApi.Model;
using CAF.IM.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CAF.IM.Web.WebApi
{
    public class MessagesController : ApiController
    {
        const string FilenameDateFormat = "yyyy-MM-dd.HHmmsszz";
        private IRepository<ChatUser> _repository;
        private IRepository<ChatRoom> _repositoryChatRoom;
        private IRepository<ChatMessage> _repositoryChatMessage;

        public MessagesController(IRepository<ChatUser> repository,
            IRepository<ChatRoom> repositoryChatRoom,
            IRepository<ChatMessage> repositoryChatMessage)
        {
            _repository = repository;
            _repositoryChatRoom = repositoryChatRoom;
            _repositoryChatMessage = repositoryChatMessage;
        }

        public HttpResponseMessage GetAllMessages(string room, string range)
        {
            if (String.IsNullOrWhiteSpace(range))
            {
                range = "last-hour";
            }

            var end = DateTime.Now;
            DateTime start;

            switch (range)
            {
                case "last-hour":
                    start = end.AddHours(-1);
                    break;
                case "last-day":
                    start = end.AddDays(-1);
                    break;
                case "last-week":
                    start = end.AddDays(-7);
                    break;
                case "last-month":
                    start = end.AddDays(-30);
                    break;
                case "all":
                    start = DateTime.MinValue;
                    break;
                default:
                    return Request.CreateChatErrorMessage(HttpStatusCode.BadRequest, "range value not recognized");
            }

            var filenamePrefix = room + ".";

            if (start != DateTime.MinValue)
            {
                filenamePrefix += start.ToString(FilenameDateFormat, CultureInfo.InvariantCulture) + ".";
            }

            filenamePrefix += end.ToString(FilenameDateFormat, CultureInfo.InvariantCulture);


            ChatRoom chatRoom = null;

            try
            {
                chatRoom = _repositoryChatRoom.VerifyRoom(room, mustBeOpen: false);
            }
            catch (Exception ex)
            {
                return Request.CreateChatErrorMessage(HttpStatusCode.NotFound, ex.Message, filenamePrefix);
            }

            if (chatRoom.Private)
            {
                // TODO: Allow viewing messages using auth token
                return Request.CreateChatErrorMessage(HttpStatusCode.NotFound, String.Format(LanguageResources.RoomNotFound, chatRoom.Name), filenamePrefix);
            }

            var messages = _repositoryChatMessage.Table.Where(m => m.RoomKey == chatRoom.Key)
                .Where(msg => msg.When <= end && msg.When >= start)
                .OrderBy(msg => msg.When)
                .Select(msg => new MessageApiModel
                {
                    Content = msg.Content,
                    Username = msg.User.Name,
                    When = msg.When,
                    HtmlEncoded = msg.HtmlEncoded,
                });


            return Request.CreateChatSuccessMessage(HttpStatusCode.OK, messages, filenamePrefix);
        }
    }
}