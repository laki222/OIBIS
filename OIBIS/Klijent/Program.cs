using CertificateManagerService;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Klijent
{
    internal class Program
    {
        static WCFClient wcfClient;
        static WCFConnect wcfConnect;

        static void Main(string[] args)
        {
            Audit.Initialize();

            NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://localhost:9999/Service";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));

            ClientNotify clientNotify = new ClientNotify();
            wcfClient = new WCFClient(clientNotify, binding, endpointAddress);

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
                Console.WriteLine("6. Konektuj se na Host server");
                Console.WriteLine("7. Nasumicno javljanje servisu");
                Console.WriteLine("8. Zatvori konekciju");
                int.TryParse(Console.ReadLine(), out option);
                string root;
                string path = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\CertificateManagerService\bin\Debug";
                string password;

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
                    case 3:
                        Console.WriteLine("Unesite sifru: ");
                        password = Console.ReadLine();
                        wcfClient.InstallCertificateWithPvk(path, password);
                        break;
                    case 4:
                        wcfClient.InstallCertificateWithoutPvk(path);
                        break;
                    case 5:
                        wcfClient.RevokeCertificate();
                        break;
                    case 6: 
                        ConnectToServer();
                        break;
                    case 7:
                        SendRandomTimedMessage();
                        break;
                    case 8: //exit program
                        closeConnection();
                        break;
                    default:
                        Console.WriteLine("Greska");
                        break;
                }
            } while (option < 8);
        }

        private static void ConnectToServer()
        {
            NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://localhost:4000/Server";

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
                    Console.WriteLine("Server ne postoji. Molimo vas probajte ponovo.");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Server ne postoji. Molimo vas probajte ponovo.\n {e.Message}");
                return;
            }

            WCFConnect proxyWcf;
  
            try
            {
                EndpointAddress addressServer = new EndpointAddress(new Uri(address), new X509CertificateEndpointIdentity(servCert));

                proxyWcf = new WCFConnect(binding, addressServer);
                string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                Console.WriteLine("Uspostavljena je komunikacija\n");

                Audit.ClientConnectionOpen(clientName);

                Console.WriteLine("Unesite poruku: ");
                string msg = Console.ReadLine();
                string name = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                string response = proxyWcf.CommunicateWithService(msg, name);

                Console.WriteLine(response);
            }
            
            catch (Exception e)
            {
                Console.WriteLine($"Neuspesna konekcija, generisite sertifikat pa probajte ponovo.\n {e.Message} ");
                return;

            }
            wcfConnect = proxyWcf;
        }
        private static void SendRandomTimedMessage()
        {
            if (wcfConnect == null)
            {
                Console.WriteLine("Prvo se konektujte na server");
                return;
            }
            Random r = new Random();
            try
            {
                bool shouldExit = false;

                while (!shouldExit)
                {
                    Thread.Sleep(r.Next(1, 10) * 1000);
                    wcfConnect.SendRandomTimedMessage(DateTime.Now);

                    Console.WriteLine("Saljem poruku, unesite 'X' za prekid");

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.X)
                        {
                            shouldExit = true;
                            Console.WriteLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void closeConnection()
        {
            if (wcfConnect != null && wcfConnect.State == CommunicationState.Opened)
            {
                try
                {
                    string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                    string odg = wcfConnect.NotifyClientDisconnected(clientName);
                    Console.WriteLine(odg);
                    Audit.ClientConnectionClosed(clientName);
                    wcfConnect.Close();
                }
                catch (Exception e)
                { }

            }

        }
    }
}
