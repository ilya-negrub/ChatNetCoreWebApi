using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using ApiEntity;
using System.Windows.Input;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace ClientChat
{
    public class MainViewModel : BaseViewModel
    {
        public string Name { get; set; } = "Client";
        

        private ICommand connectCommand;
        public ICommand ConnectCommand => connectCommand ?? (connectCommand = new Command(Connect_Click));

        private async void Connect_Click()
        {
            await ApiClient.StartListening(Name);
            await ApiClient.UpdateClients();
        }


        
    }

    
}
