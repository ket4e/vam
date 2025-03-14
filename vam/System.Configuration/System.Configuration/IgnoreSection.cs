using System.Xml;

namespace System.Configuration;

public sealed class IgnoreSection : ConfigurationSection
{
	private string xml;

	private static ConfigurationPropertyCollection properties;

	protected internal override ConfigurationPropertyCollection Properties => properties;

	static IgnoreSection()
	{
		properties = new ConfigurationPropertyCollection();
	}

	protected internal override bool IsModified()
	{
		return false;
	}

	protected internal override void DeserializeSection(XmlReader reader)
	{
		xml = reader.ReadOuterXml();
	}

	[System.MonoTODO]
	protected internal override void Reset(ConfigurationElement parentElement)
	{
		base.Reset(parentElement);
	}

	[System.MonoTODO]
	protected internal override void ResetModified()
	{
		base.ResetModified();
	}

	protected internal override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
	{
		return xml;
	}
}
