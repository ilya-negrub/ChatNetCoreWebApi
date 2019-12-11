using ApiEntity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientChat
{
    public class ClientViewModel : BaseViewModel, ApiEntity.IClient
    {

        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Message { get; set; }


        private ObservableCollection<MessageViewModel> messages = new ObservableCollection<MessageViewModel>();
        public ObservableCollection<MessageViewModel> Messages => messages;



        private ICommand sendCommand;
        public ICommand SendCommand => sendCommand ?? (sendCommand = new Command(Send_Click));

        private async void Send_Click()
        {
            ChatMessage[] messages = new[]
            {
                new ChatMessage
                {
                    From = ApiClient.SelfId,
                    To = Id,
                    Message = Message,
                    DateTime = DateTime.Now,
                }
            };

            this.messages.Add(new SelfMessageViewModel(messages[0]));

            await ApiClient.SendMessage(messages);
            
            Message = string.Empty;
            OnPropertyChanged(nameof(Message));
        }

        internal void MessageNotify(MessageViewModel message)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => 
            { 
                this.messages.Add(message); 
            }));
        }
    }

}
