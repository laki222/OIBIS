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

            Console.WriteLine("Konekcija je ugasena.");
            Console.ReadLine();

        }
        public static void UserInterface()
        {

            int option = 0;
            do
            {
                Console.WriteLine("1. Izgenerisi sertifikat sa privatnim kljucem");
                Console.WriteLine("2. Izgenerisi sertifikat bez privatnog kljuca");
                Console.WriteLine("3. Instaliraj sertifikat sa privatnim kljucem");
                Console.WriteLine("4. Instaliraj sertifikat bez privatnog kljuca");
                Console.WriteLine("5. Obrisi sertifikate");
                Console.WriteLine("6. Pokreni Host Server");
                Console.WriteLine("7. Zatvori konekciju");
                int.TryParse(Console.ReadLine(), out option);
                string root;
                string path = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\CertificateManagerService\bin\Debug";
                string password;

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
                        Console.WriteLine("Unesite sifru: ");
                        password = Console.ReadLine();
                        wcfServer.InstallCertificateWithPvk(path, password);
                        break;
                    case 4:
                        wcfServer.InstallCertificateWithoutPvk(path);
                        break;
                    case 5:
                        wcfServer.RevokeCertificate();
                        break;
                    case 6:
                        StartujHostServer();
                        break;
                    case 7: //exit program
                        CloseConnection();
                        break;
                    default:
                        Console.WriteLine("Greska ");
                        break;
                }
            } while (option < 7);
        }

        private static void StartujHostServer()
        {
            try
            {  
                wcfHost = new WCFHost();
                wcfHost.OpenServer();
                Console.ReadLine();   
            }
            catch (Exception e)
            {
                Console.WriteLine("Neuspesna konekcija, generisite sertifikat pa probajte ponovo \n" +e.Message);
            }
        }
        public static void CloseConnection()
        {
            if (wcfHost != null)
            {
                wcfHost.CloseServer();
            }
        }
    }
}

    

