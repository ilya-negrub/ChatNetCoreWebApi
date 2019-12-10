using ApiEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;

namespace ClientChat
{
    internal static class ApiClient
    {
        private static bool isListening = false;
        private static HttpClient httpClient = new HttpClient();
        private static ObservableCollection<ClientViewModel> clients = new ObservableCollection<ClientViewModel>();

        public static bool IsListening => isListening;
        public static HttpClient HttpClient => httpClient;
        public static Guid SelfId { get; private set; }
        public static ObservableCollection<ClientViewModel> Clients => clients;

        static ApiClient()
        {
            httpClient.Timeout = TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite);
            httpClient.BaseAddress = new Uri("http://localhost:5000");            
        }


        public static async Task StartListening(string clentName)
        {
            if (isListening) return;

            var stream = await httpClient.GetStreamAsync($"api/Chat/Subscribe/{clentName}");
            Listener(new StreamReader(stream));
        }

        public static async Task UpdateClients()
        {
            HttpResponseMessage response = await httpClient.GetAsync("api/Chat");
            if (response != null && response.IsSuccessStatusCode)
            {
                var c = await response.Content.ReadAsAsync<IEnumerable<ClientViewModel>>();

                System.Windows.Application.Current.Dispatcher.Invoke(clients.Clear);
                foreach (var client in c)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => { clients.Add(client); });
                }
            }
        }

        public static async Task SendMessage(IEnumerable<ChatMessage> messages)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/Chat/Send", messages);
        }

        private static void Listener(StreamReader reader)
        {
            Task.Factory.StartNew(() =>
            {
                isListening = true;
                string line = reader.ReadLine();
                SelfId = JsonConvert.DeserializeObject<Guid>(line);

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    System.Diagnostics.Debug.WriteLine(line);

                    try
                    {
                        if (JsonConvert.DeserializeObject<CommandApi>(line) is CommandApi command && commands.ContainsKey(command.TypeName))
                        {
                            commands[command.TypeName](line, command);
                        }
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("string was not recognized");
                    }
                }

                SelfId = Guid.Empty;
                isListening = false;
            });
        }

        private static Dictionary<string, Action<string, CommandApi>> commands = new Dictionary<string, Action<string, CommandApi>>
        {
            {typeof(SendMessageCommand).Name, CommandSendMessage },
            {typeof(ClientConnectedCommand).Name, CommandClientConnected },
            {typeof(ClientDisconnectedCommand).Name, CommandClientDisconnected },
            
        };

        

        private static void CommandSendMessage(string json, CommandApi commandsApi)
        {
            if (JsonConvert.DeserializeObject<SendMessageCommand>(json) is SendMessageCommand commands)
            {
                foreach (var client in clients.Where(w => w.Id == commands.Data.From))
                {
                    client.MessageNotify(new MessageViewModel(commands.Data));
                }
            }
        }

        private static void CommandClientConnected(string json, CommandApi commandsApi)
        {
            if (JsonConvert.DeserializeObject<ClientConnectedCommand>(json) is ClientConnectedCommand commands)
            {

                Task.Factory.StartNew(async () =>
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"api/Chat/{commands.Data}");
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        var c = await response.Content.ReadAsAsync<IEnumerable<ClientViewModel>>();
                        System.Windows.Application.Current.Dispatcher.Invoke(() => { c.ToList().ForEach(f => clients.Add(f)); });
                    }
                });
            }
        }

        private static void CommandClientDisconnected(string json, CommandApi commandsApi)
        {
            if (JsonConvert.DeserializeObject<ClientDisconnectedCommand>(json) is ClientDisconnectedCommand commands)
            {
                var clientVm = clients.Where(w => w.Id == commands.Data).FirstOrDefault();
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {   
                    clients.Remove(clientVm);
                }));
            }
        }
    }
}
