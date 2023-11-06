using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace Backup
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;



            ChannelFactory<ITest> cfIzvor = new ChannelFactory<ITest>("Izvor");
            ChannelFactory<ITest> cfOdrediste = new ChannelFactory<ITest>("Odrediste");
            ITest kIzvor = cfIzvor.CreateChannel();
            ITest kOdrediste = cfOdrediste.CreateChannel();

            Console.WriteLine("Backup server pokrenut");

            //string test= kOdrediste.DodajLice(kIzvor.DodajLice("marko"));
            //Console.WriteLine(test);
            Console.ReadLine();

        }
    }
}
