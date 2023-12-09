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
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        private static WCFHost wcfHost;
        private static WCFServer wcfServer;
        static void Main(string[] args)
        {
            Audit.Initialize();

            NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://localhost:9999/Service";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));
            
            ServerNotify serverNotify= new ServerNotify();
            wcfServer = new WCFServer(serverNotify, binding, endpointAddress);          

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
                Console.WriteLine("3. Obrisi sertifikat");
                Console.WriteLine("4. Startuj Host Server");
                Console.WriteLine("5. KRAJ");
                int.TryParse(Console.ReadLine(), out option);
                string root;
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Unesite root: ");
                        root = Console.ReadLine();
                        wcfServer.CertificateWithPvk(root);
                        break;
                    case 2:
                        Console.WriteLine("Unesite root: ");
                        root = Console.ReadLine();
                        wcfServer.CertificateWithoutPvk(root);
                        break;
                    case 3:
                        wcfServer.RevokeCertificate();
                        break;
                    case 4:
                        startujHostServer();
                        break;
                    case 5: //exit program
                        break;
                    default:
                        Console.WriteLine("Greska ");
                        break;
                }
            } while (option < 5);
        }

        private static void startujHostServer()
        {
            try
            {
                
                wcfHost = new WCFHost();
                wcfHost.OpenServer();
                //Console.WriteLine("WCFHost je startovan\n ");
                Console.ReadLine();
                

            }
            catch (Exception e)
            {
                Console.WriteLine("Neuspesna konekcija, generisite sertifikat pa probajte ponovo \n" +e.Message);
            }
        }

        public static void closeConnection()
        {
            if (wcfHost != null)
            {
                wcfHost.CloseServer();
            }
        }
    }
}

    

