using CertificateManagerService;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Klijent
{
    internal class Program
    {
        static WCFClient wcfClient;
        static WCFConnect wcfConnect;

        static void Main(string[] args)
        {


            NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://localhost:9999/Service";
            //string addressCert = "net.tcp://localhost:9999/ServiceCert";


            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));

            wcfClient = new WCFClient(binding, endpointAddress);

            UserInterface();

            Console.WriteLine("Konekcija ugasena.");
            Console.ReadLine();

        }

        public static void UserInterface()
        {

            int option = 0;
            do
            {
                Console.WriteLine("1. Izgenerisi sertifikat sa privatnim kljucem");
                Console.WriteLine("2. Izgenerisi sertifikat bez privatnog kljuca");
                Console.WriteLine("3. Konektuj se na server");
                Console.WriteLine("4. KRAJ");
                int.TryParse(Console.ReadLine(), out option);
                string root;

                switch (option)
                {
                    case 1:
                        Console.WriteLine("Unesite root: ");
                        root = Console.ReadLine();
                        wcfClient.CertificateWithPvk(root);
                        break;
                    case 2:
                        Console.WriteLine("Unesite root: ");
                        root = Console.ReadLine();
                        wcfClient.CertificateWithoutPvk(root);
                        break;
                    case 3: //exit program
                        ConnectToServer();
                        break;
                    case 4: //exit program
                        break;
                    default:
                        break;
                }
            } while (option < 4);
        }

        private static void ConnectToServer()
        {
            NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://localhost:4005/Server";
            //string addressCert = "net.tcp://localhost:9999/ServiceCert";


            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            binding.MaxReceivedMessageSize = 10000000;
            binding.MaxBufferSize = 10000000;
            binding.MaxBufferPoolSize = 10000000;
            binding.OpenTimeout = new TimeSpan(0, 10, 0);
            binding.CloseTimeout = new TimeSpan(0, 10, 0);
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 10, 0);

            Console.WriteLine("Unesite ime servera: ");
            string serverName = Console.ReadLine();
            X509Certificate2 servCert;
            try
            {
                servCert = CertMng.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, serverName);
                if (servCert == null)
                {
                    Console.WriteLine("Uneli ste servera koji ne postoji, molimo vas probajte ponovo");
                    return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Uneli ste servera koji ne postoji, molimo vas probajte ponovo");
                return;

            }


            WCFConnect proxyWcf;
  
            try
            {
                EndpointAddress addressServer = new EndpointAddress(new Uri(address), new X509CertificateEndpointIdentity(servCert));

                proxyWcf = new WCFConnect(binding, addressServer);

                Console.WriteLine("Uspostavljena je komunikacija\n");

                Console.WriteLine("Unesite poruku: ");
                string msg = Console.ReadLine();
                string name = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                string response = proxyWcf.CommunicateWithService(msg, name);
                Console.WriteLine(response);


                


            }
            
            catch (Exception e)
            {
                Console.WriteLine("Neuspesna konekcija, generisite sertifikat pa probajte ponovo");
                return;

            }
            

            wcfConnect = proxyWcf;


        }
        public static void closeConnection()
        {
            if (wcfConnect != null && wcfConnect.State == CommunicationState.Opened)
            {
                try
                {
                    wcfConnect.Close();
                }
                catch (Exception e)
                { }

            }

        }
    }
}
