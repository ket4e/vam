using System.Collections;
using System.Reflection;

namespace System.ComponentModel.Design;

public class DesigntimeLicenseContext : LicenseContext
{
	internal Hashtable keys = new Hashtable();

	public override LicenseUsageMode UsageMode => LicenseUsageMode.Designtime;

	public override string GetSavedLicenseKey(Type type, Assembly resourceAssembly)
	{
		return (string)keys[type];
	}

	public override void SetSavedLicenseKey(Type type, string key)
	{
		keys[type] = key;
	}
}
