﻿using CertificateManagerService;
using Common;
using Klijent;
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
    public class WCFHost
    {
        ServiceHost host;

        public WCFHost()
        {
            try
            {
                Audit.Initialize();

                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                binding.MaxReceivedMessageSize = 10000000;
                binding.MaxBufferSize = 10000000;
                binding.MaxBufferPoolSize = 10000000;
                binding.OpenTimeout = new TimeSpan(0, 10, 0);
                binding.CloseTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);

                string address = "net.tcp://localhost:4000/Server";
                string srvName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                host = new ServiceHost(typeof(CommunicationImpl));
                host.AddServiceEndpoint(typeof(ICommunication), binding, address);

                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
                host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                host.Credentials.ServiceCertificate.Certificate = CertMng.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvName);

                Console.WriteLine("Server je pokrenut");
                Console.WriteLine(WindowsIdentity.GetCurrent().Name);

            }
            catch (Exception e)
            {
                Console.WriteLine("Neuspesna konekcija, generisite sertifikat pa probajte ponovo \n"+e.Message);
            }

        }
        public void OpenServer()
        {
            host.Open();
            string srvName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            Audit.ServerConnectionOpen(srvName);
        }
        public void CloseServer()
        {   
            string srvName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            host.Close();
            Audit.ServerConnectionClosed(srvName);
            
        }
    }
}

