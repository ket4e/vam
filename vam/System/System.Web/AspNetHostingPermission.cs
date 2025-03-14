using System.Security;
using System.Security.Permissions;

namespace System.Web;

[Serializable]
public sealed class AspNetHostingPermission : CodeAccessPermission, IUnrestrictedPermission
{
	private const int version = 1;

	private AspNetHostingPermissionLevel _level;

	public AspNetHostingPermissionLevel Level
	{
		get
		{
			return _level;
		}
		set
		{
			if (value < AspNetHostingPermissionLevel.None || value > AspNetHostingPermissionLevel.Unrestricted)
			{
				string text = global::Locale.GetText("Invalid enum {0}.");
				throw new ArgumentException(string.Format(text, value), "Level");
			}
			_level = value;
		}
	}

	public AspNetHostingPermission(AspNetHostingPermissionLevel level)
	{
		Level = level;
	}

	public AspNetHostingPermission(PermissionState state)
	{
		if (System.Security.Permissions.PermissionHelper.CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			_level = AspNetHostingPermissionLevel.Unrestricted;
		}
		else
		{
			_level = AspNetHostingPermissionLevel.None;
		}
	}

	public bool IsUnrestricted()
	{
		return _level == AspNetHostingPermissionLevel.Unrestricted;
	}

	public override IPermission Copy()
	{
		return new AspNetHostingPermission(_level);
	}

	public override void FromXml(SecurityElement securityElement)
	{
		System.Security.Permissions.PermissionHelper.CheckSecurityElement(securityElement, "securityElement", 1, 1);
		if (securityElement.Tag != "IPermission")
		{
			string text = global::Locale.GetText("Invalid tag '{0}' for permission.");
			throw new ArgumentException(string.Format(text, securityElement.Tag), "securityElement");
		}
		if (securityElement.Attribute("version") == null)
		{
			string text2 = global::Locale.GetText("Missing version attribute.");
			throw new ArgumentException(text2, "securityElement");
		}
		if (System.Security.Permissions.PermissionHelper.IsUnrestricted(securityElement))
		{
			_level = AspNetHostingPermissionLevel.Unrestricted;
			return;
		}
		string text3 = securityElement.Attribute("Level");
		if (text3 != null)
		{
			_level = (AspNetHostingPermissionLevel)(int)Enum.Parse(typeof(AspNetHostingPermissionLevel), text3);
		}
		else
		{
			_level = AspNetHostingPermissionLevel.None;
		}
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = System.Security.Permissions.PermissionHelper.Element(typeof(AspNetHostingPermission), 1);
		if (IsUnrestricted())
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		securityElement.AddAttribute("Level", _level.ToString());
		return securityElement;
	}

	public override IPermission Intersect(IPermission target)
	{
		AspNetHostingPermission aspNetHostingPermission = Cast(target);
		if (aspNetHostingPermission == null)
		{
			return null;
		}
		return new AspNetHostingPermission((_level > aspNetHostingPermission.Level) ? aspNetHostingPermission.Level : _level);
	}

	public override bool IsSubsetOf(IPermission target)
	{
		AspNetHostingPermission aspNetHostingPermission = Cast(target);
		if (aspNetHostingPermission == null)
		{
			return IsEmpty();
		}
		return _level <= aspNetHostingPermission._level;
	}

	public override IPermission Union(IPermission target)
	{
		AspNetHostingPermission aspNetHostingPermission = Cast(target);
		if (aspNetHostingPermission == null)
		{
			return Copy();
		}
		return new AspNetHostingPermission((_level <= aspNetHostingPermission.Level) ? aspNetHostingPermission.Level : _level);
	}

	private bool IsEmpty()
	{
		return _level == AspNetHostingPermissionLevel.None;
	}

	private AspNetHostingPermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		AspNetHostingPermission aspNetHostingPermission = target as AspNetHostingPermission;
		if (aspNetHostingPermission == null)
		{
			System.Security.Permissions.PermissionHelper.ThrowInvalidPermission(target, typeof(AspNetHostingPermission));
		}
		return aspNetHostingPermission;
	}
}
