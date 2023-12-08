using CertificateManagerService;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerNotify : INotify
    {
        public void NotifyClients(string message, string srvName)
        {
            Console.WriteLine("Obrisan je sertifikat: " + srvName);
            string myName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            X509Certificate2 servCert = CertMng.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, myName);
            if (servCert != null)
            {
                if (servCert.Thumbprint == message)
                {
                    Console.WriteLine("Zatvaranje konekcije sa strane servera");
                    Program.closeConnection();

                    Audit.ServerConnectionClosed(srvName);
                }
            }
        }
    }
}
