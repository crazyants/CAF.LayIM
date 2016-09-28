using System;

namespace CAF.IM.Web.WebApi.Model
{
    public class MessageApiModel
    {
        public string Content { get; set; }
        public string Username { get; set; }
        public DateTimeOffset When { get; set; }
        public bool HtmlEncoded { get; set; }
    }
}