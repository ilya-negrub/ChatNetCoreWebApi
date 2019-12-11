using System;
using System.Collections.Generic;
using System.Text;

namespace ApiEntity
{
    public class ChatMessage : IMessage
    {
        public Guid From { get; set; }

        public Guid To { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }
    }
}
