using ApiEntity;
using ChatNetCoreWebApi.Services.Chat;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatNetCoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {

        IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }


        // GET: api/Chat
        [HttpGet]
        public IEnumerable<ApiEntity.IClient> Get()
        {
            return chatService.Clients.Select(s => s as IClient);
        }

        // GET: api/Chat
        [HttpGet]
        [Route("{guid}")]
        public IEnumerable<IClient> Get(Guid guid)
        {
            return chatService.Clients.Where(w => w.Id == guid).Select(s => s as IClient);
        }

        [HttpPost]
        [Route("Send")]
        public IActionResult Send(IEnumerable<ChatMessage> messages)
        {
            chatService.SendMessages(messages);
            return Ok();
        }
        
        [HttpGet]
        [Route("Subscribe/{name}")]
        public IActionResult Subscribe(string name)
        {
            return chatService.Subscribe(HttpContext, name);
        }

       
    }
}
