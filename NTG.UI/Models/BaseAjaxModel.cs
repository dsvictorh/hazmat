using System.Collections.Generic;

namespace NTG.UI.Models
{
    public class BaseAjaxModel
    {
        public readonly Dictionary<string, List<Message>> Messages;

        public BaseAjaxModel()
        {
            Messages = new Dictionary<string, List<Message>>();
        }

        public void AddMessage(string key, Message message) {
            List<Message> messages;
            if (Messages.TryGetValue(key, out messages))
            {
                messages.Add(message);
            }
            else
            {
                Messages.Add(key, new List<Message>(){ message });
            }
        }
    }
}