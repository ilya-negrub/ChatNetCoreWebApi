using ApiEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientChat
{
    public class MessageViewModel : BaseViewModel
    {
        public string FromName => ApiClient.Clients.Where(w => w.Id == ChatMessage.From).Select(s => s.Name).FirstOrDefault();

        public ChatMessage ChatMessage { get; private set; }

        public MessageViewModel(ChatMessage chatMessage)
        {
            ChatMessage = chatMessage;
        }

    }
}
