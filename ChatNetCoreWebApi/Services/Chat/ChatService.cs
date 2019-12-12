using ApiEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatNetCoreWebApi.Services.Chat
{
    public class ChatService : IChatService
    {
        private static ConcurrentBag<Client> clients = new ConcurrentBag<Client>();

        public IEnumerable<Client> Clients => clients;

        public void SendMessages(IEnumerable<ChatMessage> messages)
        {
            foreach (var message in messages)
            {
                var client = clients.Where(w => w.Id == message.To).FirstOrDefault();
                if (client != null)
                {
                    Task.Factory.StartNew(async () => { await client.SendMessage(message); });
                }
            }
        }

        public IActionResult Subscribe(HttpContext context, string clientName)
        {
            return new PushStreamResult<string>(OnStreamAvailable, "text/event-stream", context.RequestAborted, clientName);
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
