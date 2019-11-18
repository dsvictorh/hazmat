using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTG.UI.Models
{
    public enum MessageTypes
    {
        Success,
        Warning,
        Error,
    }
    public class Message
    {
        public const string GLOBAL = "Global";

        public string Text { get; private set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageTypes MessageType { get; private set; }

        public Message(string text, MessageTypes messageType)
        {
            Text = text;
            MessageType = messageType;
        }
    }
}