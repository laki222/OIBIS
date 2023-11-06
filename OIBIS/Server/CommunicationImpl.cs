using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class CommunicationImpl : ICommunication
    {
        public string CommunicateWithService(string message)
        {
            Console.WriteLine($"Client connected: {message}");
            return $"Service received: {message}";
        }
    }
}
