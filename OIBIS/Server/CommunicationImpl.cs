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

            Console.WriteLine($"Client connected: {name} with message: {message}");
            return $"Service received: {message} from {name}";
        }
    }
}
