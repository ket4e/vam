using System.Collections;

namespace System.Security.Permissions;

[Serializable]
public abstract class ResourcePermissionBase : CodeAccessPermission, IUnrestrictedPermission
{
	private const int version = 1;

	public const string Any = "*";

	public const string Local = ".";

	private ArrayList _list;

	private bool _unrestricted;

	private Type _type;

	private string[] _tags;

	private static char[] invalidChars = new char[8] { '\t', '\n', '\v', '\f', '\r', ' ', '\\', 'Š' };

	protected Type PermissionAccessType
	{
		get
		{
			return _type;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("PermissionAccessType");
			}
			if (!value.IsEnum)
			{
				throw new ArgumentException("!Enum", "PermissionAccessType");
			}
			_type = value;
		}
	}

	protected string[] TagNames
	{
		get
		{
			return _tags;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("TagNames");
			}
			if (value.Length == 0)
			{
				throw new ArgumentException("Length==0", "TagNames");
			}
			_tags = value;
		}
	}

	protected ResourcePermissionBase()
	{
		_list = new ArrayList();
	}

	protected ResourcePermissionBase(PermissionState state)
		: this()
	{
		System.Security.Permissions.PermissionHelper.CheckPermissionState(state, allowUnrestricted: true);
		_unrestricted = state == PermissionState.Unrestricted;
	}

	protected void AddPermissionAccess(ResourcePermissionBaseEntry entry)
	{
		CheckEntry(entry);
		if (Exists(entry))
		{
			string text = global::Locale.GetText("Entry already exists.");
			throw new InvalidOperationException(text);
		}
		_list.Add(entry);
	}

	protected void Clear()
	{
		_list.Clear();
	}

	public override IPermission Copy()
	{
		ResourcePermissionBase resourcePermissionBase = CreateFromType(GetType(), _unrestricted);
		if (_tags != null)
		{
			resourcePermissionBase._tags = (string[])_tags.Clone();
		}
		resourcePermissionBase._type = _type;
		resourcePermissionBase._list.AddRange(_list);
		return resourcePermissionBase;
	}

	[System.MonoTODO("incomplete - need more test")]
	public override void FromXml(SecurityElement securityElement)
	{
		if (securityElement == null)
		{
			throw new ArgumentNullException("securityElement");
		}
		CheckSecurityElement(securityElement, "securityElement", 1, 1);
		_list.Clear();
		_unrestricted = System.Security.Permissions.PermissionHelper.IsUnrestricted(securityElement);
		if (securityElement.Children == null || securityElement.Children.Count < 1)
		{
			return;
		}
		string[] array = new string[1];
		foreach (SecurityElement child in securityElement.Children)
		{
			array[0] = child.Attribute("name");
			int permissionAccess = (int)Enum.Parse(PermissionAccessType, child.Attribute("access"));
			ResourcePermissionBaseEntry entry = new ResourcePermissionBaseEntry(permissionAccess, array);
			AddPermissionAccess(entry);
		}
	}

	protected ResourcePermissionBaseEntry[] GetPermissionEntries()
	{
		ResourcePermissionBaseEntry[] array = new ResourcePermissionBaseEntry[_list.Count];
		_list.CopyTo(array, 0);
		return array;
	}

	public override IPermission Intersect(IPermission target)
	{
		ResourcePermissionBase resourcePermissionBase = Cast(target);
		if (resourcePermissionBase == null)
		{
			return null;
		}
		bool flag = IsUnrestricted();
		bool flag2 = resourcePermissionBase.IsUnrestricted();
		if (IsEmpty() && !flag2)
		{
			return null;
		}
		if (resourcePermissionBase.IsEmpty() && !flag)
		{
			return null;
		}
		ResourcePermissionBase resourcePermissionBase2 = CreateFromType(GetType(), flag && flag2);
		foreach (ResourcePermissionBaseEntry item in _list)
		{
			if (flag2 || resourcePermissionBase.Exists(item))
			{
				resourcePermissionBase2.AddPermissionAccess(item);
			}
		}
		foreach (ResourcePermissionBaseEntry item2 in resourcePermissionBase._list)
		{
			if ((flag || Exists(item2)) && !resourcePermissionBase2.Exists(item2))
			{
				resourcePermissionBase2.AddPermissionAccess(item2);
			}
		}
		return resourcePermissionBase2;
	}

	public override bool IsSubsetOf(IPermission target)
	{
		if (target == null)
		{
			return true;
		}
		if (!(target is ResourcePermissionBase resourcePermissionBase))
		{
			return false;
		}
		if (resourcePermissionBase.IsUnrestricted())
		{
			return true;
		}
		if (IsUnrestricted())
		{
			return resourcePermissionBase.IsUnrestricted();
		}
		foreach (ResourcePermissionBaseEntry item in _list)
		{
			if (!resourcePermissionBase.Exists(item))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsUnrestricted()
	{
		return _unrestricted;
	}

	protected void RemovePermissionAccess(ResourcePermissionBaseEntry entry)
	{
		CheckEntry(entry);
		for (int i = 0; i < _list.Count; i++)
		{
			ResourcePermissionBaseEntry entry2 = (ResourcePermissionBaseEntry)_list[i];
			if (Equals(entry, entry2))
			{
				_list.RemoveAt(i);
				return;
			}
		}
		string text = global::Locale.GetText("Entry doesn't exists.");
		throw new InvalidOperationException(text);
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = System.Security.Permissions.PermissionHelper.Element(GetType(), 1);
		if (IsUnrestricted())
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		else
		{
			foreach (ResourcePermissionBaseEntry item in _list)
			{
				SecurityElement securityElement2 = securityElement;
				string text = null;
				if (PermissionAccessType != null)
				{
					text = Enum.Format(PermissionAccessType, item.PermissionAccess, "g");
				}
				for (int i = 0; i < _tags.Length; i++)
				{
					SecurityElement securityElement3 = new SecurityElement(_tags[i]);
					securityElement3.AddAttribute("name", item.PermissionAccessPath[i]);
					if (text != null)
					{
						securityElement3.AddAttribute("access", text);
					}
					securityElement2.AddChild(securityElement3);
					securityElement3 = securityElement2;
				}
			}
		}
		return securityElement;
	}

	public override IPermission Union(IPermission target)
	{
		ResourcePermissionBase resourcePermissionBase = Cast(target);
		if (resourcePermissionBase == null)
		{
			return Copy();
		}
		if (IsEmpty() && resourcePermissionBase.IsEmpty())
		{
			return null;
		}
		if (resourcePermissionBase.IsEmpty())
		{
			return Copy();
		}
		if (IsEmpty())
		{
			return resourcePermissionBase.Copy();
		}
		bool flag = IsUnrestricted() || resourcePermissionBase.IsUnrestricted();
		ResourcePermissionBase resourcePermissionBase2 = CreateFromType(GetType(), flag);
		if (!flag)
		{
			foreach (ResourcePermissionBaseEntry item in _list)
			{
				resourcePermissionBase2.AddPermissionAccess(item);
			}
			foreach (ResourcePermissionBaseEntry item2 in resourcePermissionBase._list)
			{
				if (!resourcePermissionBase2.Exists(item2))
				{
					resourcePermissionBase2.AddPermissionAccess(item2);
				}
			}
		}
		return resourcePermissionBase2;
	}

	private bool IsEmpty()
	{
		return !_unrestricted && _list.Count == 0;
	}

	private ResourcePermissionBase Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		ResourcePermissionBase resourcePermissionBase = target as ResourcePermissionBase;
		if (resourcePermissionBase == null)
		{
			System.Security.Permissions.PermissionHelper.ThrowInvalidPermission(target, typeof(ResourcePermissionBase));
		}
		return resourcePermissionBase;
	}

	internal void CheckEntry(ResourcePermissionBaseEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (entry.PermissionAccessPath == null || entry.PermissionAccessPath.Length != _tags.Length)
		{
			string text = global::Locale.GetText("Entry doesn't match TagNames");
			throw new InvalidOperationException(text);
		}
	}

	internal bool Equals(ResourcePermissionBaseEntry entry1, ResourcePermissionBaseEntry entry2)
	{
		if (entry1.PermissionAccess != entry2.PermissionAccess)
		{
			return false;
		}
		if (entry1.PermissionAccessPath.Length != entry2.PermissionAccessPath.Length)
		{
			return false;
		}
		for (int i = 0; i < entry1.PermissionAccessPath.Length; i++)
		{
			if (entry1.PermissionAccessPath[i] != entry2.PermissionAccessPath[i])
			{
				return false;
			}
		}
		return true;
	}

	internal bool Exists(ResourcePermissionBaseEntry entry)
	{
		if (_list.Count == 0)
		{
			return false;
		}
		foreach (ResourcePermissionBaseEntry item in _list)
		{
			if (Equals(item, entry))
			{
				return true;
			}
		}
		return false;
	}

	internal int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
	{
		if (se == null)
		{
			throw new ArgumentNullException(parameterName);
		}
		if (se.Tag != "IPermission")
		{
			string message = string.Format(global::Locale.GetText("Invalid tag {0}"), se.Tag);
			throw new ArgumentException(message, parameterName);
		}
		int num = minimumVersion;
		string text = se.Attribute("version");
		if (text != null)
		{
			try
			{
				num = int.Parse(text);
			}
			catch (Exception innerException)
			{
				string text2 = global::Locale.GetText("Couldn't parse version from '{0}'.");
				text2 = string.Format(text2, text);
				throw new ArgumentException(text2, parameterName, innerException);
			}
		}
		if (num < minimumVersion || num > maximumVersion)
		{
			string text3 = global::Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}'].");
			text3 = string.Format(text3, num, minimumVersion, maximumVersion);
			throw new ArgumentException(text3, parameterName);
		}
		return num;
	}

	internal static void ValidateMachineName(string name)
	{
		if (name == null || name.Length == 0 || name.IndexOfAny(invalidChars) != -1)
		{
			string text = global::Locale.GetText("Invalid machine name '{0}'.");
			if (name == null)
			{
				name = "(null)";
			}
			text = string.Format(text, name);
			throw new ArgumentException(text, "MachineName");
		}
	}

	internal static ResourcePermissionBase CreateFromType(Type type, bool unrestricted)
	{
		return (ResourcePermissionBase)Activator.CreateInstance(type, unrestricted ? PermissionState.Unrestricted : PermissionState.None);
	}
}
