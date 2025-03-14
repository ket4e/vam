namespace System.Security.Permissions;

[Serializable]
public sealed class DataProtectionPermission : CodeAccessPermission, IUnrestrictedPermission
{
	private const int version = 1;

	private DataProtectionPermissionFlags _flags;

	public DataProtectionPermissionFlags Flags
	{
		get
		{
			return _flags;
		}
		set
		{
			if (((uint)value & 0xFFFFFFF0u) != 0)
			{
				string message = string.Format(global::Locale.GetText("Invalid enum {0}"), value);
				throw new ArgumentException(message, "DataProtectionPermissionFlags");
			}
			_flags = value;
		}
	}

	public DataProtectionPermission(PermissionState state)
	{
		if (System.Security.Permissions.PermissionHelper.CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			_flags = DataProtectionPermissionFlags.AllFlags;
		}
	}

	public DataProtectionPermission(DataProtectionPermissionFlags flags)
	{
		Flags = flags;
	}

	public bool IsUnrestricted()
	{
		return _flags == DataProtectionPermissionFlags.AllFlags;
	}

	public override IPermission Copy()
	{
		return new DataProtectionPermission(_flags);
	}

	public override IPermission Intersect(IPermission target)
	{
		DataProtectionPermission dataProtectionPermission = Cast(target);
		if (dataProtectionPermission == null)
		{
			return null;
		}
		if (IsUnrestricted() && dataProtectionPermission.IsUnrestricted())
		{
			return new DataProtectionPermission(PermissionState.Unrestricted);
		}
		if (IsUnrestricted())
		{
			return dataProtectionPermission.Copy();
		}
		if (dataProtectionPermission.IsUnrestricted())
		{
			return Copy();
		}
		return new DataProtectionPermission(_flags & dataProtectionPermission._flags);
	}

	public override IPermission Union(IPermission target)
	{
		DataProtectionPermission dataProtectionPermission = Cast(target);
		if (dataProtectionPermission == null)
		{
			return Copy();
		}
		if (IsUnrestricted() || dataProtectionPermission.IsUnrestricted())
		{
			return new SecurityPermission(PermissionState.Unrestricted);
		}
		return new DataProtectionPermission(_flags | dataProtectionPermission._flags);
	}

	public override bool IsSubsetOf(IPermission target)
	{
		DataProtectionPermission dataProtectionPermission = Cast(target);
		if (dataProtectionPermission == null)
		{
			return _flags == DataProtectionPermissionFlags.NoFlags;
		}
		if (dataProtectionPermission.IsUnrestricted())
		{
			return true;
		}
		if (IsUnrestricted())
		{
			return false;
		}
		return (_flags & ~dataProtectionPermission._flags) == 0;
	}

	public override void FromXml(SecurityElement e)
	{
		System.Security.Permissions.PermissionHelper.CheckSecurityElement(e, "e", 1, 1);
		_flags = (DataProtectionPermissionFlags)(int)Enum.Parse(typeof(DataProtectionPermissionFlags), e.Attribute("Flags"));
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = System.Security.Permissions.PermissionHelper.Element(typeof(DataProtectionPermission), 1);
		securityElement.AddAttribute("Flags", _flags.ToString());
		return securityElement;
	}

	private DataProtectionPermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		DataProtectionPermission dataProtectionPermission = target as DataProtectionPermission;
		if (dataProtectionPermission == null)
		{
			System.Security.Permissions.PermissionHelper.ThrowInvalidPermission(target, typeof(DataProtectionPermission));
		}
		return dataProtectionPermission;
	}
}
