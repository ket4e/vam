namespace System.Security.Permissions;

[Serializable]
public sealed class StorePermission : CodeAccessPermission, IUnrestrictedPermission
{
	private const int version = 1;

	private StorePermissionFlags _flags;

	public StorePermissionFlags Flags
	{
		get
		{
			return _flags;
		}
		set
		{
			if (value != 0 && (value & StorePermissionFlags.AllFlags) == 0)
			{
				string message = string.Format(global::Locale.GetText("Invalid enum {0}"), value);
				throw new ArgumentException(message, "StorePermissionFlags");
			}
			_flags = value;
		}
	}

	public StorePermission(PermissionState state)
	{
		if (System.Security.Permissions.PermissionHelper.CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			_flags = StorePermissionFlags.AllFlags;
		}
		else
		{
			_flags = StorePermissionFlags.NoFlags;
		}
	}

	public StorePermission(StorePermissionFlags flags)
	{
		Flags = flags;
	}

	public bool IsUnrestricted()
	{
		return _flags == StorePermissionFlags.AllFlags;
	}

	public override IPermission Copy()
	{
		if (_flags == StorePermissionFlags.NoFlags)
		{
			return null;
		}
		return new StorePermission(_flags);
	}

	public override IPermission Intersect(IPermission target)
	{
		StorePermission storePermission = Cast(target);
		if (storePermission == null)
		{
			return null;
		}
		if (IsUnrestricted() && storePermission.IsUnrestricted())
		{
			return new StorePermission(PermissionState.Unrestricted);
		}
		if (IsUnrestricted())
		{
			return storePermission.Copy();
		}
		if (storePermission.IsUnrestricted())
		{
			return Copy();
		}
		StorePermissionFlags storePermissionFlags = _flags & storePermission._flags;
		if (storePermissionFlags == StorePermissionFlags.NoFlags)
		{
			return null;
		}
		return new StorePermission(storePermissionFlags);
	}

	public override IPermission Union(IPermission target)
	{
		StorePermission storePermission = Cast(target);
		if (storePermission == null)
		{
			return Copy();
		}
		if (IsUnrestricted() || storePermission.IsUnrestricted())
		{
			return new StorePermission(PermissionState.Unrestricted);
		}
		StorePermissionFlags storePermissionFlags = _flags | storePermission._flags;
		if (storePermissionFlags == StorePermissionFlags.NoFlags)
		{
			return null;
		}
		return new StorePermission(storePermissionFlags);
	}

	public override bool IsSubsetOf(IPermission target)
	{
		StorePermission storePermission = Cast(target);
		if (storePermission == null)
		{
			return _flags == StorePermissionFlags.NoFlags;
		}
		if (storePermission.IsUnrestricted())
		{
			return true;
		}
		if (IsUnrestricted())
		{
			return false;
		}
		return (_flags & ~storePermission._flags) == 0;
	}

	public override void FromXml(SecurityElement e)
	{
		System.Security.Permissions.PermissionHelper.CheckSecurityElement(e, "e", 1, 1);
		string text = e.Attribute("Flags");
		if (text == null)
		{
			_flags = StorePermissionFlags.NoFlags;
		}
		else
		{
			_flags = (StorePermissionFlags)(int)Enum.Parse(typeof(StorePermissionFlags), text);
		}
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = System.Security.Permissions.PermissionHelper.Element(typeof(StorePermission), 1);
		if (IsUnrestricted())
		{
			securityElement.AddAttribute("Unrestricted", bool.TrueString);
		}
		else
		{
			securityElement.AddAttribute("Flags", _flags.ToString());
		}
		return securityElement;
	}

	private StorePermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		StorePermission storePermission = target as StorePermission;
		if (storePermission == null)
		{
			System.Security.Permissions.PermissionHelper.ThrowInvalidPermission(target, typeof(StorePermission));
		}
		return storePermission;
	}
}
