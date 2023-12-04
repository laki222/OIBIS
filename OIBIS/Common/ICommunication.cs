using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface ICommunication
    {
        [OperationContract]
        string CommunicateWithService(string message, string name);

        [OperationContract]
        string NotifyClientDisconnected(string clientName);
    }
}
