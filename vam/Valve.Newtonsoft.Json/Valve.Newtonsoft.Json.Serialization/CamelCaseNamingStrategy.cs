using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Serialization;

public class CamelCaseNamingStrategy : NamingStrategy
{
	public CamelCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
	{
		base.ProcessDictionaryKeys = processDictionaryKeys;
		base.OverrideSpecifiedNames = overrideSpecifiedNames;
	}

	public CamelCaseNamingStrategy()
	{
	}

	protected override string ResolvePropertyName(string name)
	{
		return StringUtils.ToCamelCase(name);
	}
}
