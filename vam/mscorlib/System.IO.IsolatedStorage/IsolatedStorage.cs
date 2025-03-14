using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.IO.IsolatedStorage;

[ComVisible(true)]
public abstract class IsolatedStorage : MarshalByRefObject
{
	internal IsolatedStorageScope storage_scope;

	internal object _assemblyIdentity;

	internal object _domainIdentity;

	internal object _applicationIdentity;

	[MonoTODO("requires manifest support")]
	[ComVisible(false)]
	public object ApplicationIdentity
	{
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		get
		{
			if ((storage_scope & IsolatedStorageScope.Application) == 0)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid Isolation Scope."));
			}
			if (_applicationIdentity == null)
			{
				throw new InvalidOperationException(Locale.GetText("Identity unavailable."));
			}
			throw new NotImplementedException(Locale.GetText("CAS related"));
		}
	}

	public object AssemblyIdentity
	{
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		get
		{
			if ((storage_scope & IsolatedStorageScope.Assembly) == 0)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid Isolation Scope."));
			}
			if (_assemblyIdentity == null)
			{
				throw new InvalidOperationException(Locale.GetText("Identity unavailable."));
			}
			return _assemblyIdentity;
		}
	}

	[CLSCompliant(false)]
	public virtual ulong CurrentSize
	{
		get
		{
			throw new InvalidOperationException(Locale.GetText("IsolatedStorage does not have a preset CurrentSize."));
		}
	}

	public object DomainIdentity
	{
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		get
		{
			if ((storage_scope & IsolatedStorageScope.Domain) == 0)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid Isolation Scope."));
			}
			if (_domainIdentity == null)
			{
				throw new InvalidOperationException(Locale.GetText("Identity unavailable."));
			}
			return _domainIdentity;
		}
	}

	[CLSCompliant(false)]
	public virtual ulong MaximumSize
	{
		get
		{
			throw new InvalidOperationException(Locale.GetText("IsolatedStorage does not have a preset MaximumSize."));
		}
	}

	public IsolatedStorageScope Scope => storage_scope;

	protected virtual char SeparatorExternal => Path.DirectorySeparatorChar;

	protected virtual char SeparatorInternal => '.';

	protected abstract IsolatedStoragePermission GetPermission(PermissionSet ps);

	protected void InitStore(IsolatedStorageScope scope, Type domainEvidenceType, Type assemblyEvidenceType)
	{
		switch (scope)
		{
		case IsolatedStorageScope.User | IsolatedStorageScope.Assembly:
		case IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly:
			throw new NotImplementedException(scope.ToString());
		default:
			throw new ArgumentException(scope.ToString());
		}
	}

	[MonoTODO("requires manifest support")]
	protected void InitStore(IsolatedStorageScope scope, Type appEvidenceType)
	{
		if (AppDomain.CurrentDomain.ApplicationIdentity == null)
		{
			throw new IsolatedStorageException(Locale.GetText("No ApplicationIdentity available for AppDomain."));
		}
		if (appEvidenceType == null)
		{
		}
		storage_scope = scope;
	}

	public abstract void Remove();
}
