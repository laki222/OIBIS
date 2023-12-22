using CertificateManagerService;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Klijent;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Claims;
using System.IO;
using System.Security;
using System.ServiceModel;

namespace Server
{
    public class CommunicationImpl : ICommunication
    {
        private int id = 0;
        public string CommunicateWithService(string message, string name)
        {
            X509Certificate2 clientCert = ((X509CertificateClaimSet)
                    OperationContext.Current.ServiceSecurityContext.AuthorizationContext.ClaimSets[0]).X509Certificate;

            string subjectName = clientCert.SubjectName.Name;
            string OU = subjectName.Split(',')[1];

            string commonName = subjectName.Split(',')[0];
            commonName = commonName.Substring(3);

            if (!(OU.Contains("RegionWest") || OU.Contains("RegionEast") || OU.Contains("RegionNorth") || OU.Contains("RegionSouth")))
            {
                Console.WriteLine("Korisnik koji pokusava da salje poruku nije deo zahtevane grupe");
                throw new SecurityException("Korisnik koji pokusava da salje poruku nije deo zahtevane grupe");

            }
            name = commonName;
            Console.WriteLine($"Client '{name}' connected. \nwith message: {message}");
            return "";
        }

        public string NotifyClientDisconnected(string clientName)
        {
            Console.WriteLine($"Client '{clientName}' has disconnected.");
            return "";
        }

        public void SendRandomTimedMessage(DateTime vreme)
        {
            X509Certificate2 clientCert = ((X509CertificateClaimSet)
                    OperationContext.Current.ServiceSecurityContext.AuthorizationContext.ClaimSets[0]).X509Certificate;

            string subjectName = clientCert.SubjectName.Name;
            string OU = subjectName.Split(',')[1];

            string commonName = subjectName.Split(',')[0];
            commonName = commonName.Substring(3);

            if (!(OU.Contains("RegionWest") || OU.Contains("RegionEast") || OU.Contains("RegionNorth") || OU.Contains("RegionSouth")))
            {
                Console.WriteLine("Korisnik koji pokusava da salje poruku nije deo zahtevane grupe");
                throw new SecurityException("Korisnik koji pokusava da salje poruku nije deo zahtevane grupe");

            }

            if (File.Exists("servisLog.txt"))
            {
                string lastLine = File.ReadLines("servisLog.txt").Last();
                int index = lastLine.IndexOf(':');
                int lastID = int.Parse(lastLine.Substring(0, index));

                id = lastID;
            }

            using (StreamWriter sw = new StreamWriter("servisLog.txt", true))
            {
                id++;
                sw.WriteLine(id + ": " + String.Format("{0:g}", vreme) + "; " + commonName);
            }
            Console.WriteLine(id + ": " + String.Format("{0:g}", vreme) + "; " + commonName);
        }
    }
}
