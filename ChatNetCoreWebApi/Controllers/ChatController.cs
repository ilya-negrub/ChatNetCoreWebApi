using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatNetCoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {

        private static ConcurrentBag<Client> clients = new ConcurrentBag<Client>();

        // GET: api/Chat
        [HttpGet]
        public IEnumerable<ApiEntity.IClient> Get()
        {
            return clients.Select(s => s as IClient);
        }

        // GET: api/Chat
        [HttpGet]
        [Route("{guid}")]
        public IEnumerable<IClient> Get(Guid guid)
        {
            return clients.Where(w => w.Id == guid).Select(s => s as IClient);
        }

        [HttpPost]
        [Route("Send")]
        public IActionResult Send(IEnumerable<ChatMessage> messages)
        {
            foreach (var message in messages)
            {
                var client = clients.Where(w => w.Id == message.To).FirstOrDefault();
                if (client != null)
                {
                    Task.Factory.StartNew(async () => { await client.SendMessage(message); });
                }   
            }

            return Ok();
        }
        
        [HttpGet]
        [Route("Subscribe/{name}")]
        public IActionResult Subscribe(string name)
        {
            return new PushStreamResult<string>(OnStreamAvailable, "text/event-stream", HttpContext.RequestAborted, name);
        }

        private async void OnStreamAvailable(Stream stream, CancellationToken requestAborted, string name)
        {
            var wait = requestAborted.WaitHandle;
            var writer = new StreamWriter(stream);
            var client = new Client(name, writer);
            clients.Add(client);

            //Send client created Id
            await writer.WriteLineAsync(JsonConvert.SerializeObject(client.Id));
            await writer.FlushAsync();

            await ClientConnected(client);

            wait.WaitOne();

            Client ignore;
            clients.TryTake(out ignore);

            await ClientDisconnected(client);
        }

        private async Task ClientConnected(Client client)
        {
            //Send message all client
            foreach (var item in clients.Where(w => w.Id != client.Id))
            {
                await item.Connected(client);
            }
        }

        private async Task ClientDisconnected(Client client)
        {
            //Send message all client
            foreach (var item in clients.Where(w => w.Id != client.Id))
            {
                await item.Disconnected(client);
            }
        }
    }
}
