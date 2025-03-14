using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy;

[ComVisible(true)]
public static class ApplicationSecurityManager
{
	private const string config = "ApplicationTrust.config";

	private static IApplicationTrustManager _appTrustManager;

	private static ApplicationTrustCollection _userAppTrusts;

	public static IApplicationTrustManager ApplicationTrustManager
	{
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		get
		{
			if (_appTrustManager == null)
			{
				_appTrustManager = new MonoTrustManager();
			}
			return _appTrustManager;
		}
	}

	public static ApplicationTrustCollection UserApplicationTrusts
	{
		get
		{
			if (_userAppTrusts == null)
			{
				_userAppTrusts = new ApplicationTrustCollection();
			}
			return _userAppTrusts;
		}
	}

	[MonoTODO("Missing application manifest support")]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
	public static bool DetermineApplicationTrust(ActivationContext activationContext, TrustManagerContext context)
	{
		if (activationContext == null)
		{
			throw new NullReferenceException("activationContext");
		}
		ApplicationTrust applicationTrust = ApplicationTrustManager.DetermineApplicationTrust(activationContext, context);
		return applicationTrust.IsApplicationTrustedToRun;
	}
}
