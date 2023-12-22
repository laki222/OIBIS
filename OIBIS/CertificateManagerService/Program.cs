using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    public class Program
    {
        public static ProxyBackup proxyBackup = null;
        static void Main(string[] args)
        {
            Audit.Initialize();

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/Service";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(CertificateManager));
            host.AddServiceEndpoint(typeof(IService), binding, address);

            //Replication proxy
            NetTcpBinding bindingBackup = new NetTcpBinding();
            string addressBackup = "net.tcp://localhost:9997/BackupService";

            bindingBackup.Security.Mode = SecurityMode.Transport;
            bindingBackup.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            bindingBackup.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(addressBackup));
            proxyBackup = new ProxyBackup(binding, endpointAddress);

            napraviRoot();

            try
            {
                host.Open();
                Console.WriteLine("CMS servis je pokrenut.\nKliknite enter da zaustavite ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Desila se greska" + e.Message);
                Console.ReadLine();
            }
            finally
            {
                host.Close();
            }
        }

        private static void napraviRoot()
        {
            Console.WriteLine("Unesite ime root-a : ");
            string root = Console.ReadLine();
            try
            {
                if (File.Exists(root + ".cer"))
                {
                    Console.WriteLine("Vec postoji sertifikat " + root);
                    return;
                }

                string cmd = "/c makecert -n \"CN =" + root + "\" -r -sv " + root + ".pvk " + root + ".cer";
                System.Diagnostics.Process.Start("cmd.exe", cmd).WaitForExit();

                Console.WriteLine("Kreiran je root sertifikat");

                X509Certificate2 certificate = new X509Certificate2(root + ".cer");

                proxyBackup.UpisCertificateList(certificate.Subject + ", thumbprint: " + certificate.Thumbprint);

                Audit.CertificatePasswordCreated(root);
                Audit.CertificateReplicated(root);
               
            }
            catch (Exception e)
            {
                Audit.CertificatePasswordFailed(root);
                Console.WriteLine("Neuspesno kreiran root sertifikat"); 
            }
        }

    }
}

