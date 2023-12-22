using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    public class ProxyBackup : ChannelFactory<IBackupService>, IBackupService, IDisposable
    {
        IBackupService factory;

        public ProxyBackup(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public void UpisCertificateList(string certificate)
        {
            factory.UpisCertificateList(certificate);
        }

        public void UpisRevocationList(string thumbprint)
        {
            Console.WriteLine("Saljem na Backup server");
            factory.UpisRevocationList(thumbprint);
        }
    }
}
