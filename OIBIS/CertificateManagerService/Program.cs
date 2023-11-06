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
        static void Main(string[] args)
        {
            

            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/Service";


            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            




            ServiceHost host = new ServiceHost(typeof(CertificateManager));
            host.AddServiceEndpoint(typeof(IService), binding, address);


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

                


                

            }
            catch (Exception e)
            {
                
            }
        }




        /*NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
        binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

        using (ServiceHost host = new ServiceHost(typeof(CertificateManager)))
        {


            host.Open();

            Console.WriteLine("Korisnik koji je pokrenuo servera :" + WindowsIdentity.GetCurrent().Name);

            Console.WriteLine("Servis je pokrenut.");

            Console.ReadLine();
        }
        */
    }
}

