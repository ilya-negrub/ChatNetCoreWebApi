using ApiEntity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ChatNetCoreWebApi.Services.Chat
{
    public class Client : IClient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public StreamWriter Stream { get; private set; }

        public Client(string name, StreamWriter stream)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Stream = stream;
        }

        public async Task SendMessage(ChatMessage message)
        {

            var command = new SendMessageCommand()
            {
                TypeName = typeof(SendMessageCommand).Name,
                Data = message,
            };

            await Stream.WriteLineAsync(JsonConvert.SerializeObject(command));
            await Stream.FlushAsync();
        }

        public async Task Connected(IClient client)
        {
            var command = new ClientConnectedCommand
            {
                TypeName = typeof(ClientConnectedCommand).Name,
                Data = client.Id,
            };

            await Stream.WriteLineAsync(JsonConvert.SerializeObject(command));
            await Stream.FlushAsync();
        }

        public async Task Disconnected(IClient client)
        {
            var command = new ClientDisconnectedCommand
            {
                TypeName = typeof(ClientDisconnectedCommand).Name,
                Data = client.Id,
            };

            await Stream.WriteLineAsync(JsonConvert.SerializeObject(command));
            await Stream.FlushAsync();
        }
    }
}
