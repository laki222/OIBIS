using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    public class CertificateManager : IService, ICertificateManagerService
    {
        public string CommunicateWithService(string message)
        {
            Console.WriteLine($"Client connected: {message}");
            return $"Service received: {message}";
        }

        public bool CertificateWithPvk(string root)
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string commonName = Formatter.ParseName(windowsIdentity.Name);


            if (!File.Exists(root + ".cer"))
            {
                Console.WriteLine("Ne postoji root sa tim imenom: " + root);
                return false;
            }

            try
            {
                string password = "1234";

                /*> makecert -sv WCFService.pvk -iv TestCA.pvk -n "CN=wcfservice" -pe -ic TestCA.cer WCFService.cer -sr localmachine -ss My -sky exchange */

                string cmd = "/c makecert -sv " + commonName + ".pvk -iv " + root + ".pvk -n \"CN=" + commonName  + "\" -pe -ic " + root + ".cer " + commonName + ".cer -sr localmachine -ss My -sky exchange";
                System.Diagnostics.Process.Start("cmd.exe", cmd).WaitForExit();

                string cmd2 = "/c pvk2pfx.exe /pvk " + commonName + ".pvk /pi " + password + " /spc " + commonName + ".cer /pfx " + commonName + ".pfx";
                System.Diagnostics.Process.Start("cmd.exe", cmd2).WaitForExit();

                UpisiSertifikat(commonName, password);

                if (File.Exists(commonName + ".cer"))
                {

                    return true;
                }
                else
                {                  
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Greska prilikom kreiranja sertifikata sa sifrom " + e.Message);
                return false;
            }

        }

        public bool CertificateWithoutPvk(string root)
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string commonName = Formatter.ParseName(windowsIdentity.Name);

            if (!File.Exists(root + ".cer"))
            {
                Console.WriteLine("Ne postoji root sa tim imenom: " + root);
                return false;
            }
            
            try
            {

                string cmd = "/c makecert -iv " + root + ".pvk -n \"CN=" + commonName + "\" -ic " + root + ".cer " + commonName + ".cer -sr localmachine -ss My -sky exchange";
                System.Diagnostics.Process.Start("cmd.exe", cmd).WaitForExit();

                UpisiSertifikat(commonName, "");

                if (File.Exists(commonName + ".cer"))
                {         
                     return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Greska prilikom kreiranja sertifikata bez sifre " + e.Message);
                return false;
            }

        }

        public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            using (X509Store store = new X509Store(storeName, storeLocation))
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);

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


        private void UpisiSertifikat(string commonName, string password)
        {
            try
            {
                
                string folderPath = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\Sertifikati"; 
                Directory.CreateDirectory(folderPath);

                X509Certificate2 certificate;
                if (password == "")
                    certificate = new X509Certificate2(commonName + ".cer");
                else
                    certificate = new X509Certificate2(commonName + ".cer", password);

                
                string certificatePath = Path.Combine(folderPath, commonName + ".cer");
                File.WriteAllBytes(certificatePath, certificate.Export(X509ContentType.Cert));
                

                Console.WriteLine("Sertifikat sacuvan u: " + certificatePath);

                string pfxFilePath = Path.Combine(folderPath, commonName + ".pfx");
                File.WriteAllBytes(pfxFilePath, certificate.Export(X509ContentType.Pkcs12, password));

                Console.WriteLine("Kljuc sacuvan u: " + certificatePath);

            }
            catch (Exception e)
            {
                Console.WriteLine("Greska prilikom cuvanja sertifikata: " + commonName + " " + e.Message);
            }
        }

        public static X509Certificate2 GetCertificateFromFile(string fileName)
        {
            X509Certificate2 certificate = new X509Certificate2();

            byte[] niz = File.ReadAllBytes(fileName + ".cer");
            certificate.Import(niz);

            return certificate;
        }

    }
}
