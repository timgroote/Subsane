using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SubsonicAPI
{
    public class ChatMessage
    {
        public String User { get; private set; }
        public String Message { get; private set;}
        public DateTime? Date { get; private set; }

        public ChatMessage(string user, string message, DateTime date)
        {
            User = user;
            Message = message;
            Date = date;
        }

        public ChatMessage(string user, string message)
        {
            User = user;
            Message = message;
            Date = null;
        }

        public override String ToString()
        {
            return String.Format("{0} {1} : {2}", Date.HasValue ? Date.Value.ToString("T") : "------", User, Message);
        }

        public static ChatMessage FromXml(XmlElement ch)
        {
            return new ChatMessage(
                ch.Attributes["username"].Value,
                ch.Attributes["message"].Value,
                DateTimeToolkit.ConvertFromUnixTimestamp(double.Parse(ch.Attributes["time"].Value)/10)    //todo : timestamps are wrong. nobody cares to tell me what i'm looking @
            );
        }
    }
}
