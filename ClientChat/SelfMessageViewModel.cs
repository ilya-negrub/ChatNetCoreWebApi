using ApiEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientChat
{
    public class SelfMessageViewModel : MessageViewModel
    {
        public SelfMessageViewModel(ChatMessage chatMessage) : base(chatMessage)
        {
        }
    }
}
