using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml.Serialization;

namespace System.Diagnostics;

public abstract class Switch
{
	private string name;

	private string description;

	private int switchSetting;

	private string value;

	private string defaultSwitchValue;

	private bool initialized;

	private StringDictionary attributes = new StringDictionary();

	public string Description => description;

	public string DisplayName => name;

	protected int SwitchSetting
	{
		get
		{
			if (!initialized)
			{
				initialized = true;
				GetConfigFileSetting();
				OnSwitchSettingChanged();
			}
			return switchSetting;
		}
		set
		{
			if (switchSetting != value)
			{
				switchSetting = value;
				OnSwitchSettingChanged();
			}
			initialized = true;
		}
	}

	[XmlIgnore]
	public StringDictionary Attributes => attributes;

	protected string Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			try
			{
				OnValueChanged();
			}
			catch (Exception inner)
			{
				string message = $"The config value for Switch '{DisplayName}' was invalid.";
				throw new ConfigurationErrorsException(message, inner);
			}
		}
	}

	protected Switch(string displayName, string description)
	{
		name = displayName;
		this.description = description;
	}

	protected Switch(string displayName, string description, string defaultSwitchValue)
		: this(displayName, description)
	{
		this.defaultSwitchValue = defaultSwitchValue;
	}

	protected internal virtual string[] GetSupportedAttributes()
	{
		return null;
	}

	protected virtual void OnValueChanged()
	{
	}

	private void GetConfigFileSetting()
	{
		IDictionary dictionary = null;
		dictionary = (IDictionary)System.Diagnostics.DiagnosticsConfiguration.Settings["switches"];
		if (dictionary != null && dictionary.Contains(name))
		{
			Value = dictionary[name] as string;
		}
		else if (defaultSwitchValue != null)
		{
			value = defaultSwitchValue;
			OnValueChanged();
		}
	}

	protected virtual void OnSwitchSettingChanged()
	{
	}
}
