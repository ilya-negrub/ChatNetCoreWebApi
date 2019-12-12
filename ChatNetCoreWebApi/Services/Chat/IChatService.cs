using ApiEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ChatNetCoreWebApi.Services.Chat
{
    public interface IChatService
    {
        IEnumerable<Client> Clients { get; }
        void SendMessages(IEnumerable<ChatMessage> messages);
        IActionResult Subscribe(HttpContext context, string clientName);

    }
}
