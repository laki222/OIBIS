using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract(CallbackContract = typeof(INotify))]
    public interface IService
    {

        [OperationContract]
        bool CertificateWithoutPvk(string root);

        [OperationContract]
        bool CertificateWithPvk(string root);

        [OperationContract]
        bool RevokeCertificate();
    }
}
