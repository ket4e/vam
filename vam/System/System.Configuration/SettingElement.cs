namespace System.Configuration;

public sealed class SettingElement : ConfigurationElement
{
	private static ConfigurationPropertyCollection properties;

	private static ConfigurationProperty name_prop;

	private static ConfigurationProperty serialize_as_prop;

	private static ConfigurationProperty value_prop;

	[ConfigurationProperty("name", DefaultValue = "", Options = (ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey))]
	public string Name
	{
		get
		{
			return (string)base[name_prop];
		}
		set
		{
			base[name_prop] = value;
		}
	}

	[ConfigurationProperty("value", DefaultValue = null, Options = ConfigurationPropertyOptions.IsRequired)]
	public SettingValueElement Value
	{
		get
		{
			return (SettingValueElement)base[value_prop];
		}
		set
		{
			base[value_prop] = value;
		}
	}

	[ConfigurationProperty("serializeAs", DefaultValue = SettingsSerializeAs.String, Options = ConfigurationPropertyOptions.IsRequired)]
	public SettingsSerializeAs SerializeAs
	{
		get
		{
			return (base[serialize_as_prop] != null) ? ((SettingsSerializeAs)(int)base[serialize_as_prop]) : SettingsSerializeAs.String;
		}
		set
		{
			base[serialize_as_prop] = value;
		}
	}

	protected override ConfigurationPropertyCollection Properties => properties;

	public SettingElement()
	{
	}

	public SettingElement(string name, SettingsSerializeAs serializeAs)
	{
		Name = name;
		SerializeAs = serializeAs;
	}

	static SettingElement()
	{
		name_prop = new ConfigurationProperty("name", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
		serialize_as_prop = new ConfigurationProperty("serializeAs", typeof(SettingsSerializeAs), null, ConfigurationPropertyOptions.IsRequired);
		value_prop = new ConfigurationProperty("value", typeof(SettingValueElement), null, ConfigurationPropertyOptions.IsRequired);
		properties = new ConfigurationPropertyCollection();
		properties.Add(name_prop);
		properties.Add(serialize_as_prop);
		properties.Add(value_prop);
	}

	public override bool Equals(object o)
	{
		if (!(o is SettingElement settingElement))
		{
			return false;
		}
		return settingElement.SerializeAs == SerializeAs && settingElement.Value == Value && settingElement.Name == Name;
	}

	public override int GetHashCode()
	{
		int num = (int)(SerializeAs ^ (SettingsSerializeAs)127);
		if (Name != null)
		{
			num += Name.GetHashCode() ^ 0x7F;
		}
		if (Value != null)
		{
			num += Value.GetHashCode();
		}
		return num;
	}
}
