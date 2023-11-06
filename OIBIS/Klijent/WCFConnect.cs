using CertificateManagerService;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Klijent
{
    public class WCFConnect : ChannelFactory<ICommunication>, ICommunication, IDisposable
    {
        ICommunication factory;

        public WCFConnect(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = CertMng.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }


        public string CommunicateWithService(string message)
        {
            
            try
            {
                return factory.CommunicateWithService(message);
                 
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Problem u komunikaciji izmedju servera i klijenta";
            }
        }
    
    }
}
