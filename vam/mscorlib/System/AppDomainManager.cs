using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System;

[ComVisible(true)]
[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
public class AppDomainManager : MarshalByRefObject
{
	private ApplicationActivator _activator;

	private AppDomainManagerInitializationOptions _flags;

	public virtual ApplicationActivator ApplicationActivator
	{
		get
		{
			if (_activator == null)
			{
				_activator = new ApplicationActivator();
			}
			return _activator;
		}
	}

	public virtual Assembly EntryAssembly => Assembly.GetEntryAssembly();

	[MonoTODO]
	public virtual HostExecutionContextManager HostExecutionContextManager
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public virtual HostSecurityManager HostSecurityManager => null;

	public AppDomainManagerInitializationOptions InitializationFlags
	{
		get
		{
			return _flags;
		}
		set
		{
			_flags = value;
		}
	}

	public AppDomainManager()
	{
		_flags = AppDomainManagerInitializationOptions.None;
	}

	public virtual AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
	{
		InitializeNewDomain(appDomainInfo);
		AppDomain appDomain = CreateDomainHelper(friendlyName, securityInfo, appDomainInfo);
		if ((HostSecurityManager.Flags & HostSecurityManagerOptions.HostPolicyLevel) == HostSecurityManagerOptions.HostPolicyLevel)
		{
			PolicyLevel domainPolicy = HostSecurityManager.DomainPolicy;
			if (domainPolicy != null)
			{
				appDomain.SetAppDomainPolicy(domainPolicy);
			}
		}
		return appDomain;
	}

	public virtual void InitializeNewDomain(AppDomainSetup appDomainInfo)
	{
	}

	public virtual bool CheckSecuritySettings(SecurityState state)
	{
		return false;
	}

	protected static AppDomain CreateDomainHelper(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
	{
		return AppDomain.CreateDomain(friendlyName, securityInfo, appDomainInfo);
	}
}
