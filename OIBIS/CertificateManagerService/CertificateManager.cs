using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CertificateManager : IService, ICertificateManagerService
    {
        private static List<INotify> clients = new List<INotify>();
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
            string groups = GetUserGroups(windowsIdentity);

            if (!File.Exists(root + ".cer"))
            {
                Console.WriteLine("Ne postoji root sa tim imenom: " + root);
                return false;
            }

            try
            {
                string password = "1234";

                /*> makecert -sv WCFService.pvk -iv TestCA.pvk -n "CN=wcfservice" -pe -ic TestCA.cer WCFService.cer -sr localmachine -ss My -sky exchange */

                
                string cmd = "/c makecert -sv " + commonName + ".pvk -iv " + root + ".pvk -n \"CN=" + commonName + ",OU=" + groups + "\" -pe -ic " + root + ".cer " + commonName + ".cer -sr localmachine -ss My -sky exchange";
                System.Diagnostics.Process.Start("cmd.exe", cmd).WaitForExit();

                string cmd2 = "/c pvk2pfx.exe /pvk " + commonName + ".pvk /pi " + password + " /spc " + commonName + ".cer /pfx " + commonName + ".pfx";
                System.Diagnostics.Process.Start("cmd.exe", cmd2).WaitForExit();

                UpisiSertifikat(commonName, password);

                if (File.Exists(commonName + ".cer"))
                {
                    Audit.CertificatePasswordCreated(commonName);
                    return true;
                }
                else
                {
                    Audit.CertificatePasswordFailed(commonName);
                    return false;
                }

            }
            catch (Exception e)
            {
                Audit.CertificatePasswordFailed(commonName);
                Console.WriteLine("Greska prilikom kreiranja sertifikata sa sifrom " + e.Message);
                return false;
            }

        }

        public bool CertificateWithoutPvk(string root)
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string commonName = Formatter.ParseName(windowsIdentity.Name);
            string groups = GetUserGroups(windowsIdentity);

            if (!File.Exists(root + ".cer"))
            {
                Console.WriteLine("Ne postoji root sa tim imenom: " + root);
                return false;
            }
            
            try
            {

                string cmd = "/c makecert -iv " + root + ".pvk -n \"CN=" + commonName + ",OU=" + groups + "\" -ic " + root + ".cer " + commonName + ".cer -sr localmachine -ss My -sky exchange";
                System.Diagnostics.Process.Start("cmd.exe", cmd).WaitForExit();

                UpisiSertifikat(commonName, "");

                if (File.Exists(commonName + ".cer"))
                {
                    Audit.CertificateCreated(commonName);
                    return true;
                }
                else
                {
                    Audit.CertificateFailed(commonName);
                    return false;
                }
            }
            catch (Exception e)
            {
                Audit.CertificateFailed(commonName);
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

        public static X509Certificate2 GetCertificateFromFile(string fileName)
        {
            X509Certificate2 certificate = new X509Certificate2();

            byte[] niz = File.ReadAllBytes(fileName + ".cer");
            certificate.Import(niz);

            return certificate;
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


        private string GetUserGroups(WindowsIdentity windowsIdentity)
        {
            string groups = "";
            foreach (IdentityReference group in windowsIdentity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount)).ToString();

                if (name.Contains('\\'))
                    name = name.Split('\\')[1];

                if (name == "RegionEast" || name == "RegionWest" || name == "RegionSouth" || name == "RegionNorth")
                {
                    if (groups != "")
                        groups += "_" + name;
                    else
                        groups = name;
                }
            }

            return groups;
        }

        public bool RevokeCertificate()
        {
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            string path = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\Sertifikati";
            string commonName = Formatter.ParseName(windowsIdentity.Name);

            
            string cerFilePath = Path.Combine(path, commonName + ".cer");
            string pvkFilePath = Path.Combine(path, commonName + ".pvk");
            string pfxFilePath = Path.Combine(path, commonName + ".pfx");

            Console.WriteLine(cerFilePath);

            try
            {
                X509Certificate2 certificate = CertMng.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, commonName);
                if (certificate == null)
                    return false;

                if (File.Exists("RevocationLista.txt"))
                {
                    using (StreamReader sr = new StreamReader("RevocationLista.txt"))
                    {
                        string contents = sr.ReadToEnd();
                        if (contents.Contains(certificate.Thumbprint))
                        {
                            Console.WriteLine("Sertifikat je vec povucen");
                            return false;
                        }
                    }
                }

                using (StreamWriter sw = new StreamWriter("RevocationLista.txt", true))
                {
                    sw.WriteLine(certificate.Thumbprint);
                }
                Console.WriteLine("Dodavanje u Revocation listu! ");

                


                if (File.Exists(commonName + ".cer") && File.Exists(cerFilePath))
                {
                    File.Delete(commonName + ".cer");
                    File.Delete(cerFilePath); 
                }
                if (File.Exists(commonName + ".pvk") && File.Exists(pvkFilePath))
                {
                    File.Delete(commonName + ".pvk");
                    File.Delete(pvkFilePath);
                }
                if (File.Exists(commonName + ".pfx") && File.Exists(pfxFilePath))
                {
                    File.Delete(commonName + ".pfx");
                    File.Delete(pfxFilePath);
                }

                Audit.CertificateRevoked(commonName);


                foreach (var item in clients)
                {
                    try
                    {
                        item.NotifyClients(certificate.Thumbprint, commonName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Desila se greska prilikom obavestavanja klijenata " + e.Message);
                    }
                }



                return true;
            }
            catch (Exception e)
            {
                Audit.CertificateRevokeFailed(commonName);

                return false;
            }
        }


    }
}
