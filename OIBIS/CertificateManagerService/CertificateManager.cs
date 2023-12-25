using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CertificateManagerService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CertificateManager : IService
    {
        public static List<INotify> clients = new List<INotify>();

        /// <summary>
        /// Generisanje sertifikata sa pvk-om
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        /// 
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

                //Komanda za generisanje sertifikata sa pvkom
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
        /// <summary>
        /// Generisanje sertifikata bez pvk-a
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
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
                //  makecert -sv WCFService.pvk -iv TestCA.pvk -n "CN=wcfservice" -pe -ic TestCA.cer
                // WCFService.cer - sr localmachine - ss My - sky exchange

                // cer ima samo javni kljuc
                // pvk ima samo privatni
                // pfx i javni i privatni

                //Komanda za generisanje sertifikata bez pvka
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
                Console.WriteLine("Greska prilikom kreiranja sertifikata bez sifre " + e.Message);
                Audit.CertificateFailed(commonName);
                return false;
            }
        }

        private void UpisiSertifikat(string commonName, string password)
        {
            try
            {
                string folderPath = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\Sertifikati";
                string sourcePath = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\CertificateManagerService\bin\Debug";
                Directory.CreateDirectory(folderPath);

                X509Certificate2 certificate;
                if (password == "")
                  {
                   //Kreiranje sertifikata bez pvk
                   certificate = new X509Certificate2(commonName + ".cer");

                   //Cuvanje samo .cer fajlova
                   string certificatePath = Path.Combine(folderPath, commonName + ".cer");
                   File.WriteAllBytes(certificatePath, certificate.Export(X509ContentType.Cert));
                   Console.WriteLine("Sertifikat sacuvan u: " + certificatePath);
                }
                else
                {
                   //Kreiranje sertifikata sa sifrom
                   certificate = new X509Certificate2(commonName + ".cer", password);

                   //Cuvanje .cer i .pfx fajlova
                   string certificatePath = Path.Combine(folderPath, commonName + ".cer");
                   File.WriteAllBytes(certificatePath, certificate.Export(X509ContentType.Cert));
                   Console.WriteLine("Sertifikat sacuvan u: " + certificatePath);

                   string pfxFilePath = Path.Combine(folderPath, commonName + ".pfx");
                   File.WriteAllBytes(pfxFilePath, certificate.Export(X509ContentType.Pkcs12, password));
                   Console.WriteLine("Kljuc sacuvan u: " + pfxFilePath);

                   string sourcePvkPath = Path.Combine(sourcePath, commonName + ".pvk");
                   string pvkFilePath = Path.Combine(folderPath, commonName + ".pvk");
                   File.Copy(sourcePvkPath, pvkFilePath, true);
                   Console.WriteLine("Privatan kljuc sacuvan u: " + pvkFilePath);

                }
                Program.proxyBackup.UpisCertificateList(certificate.Subject + ", thumbprint: " + certificate.Thumbprint);

                Audit.CertificateReplicated(commonName);
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

            try
            {
                X509Certificate2 certificate = CertMng.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, commonName);

                if (certificate == null)
                    return false;
                //Proverava da li je sertifikat vec povucen
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
                //Otvara Personal skladiste u okviru LocalMachine
                using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                {
                    store.Open(OpenFlags.MaxAllowed);

                    //Brisanje sertifikata iz Personal skladista
                    store.Remove(certificate);
                    Console.WriteLine($"Sertifikat '{commonName}' uspesno povucen iz 'Personal' skladista");
                }

                //Otvara TrustedPeople skladiste u okviru LocalMachine
                using (X509Store storeTrust = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
                {
                    storeTrust.Open(OpenFlags.MaxAllowed);

                    //Brisanje sertifikata iz TrustedPeople skladista
                    storeTrust.Remove(certificate);
                    Console.WriteLine($"Sertifikat '{commonName}' uspesno povucen iz 'TrustedPeople' skladista");
                }

                if (File.Exists(commonName + ".cer") && File.Exists(cerFilePath))
                {
                    File.Delete(commonName + ".cer");
                    File.Delete(cerFilePath);
                    Console.WriteLine($"Sertifikat '{commonName}.cer' uspesno obrisan iz lokalnog direktorijuma");
                }

                if (File.Exists(commonName + ".pvk") && File.Exists(pvkFilePath))
                {
                    File.Delete(commonName + ".pvk");
                    File.Delete(pvkFilePath);
                    Console.WriteLine($"Privatni kljuc '{commonName}.pvk' uspesno obrisan iz lokalnog direktorijuma");
                }

                if (File.Exists(commonName + ".pfx") && File.Exists(pfxFilePath))
                {
                    File.Delete(commonName + ".pfx");
                    File.Delete(pfxFilePath);
                    Console.WriteLine($"Sertifikat '{commonName}.pfx' uspesno obrisan iz lokalnog direktorijuma");
                }

                Audit.CertificateRevoked(commonName);

                using (StreamWriter sw = new StreamWriter("RevocationLista.txt", true))
                {
                    sw.WriteLine(certificate.Subject + ", thumbprint: " + certificate.Thumbprint);
                }
                Console.WriteLine("Dodavanje u Revocation listu! ");

                Program.proxyBackup.UpisRevocationList(certificate.Subject + ", thumbprint: " + certificate.Thumbprint);

                Audit.CertificateRevokedReplicated(commonName);

                Console.WriteLine($"Sertifikat '{commonName}' uspesno povucen.");


                foreach (var item in clients)
                {
                    Console.WriteLine("Obavestavanje klijenata");
                    try
                    {
                        if (item != null)
                        {
                            item.NotifyClients(certificate.Thumbprint, commonName);
                            Console.WriteLine("Klijent uspesno obavesten");
                        }
                        else
                        {
                            Console.WriteLine("Klijent nije obavesten");
                        }
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
                Console.WriteLine($"Greska prilikom povlacenja sertifikata '{commonName}': {e.Message}");
                return false;
            }
        }
        
        public void InstallCertificateWithoutPvk(string sourcePath)
        {
            try
            {
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                WindowsIdentity windowsIdentity = identity as WindowsIdentity;

                //string path = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\Sertifikati";
                sourcePath = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\CertificateManagerService\bin\Debug";

                string commonName = Formatter.ParseName(windowsIdentity.Name);

                string cerFilePath = Path.Combine(sourcePath, commonName + ".cer");

                
                X509Certificate2 certificate = new X509Certificate2(cerFilePath);
                X509Certificate2 certificateTrust = new X509Certificate2(cerFilePath);

                if (certificate == null && certificateTrust == null)
                {
                    Console.WriteLine("Ne postoji sertifikat");
                    return;
                }

                using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                using (X509Store storeTrust = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
                {
                    try
                    {
                        store.Open(OpenFlags.MaxAllowed);
                        store.Add(certificate);
                        //Console.WriteLine("Sertifikat uspesno instaliran!");

                        storeTrust.Open(OpenFlags.MaxAllowed);
                        storeTrust.Add(certificateTrust);
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Greska prilikom instalacije sertifikata: {ex.Message}");
                    }
                }

                Console.WriteLine("Sertifikat bez privatnog kljuca uspesno instaliran.");
                Audit.CertificateWithoutPvkInstallationSuccess(commonName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Greška prilikom instalacije sertifikata: {e.Message}");
            }
        }

        public void InstallCertificateWithPvk(string sourcePath, string password)
        {
           try 
           { 
                IIdentity identity = Thread.CurrentPrincipal.Identity;
                WindowsIdentity windowsIdentity = identity as WindowsIdentity;

                //string path = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\Sertifikati";
                sourcePath = @"C:\Users\Korisnik\Desktop\Projekat\OIBIS\OIBIS\CertificateManagerService\bin\Debug";

                string commonName = Formatter.ParseName(windowsIdentity.Name);

                string pfxFilePath = Path.Combine(sourcePath, commonName + ".pfx");

                if (!File.Exists(pfxFilePath))
                {
                    Console.WriteLine($"Sertifikat ne postoji na ovoj lokaciji: {pfxFilePath}");
                    return;
                }

                X509Certificate2 certificate = new X509Certificate2(pfxFilePath, password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                X509Certificate2 certificateTrust = new X509Certificate2(pfxFilePath, password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                if(certificate == null && certificateTrust == null)
                {
                    Console.WriteLine("Ne postoji sertifikat");
                    return;
                }

                using (X509Store myStore = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                using (X509Store trustedPeopleStore = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine))
                {
                    try
                    {
                        myStore.Open(OpenFlags.MaxAllowed);
                        myStore.Add(certificate);
                        
                        trustedPeopleStore.Open(OpenFlags.MaxAllowed);
                        trustedPeopleStore.Add(certificateTrust);
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Greska prilikom instalacije sertifikata sa privatnim kljucem: {ex.Message}");
                    }
                }

                Console.WriteLine("Sertifikat sa privatnim kljucem uspesno instaliran.");
                Audit.CertificateWithPvkInstallationSuccess(commonName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Greška prilikom instalacije sertifikata sa privatnim kljucem: {e.Message}");
            }
        }
    }
}

