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

namespace Server
{
    public class WCFServer : DuplexChannelFactory<IService>, IService, IDisposable
    {
        IService factory;

        public WCFServer(object serverNotify, NetTcpBinding binding, EndpointAddress address) : base(serverNotify, binding, address)
        {

            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = CertMng.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }

        public bool CertificateWithPvk(string root)
        {

            if (!factory.CertificateWithPvk(root))
            {

                Console.WriteLine("Neuspesno  generisanje sertifikata sa kljucem. Pokusajte ponovo!");
                return false;
            }
            else
            {

                Console.WriteLine("Uspesno generisanje sertifikata sa kljucem!");
                return true;
            }

        }

        public bool CertificateWithoutPvk(string root)
        {
            if (!factory.CertificateWithoutPvk(root))
            {
                Console.WriteLine("Neuspesno  generisanje sertifikata bez kljuca. Pokusajte ponovo!");
                return false;
            }
            else
            {
                Console.WriteLine("Uspesno generisanje sertifikata bez kljuca!");

                return true;
            }

        }

        public bool RevokeCertificate()
        {
            if (!factory.RevokeCertificate())
            {
                Console.WriteLine("Neuspesno povlacenje sertifikata!");
                return false;
            }
            else
            {
                Console.WriteLine("Sertifikati su povuceni i upisani u RevocationList.txt. Pritisnite enter da nastavite");
                Console.ReadLine();

                return true;
            }

        }
        public void InstallCertificateWithoutPvk(string path)
        {
            try
            {
                factory.InstallCertificateWithoutPvk(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem prilikom instalacije sertifikata bez privatnog kljuca");
            }
        }
        public void InstallCertificateWithPvk(string path, string password)
        {
            try
            {
                factory.InstallCertificateWithPvk(path, password);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem prilikom instalacije sertifikata sa privatnim kljucem");
            }
        }
    }
}
