using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Audit : IDisposable
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

        public static void ClientConnectionOpen(string certName)
        {
            if (log != null)
            {
                string ClientConnectionOpen =
                    AuditEvent.ClientConnectionOpen;
                string message = String.Format(ClientConnectionOpen,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.ClientConnectionOpen));
            }
        }

        public static void ServerConnectionOpen(string certName)
        {
            if (log != null)
            {
                string ServerConnectionOpen =
                    AuditEvent.ServerConnectionOpen;
                string message = String.Format(ServerConnectionOpen,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.ServerConnectionOpen));
            }
        }

        public static void CertificateReplicated(string certName)
        {
            if (log != null)
            {
                string CertificateReplicated =
                    AuditEvent.CertificateReplicated;
                string message = String.Format(CertificateReplicated,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificateReplicated));
            }
        }

        public static void CertificateRevokedReplicated(string certName)
        {
            if (log != null)
            {
                string CertificateRevokedReplicated =
                    AuditEvent.CertificateRevokedReplicated;
                string message = String.Format(CertificateRevokedReplicated,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificateRevokedReplicated));
            }
        }

        public static void CertificateWithPvkInstallationSuccess(string certName)
        {
            if (log != null)
            {
                string CertificateWithPvkInstallationSuccess =
                    AuditEvent.CertificateWithPvkInstallationSuccess;
                string message = String.Format(CertificateWithPvkInstallationSuccess,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificateWithPvkInstallationSuccess));
            }
        }

        public static void CertificateWithoutPvkInstallationSuccess(string certName)
        {
            if (log != null)
            {
                string CertificateWithoutPvkInstallationSuccess =
                    AuditEvent.CertificateWithoutPvkInstallationSuccess;
                string message = String.Format(CertificateWithoutPvkInstallationSuccess,
                    certName);
                log.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.CertificateWithoutPvkInstallationSuccess));
            }
        }

        public void Dispose()
        {
            if (log != null)
            {
                log.Dispose();
                log = null;
            }
        }
    }
}
