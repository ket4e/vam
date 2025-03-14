using System.Configuration;

namespace System.Net.Configuration;

public sealed class BypassElement : ConfigurationElement
{
	private static ConfigurationPropertyCollection properties;

	private static ConfigurationProperty addressProp;

	[ConfigurationProperty("address", Options = (ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey))]
	public string Address
	{
		get
		{
			return (string)base[addressProp];
		}
		set
		{
			base[addressProp] = value;
		}
	}

	protected override ConfigurationPropertyCollection Properties => properties;

	public BypassElement()
	{
	}

	public BypassElement(string address)
	{
		Address = address;
	}

	static BypassElement()
	{
		addressProp = new ConfigurationProperty("Address", typeof(string), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
		properties = new ConfigurationPropertyCollection();
		properties.Add(addressProp);
	}
}
