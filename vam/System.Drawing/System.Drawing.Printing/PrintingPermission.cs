using System.Globalization;
using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing;

[Serializable]
public sealed class PrintingPermission : CodeAccessPermission, IUnrestrictedPermission
{
	private const int version = 1;

	private PrintingPermissionLevel _Level;

	public PrintingPermissionLevel Level
	{
		get
		{
			return _Level;
		}
		set
		{
			if (!Enum.IsDefined(typeof(PrintingPermissionLevel), value))
			{
				string text = global::Locale.GetText("Invalid enum {0}");
				throw new ArgumentException(string.Format(text, value), "Level");
			}
			_Level = value;
		}
	}

	public PrintingPermission(PermissionState state)
	{
		if (CheckPermissionState(state, allowUnrestricted: true) == PermissionState.Unrestricted)
		{
			_Level = PrintingPermissionLevel.AllPrinting;
		}
	}

	public PrintingPermission(PrintingPermissionLevel printingLevel)
	{
		Level = printingLevel;
	}

	public override IPermission Copy()
	{
		return new PrintingPermission(Level);
	}

	public override void FromXml(SecurityElement esd)
	{
		CheckSecurityElement(esd, "esd", 1, 1);
		if (IsUnrestricted(esd))
		{
			_Level = PrintingPermissionLevel.AllPrinting;
			return;
		}
		string text = esd.Attribute("Level");
		if (text != null)
		{
			_Level = (PrintingPermissionLevel)(int)Enum.Parse(typeof(PrintingPermissionLevel), text);
		}
		else
		{
			_Level = PrintingPermissionLevel.NoPrinting;
		}
	}

	public override IPermission Intersect(IPermission target)
	{
		PrintingPermission printingPermission = Cast(target);
		if (printingPermission == null || IsEmpty() || printingPermission.IsEmpty())
		{
			return null;
		}
		PrintingPermissionLevel printingLevel = ((_Level > printingPermission.Level) ? printingPermission.Level : _Level);
		return new PrintingPermission(printingLevel);
	}

	public override bool IsSubsetOf(IPermission target)
	{
		PrintingPermission printingPermission = Cast(target);
		if (printingPermission == null)
		{
			return IsEmpty();
		}
		return _Level <= printingPermission.Level;
	}

	public bool IsUnrestricted()
	{
		return _Level == PrintingPermissionLevel.AllPrinting;
	}

	public override SecurityElement ToXml()
	{
		SecurityElement securityElement = Element(1);
		if (IsUnrestricted())
		{
			securityElement.AddAttribute("Unrestricted", "true");
		}
		else
		{
			securityElement.AddAttribute("Level", _Level.ToString());
		}
		return securityElement;
	}

	public override IPermission Union(IPermission target)
	{
		PrintingPermission printingPermission = Cast(target);
		if (printingPermission == null)
		{
			return new PrintingPermission(_Level);
		}
		if (IsUnrestricted() || printingPermission.IsUnrestricted())
		{
			return new PrintingPermission(PermissionState.Unrestricted);
		}
		if (IsEmpty() && printingPermission.IsEmpty())
		{
			return null;
		}
		PrintingPermissionLevel printingLevel = ((_Level <= printingPermission.Level) ? printingPermission.Level : _Level);
		return new PrintingPermission(printingLevel);
	}

	private bool IsEmpty()
	{
		return _Level == PrintingPermissionLevel.NoPrinting;
	}

	private PrintingPermission Cast(IPermission target)
	{
		if (target == null)
		{
			return null;
		}
		PrintingPermission printingPermission = target as PrintingPermission;
		if (printingPermission == null)
		{
			ThrowInvalidPermission(target, typeof(PrintingPermission));
		}
		return printingPermission;
	}

	internal SecurityElement Element(int version)
	{
		SecurityElement securityElement = new SecurityElement("IPermission");
		Type type = GetType();
		securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
		securityElement.AddAttribute("version", version.ToString());
		return securityElement;
	}

	internal static PermissionState CheckPermissionState(PermissionState state, bool allowUnrestricted)
	{
		switch (state)
		{
		case PermissionState.Unrestricted:
			if (!allowUnrestricted)
			{
				string message = global::Locale.GetText("Unrestricted isn't not allowed for identity permissions.");
				throw new ArgumentException(message, "state");
			}
			break;
		default:
		{
			string message = string.Format(global::Locale.GetText("Invalid enum {0}"), state);
			throw new ArgumentException(message, "state");
		}
		case PermissionState.None:
			break;
		}
		return state;
	}

	internal static int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
	{
		if (se == null)
		{
			throw new ArgumentNullException(parameterName);
		}
		string text = se.Attribute("class");
		if (text == null)
		{
			string text2 = global::Locale.GetText("Missing 'class' attribute.");
			throw new ArgumentException(text2, parameterName);
		}
		int num = minimumVersion;
		string text3 = se.Attribute("version");
		if (text3 != null)
		{
			try
			{
				num = int.Parse(text3);
			}
			catch (Exception innerException)
			{
				string text4 = global::Locale.GetText("Couldn't parse version from '{0}'.");
				text4 = string.Format(text4, text3);
				throw new ArgumentException(text4, parameterName, innerException);
			}
		}
		if (num < minimumVersion || num > maximumVersion)
		{
			string text5 = global::Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}'].");
			text5 = string.Format(text5, num, minimumVersion, maximumVersion);
			throw new ArgumentException(text5, parameterName);
		}
		return num;
	}

	internal static bool IsUnrestricted(SecurityElement se)
	{
		string text = se.Attribute("Unrestricted");
		if (text == null)
		{
			return false;
		}
		return string.Compare(text, bool.TrueString, ignoreCase: true, CultureInfo.InvariantCulture) == 0;
	}

	internal static void ThrowInvalidPermission(IPermission target, Type expected)
	{
		string text = global::Locale.GetText("Invalid permission type '{0}', expected type '{1}'.");
		text = string.Format(text, target.GetType(), expected);
		throw new ArgumentException(text, "target");
	}
}
