﻿using CertificateManagerService;
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
    public class WCFClient : ChannelFactory<IService>, IService, IDisposable
	{
        IService factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
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

                Console.WriteLine("Instalirajte sertifikate u mmc i kliknite enter da nastavite.");
                Console.ReadLine();

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

                Console.WriteLine("Instalirajte sertifikate u mmc i kliknite enter da nastavite.");
                Console.ReadLine();

                Console.WriteLine("Uspesno generisanje sertifikata bez kljuca!");

                return true;
            }

        }


    }
}
