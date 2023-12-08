using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Audit
    {
        private static EventLog log = new EventLog();


        public static void Initialize()
        {
            try
            {
                if (!EventLog.SourceExists("CMSEvents"))
                {
                    EventLog.CreateEventSource("CMSEvents", "CMSLog");


                    Console.WriteLine("Napravljen je event log");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem u podizanju event loga " + e.Message);
            }


            log.Source = "CMSEvents";
            log.Log = "CMSLog";
        }


        public static void CertificateCreated(string certName)
        {
            if (log != null)
            {
                string CertificateCreated =
                    AuditEvent.CertificateCreated;
                string message = String.Format(CertificateCreated,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificateCreated));
            }
        }

        public static void CertificateFailed(string certName)
        {
            if (log != null)
            {
                string CertificateFailed =
                    AuditEvent.CertificateFailed;
                string message = String.Format(CertificateFailed,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificateFailed));
            }
        }

        public static void CertificatePasswordCreated(string certName)
        {
            if (log != null)
            {
                string CertificatePasswordCreated =
                    AuditEvent.CertificatePasswordCreated;
                string message = String.Format(CertificatePasswordCreated,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificatePasswordCreated));
            }
        }

        public static void CertificatePasswordFailed(string certName)
        {
            if (log != null)
            {
                string CertificatePasswordFailed =
                    AuditEvent.CertificatePasswordFailed;
                string message = String.Format(CertificatePasswordFailed,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificatePasswordFailed));
            }
        }

        public static void CertificateRevoked(string certName)
        {
            if (log != null)
            {
                string CertificateRevoked =
                    AuditEvent.CertificateRevoked;
                string message = String.Format(CertificateRevoked,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificateRevoked));
            }
        }

        public static void CertificateRevokeFailed(string certName)
        {
            if (log != null)
            {
                string CertificateRevokeFailed =
                    AuditEvent.CertificateRevokeFailed;
                string message = String.Format(CertificateRevokeFailed,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificateRevokeFailed));
            }
        }

        public static void ClientConnectionClosed(string certName)
        {
            if (log != null)
            {
                string ClientConnectionClosed =
                    AuditEvent.ClientConnectionClosed;
                string message = String.Format(ClientConnectionClosed,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.ClientConnectionClosed));
            }
        }

        public static void ServerConnectionClosed(string certName)
        {
            if (log != null)
            {
                string ServerConnectionClosed =
                    AuditEvent.ServerConnectionClosed;
                string message = String.Format(ServerConnectionClosed,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.ServerConnectionClosed));
            }
        }

    }
}
