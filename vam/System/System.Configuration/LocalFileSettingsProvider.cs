using System.Collections.Specialized;

namespace System.Configuration;

public class LocalFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider
{
	private System.Configuration.CustomizableFileSettingsProvider impl;

	public override string ApplicationName
	{
		get
		{
			return impl.ApplicationName;
		}
		set
		{
			impl.ApplicationName = value;
		}
	}

	public LocalFileSettingsProvider()
	{
		impl = new System.Configuration.CustomizableFileSettingsProvider();
	}

	[System.MonoTODO]
	public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
	{
		return impl.GetPreviousVersion(context, property);
	}

	[System.MonoTODO]
	public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
	{
		return impl.GetPropertyValues(context, properties);
	}

	public override void Initialize(string name, NameValueCollection values)
	{
		if (name == null)
		{
			name = "LocalFileSettingsProvider";
		}
		if (values != null)
		{
			impl.ApplicationName = values["applicationName"];
		}
		base.Initialize(name, values);
	}

	[System.MonoTODO]
	public void Reset(SettingsContext context)
	{
		impl.Reset(context);
	}

	[System.MonoTODO]
	public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
	{
		impl.SetPropertyValues(context, values);
	}

	[System.MonoTODO]
	public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
	{
		impl.Upgrade(context, properties);
	}
}
