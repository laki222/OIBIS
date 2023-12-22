using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CertMng
    {
        public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

            foreach (X509Certificate2 cert in certCollection)
            {
                string name = "";
                if (cert.SubjectName.Name.Contains(','))
                {
                    name = cert.SubjectName.Name.Split(',')[0];
                }

                if (name.Equals(string.Format("CN={0}", subjectName)))
                {
                    return cert;
                }
            }
            return null;

        }
    }
}
