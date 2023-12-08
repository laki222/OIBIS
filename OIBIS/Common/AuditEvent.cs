using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public enum AuditEventTypes
	{
		CertificateCreated = 0,
		CertificateFailed = 1,
		CertificatePasswordCreated = 2,
		CertificatePasswordFailed = 3,
		CertificateRevoked = 4,
		CertificateRevokeFailed = 5,
		ClientConnectionClosed = 6,
		ServerConnectionClosed = 7	
	}
	public class AuditEvent
    {
		private static ResourceManager resourceManager = null;
		private static object resourceLock = new object();

		private static ResourceManager ResourceMgr
		{
			get
			{
				lock (resourceLock)
				{
					if (resourceManager == null)
					{
						resourceManager = new ResourceManager
							(typeof(EventResource).ToString(),
							Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}

		public static string CertificateCreated
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.CertificateCreated.ToString());
			}
		}

		public static string CertificateFailed
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.CertificateFailed.ToString());
			}
		}

		public static string CertificatePasswordCreated
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.CertificatePasswordCreated.ToString());
			}
		}

		public static string CertificatePasswordFailed
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.CertificatePasswordFailed.ToString());
			}
		}

		public static string CertificateRevoked
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.CertificateRevoked.ToString());
			}
		}

		public static string CertificateRevokeFailed
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.CertificateRevokeFailed.ToString());
			}
		}

		public static string ClientConnectionClosed
		{
			get
			{
				return ResourceMgr.GetString(AuditEventTypes.ClientConnectionClosed.ToString());
			}
		}

		public static string ServerConnectionClosed
		{
			get
			{	
				return ResourceMgr.GetString(AuditEventTypes.ServerConnectionClosed.ToString());
			}
		}
	}
}
