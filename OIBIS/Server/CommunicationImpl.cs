using CertificateManagerService;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Klijent;

namespace Server
{
    public class CommunicationImpl : ICommunication
    {
        public string CommunicateWithService(string message, string name)
        {
            //$"Service received: {message} from {name}"
            Console.WriteLine($"Client '{name}' connected. \nwith message: {message}");
            return "";
        }

        public string NotifyClientDisconnected(string clientName)
        {
            // Handle the client disconnection here
            Console.WriteLine($"Client '{clientName}' has disconnected.");
            return "";
        }
    }
}
