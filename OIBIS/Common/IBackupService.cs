using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IBackupService
    {
        [OperationContract]
        void UpisCertificateList(String thumbprint);

        [OperationContract]
        void UpisRevocationList(String thumbprint);
    }
}
