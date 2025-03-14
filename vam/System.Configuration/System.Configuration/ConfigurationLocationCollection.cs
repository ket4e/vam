using System.Collections;

namespace System.Configuration;

public class ConfigurationLocationCollection : ReadOnlyCollectionBase
{
	public ConfigurationLocation this[int index] => base.InnerList[index] as ConfigurationLocation;

	internal ConfigurationLocationCollection()
	{
	}

	internal void Add(ConfigurationLocation loc)
	{
		base.InnerList.Add(loc);
	}

	internal ConfigurationLocation Find(string location)
	{
		foreach (ConfigurationLocation inner in base.InnerList)
		{
			if (string.Compare(inner.Path, location, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return inner;
			}
		}
		return null;
	}
}
