using System.Runtime.InteropServices;

namespace System.Resources;

[AttributeUsage(AttributeTargets.Assembly)]
[ComVisible(true)]
public sealed class NeutralResourcesLanguageAttribute : Attribute
{
	private string culture;

	private UltimateResourceFallbackLocation loc;

	public string CultureName => culture;

	public UltimateResourceFallbackLocation Location => loc;

	public NeutralResourcesLanguageAttribute(string cultureName)
	{
		if (cultureName == null)
		{
			throw new ArgumentNullException("culture is null");
		}
		culture = cultureName;
	}

	public NeutralResourcesLanguageAttribute(string cultureName, UltimateResourceFallbackLocation location)
	{
		if (cultureName == null)
		{
			throw new ArgumentNullException("culture is null");
		}
		culture = cultureName;
		loc = location;
	}
}
