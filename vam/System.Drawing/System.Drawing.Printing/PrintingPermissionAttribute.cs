using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class PrintingPermissionAttribute : CodeAccessSecurityAttribute
{
	private PrintingPermissionLevel _level;

	public PrintingPermissionLevel Level
	{
		get
		{
			return _level;
		}
		set
		{
			if (!Enum.IsDefined(typeof(PrintingPermissionLevel), value))
			{
				string text = global::Locale.GetText("Invalid enum {0}");
				throw new ArgumentException(string.Format(text, value), "Level");
			}
			_level = value;
		}
	}

	public PrintingPermissionAttribute(SecurityAction action)
		: base(action)
	{
	}

	public override IPermission CreatePermission()
	{
		if (base.Unrestricted)
		{
			return new PrintingPermission(PermissionState.Unrestricted);
		}
		return new PrintingPermission(_level);
	}
}
