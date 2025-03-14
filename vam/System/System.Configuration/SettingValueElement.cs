using System.Xml;

namespace System.Configuration;

public sealed class SettingValueElement : ConfigurationElement
{
	private XmlNode node;

	[System.MonoTODO]
	protected override ConfigurationPropertyCollection Properties => base.Properties;

	public XmlNode ValueXml
	{
		get
		{
			return node;
		}
		set
		{
			node = value;
		}
	}

	[System.MonoTODO]
	public SettingValueElement()
	{
	}

	[System.MonoTODO]
	protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
	{
		node = new XmlDocument().ReadNode(reader);
	}

	public override bool Equals(object settingValue)
	{
		throw new NotImplementedException();
	}

	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}

	protected override bool IsModified()
	{
		throw new NotImplementedException();
	}

	protected override void Reset(ConfigurationElement parentElement)
	{
		node = null;
	}

	protected override void ResetModified()
	{
		throw new NotImplementedException();
	}

	protected override bool SerializeToXmlElement(XmlWriter writer, string elementName)
	{
		if (node == null)
		{
			return false;
		}
		node.WriteTo(writer);
		return true;
	}

	protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
	{
		throw new NotImplementedException();
	}
}
