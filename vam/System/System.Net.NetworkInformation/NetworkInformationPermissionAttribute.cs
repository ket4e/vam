using System.Security;
using System.Security.Permissions;

namespace System.Net.NetworkInformation;

[Serializable]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class NetworkInformationPermissionAttribute : CodeAccessSecurityAttribute
{
	private string access;

	public string Access
	{
		get
		{
			return access;
		}
		set
		{
			switch (access)
			{
			default:
				throw new ArgumentException("Only 'Read', 'Full' and 'None' are allowed");
			case "Read":
			case "Full":
			case "None":
				access = value;
				break;
			}
		}
	}

	public NetworkInformationPermissionAttribute(SecurityAction action)
		: base(action)
	{
	}

	[System.MonoTODO("verify implementation")]
	public override IPermission CreatePermission()
	{
		NetworkInformationAccess networkInformationAccess = NetworkInformationAccess.None;
		switch (Access)
		{
		case "Read":
			networkInformationAccess = NetworkInformationAccess.Read;
			break;
		case "Full":
			networkInformationAccess = NetworkInformationAccess.Read | NetworkInformationAccess.Ping;
			break;
		}
		return new NetworkInformationPermission(networkInformationAccess);
	}
}
