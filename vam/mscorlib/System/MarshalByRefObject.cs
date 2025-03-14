using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;

namespace System;

[Serializable]
[ComVisible(true)]
public abstract class MarshalByRefObject
{
	[NonSerialized]
	private ServerIdentity _identity;

	internal ServerIdentity ObjectIdentity
	{
		get
		{
			return _identity;
		}
		set
		{
			_identity = value;
		}
	}

	internal Identity GetObjectIdentity(MarshalByRefObject obj, out bool IsClient)
	{
		IsClient = false;
		Identity identity = null;
		if (RemotingServices.IsTransparentProxy(obj))
		{
			identity = RemotingServices.GetRealProxy(obj).ObjectIdentity;
			IsClient = true;
		}
		else
		{
			identity = obj.ObjectIdentity;
		}
		return identity;
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
	public virtual ObjRef CreateObjRef(Type requestedType)
	{
		if (_identity == null)
		{
			throw new RemotingException(Locale.GetText("No remoting information was found for the object."));
		}
		return _identity.CreateObjRef(requestedType);
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
	public object GetLifetimeService()
	{
		if (_identity == null)
		{
			return null;
		}
		return _identity.Lease;
	}

	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
	public virtual object InitializeLifetimeService()
	{
		if (_identity != null && _identity.Lease != null)
		{
			return _identity.Lease;
		}
		return new Lease();
	}

	protected MarshalByRefObject MemberwiseClone(bool cloneIdentity)
	{
		MarshalByRefObject marshalByRefObject = (MarshalByRefObject)MemberwiseClone();
		if (!cloneIdentity)
		{
			marshalByRefObject._identity = null;
		}
		return marshalByRefObject;
	}
}
