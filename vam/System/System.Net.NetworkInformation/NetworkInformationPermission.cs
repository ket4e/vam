using System.Security;
using System.Security.Permissions;

namespace System.Net.NetworkInformation;

[Serializable]
public sealed class NetworkInformationPermission : CodeAccessPermission, IUnrestrictedPermission
{
	private const int version = 1;

	[System.MonoTODO]
	public NetworkInformationAccess Access => NetworkInformationAccess.None;

	[System.MonoTODO]
	public NetworkInformationPermission(PermissionState state)
	{
	}

	[System.MonoTODO]
	public NetworkInformationPermission(NetworkInformationAccess access)
	{
	}

	[System.MonoTODO]
	public void AddPermission(NetworkInformationAccess access)
	{
	}

	[System.MonoTODO]
	public override IPermission Copy()
	{
		return null;
	}

	[System.MonoTODO]
	public override void FromXml(SecurityElement securityElement)
	{
	}

	[System.MonoTODO]
	public override IPermission Intersect(IPermission target)
	{
		return null;
	}

	[System.MonoTODO]
	public override bool IsSubsetOf(IPermission target)
	{
		return false;
	}

	[System.MonoTODO]
	public bool IsUnrestricted()
	{
		return false;
	}

	[System.MonoTODO]
	public override SecurityElement ToXml()
	{
		return System.Security.Permissions.PermissionHelper.Element(typeof(NetworkInformationPermission), 1);
	}

	[System.MonoTODO]
	public override IPermission Union(IPermission target)
	{
		return null;
	}
}
