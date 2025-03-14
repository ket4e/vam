using System.Xml;

namespace System.Security.Cryptography.Xml;

public sealed class EncryptionProperty
{
	private XmlElement elemProp;

	private string id;

	private string target;

	public string Id => id;

	public XmlElement PropertyElement
	{
		get
		{
			return elemProp;
		}
		set
		{
			LoadXml(value);
		}
	}

	public string Target => target;

	public EncryptionProperty()
	{
	}

	public EncryptionProperty(XmlElement elemProp)
	{
		LoadXml(elemProp);
	}

	public XmlElement GetXml()
	{
		return GetXml(new XmlDocument());
	}

	internal XmlElement GetXml(XmlDocument document)
	{
		XmlElement xmlElement = document.CreateElement("EncryptionProperty", "http://www.w3.org/2001/04/xmlenc#");
		if (Id != null)
		{
			xmlElement.SetAttribute("Id", Id);
		}
		if (Target != null)
		{
			xmlElement.SetAttribute("Target", Target);
		}
		return xmlElement;
	}

	public void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.LocalName != "EncryptionProperty" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
		{
			throw new CryptographicException("Malformed EncryptionProperty element.");
		}
		if (value.HasAttribute("Id"))
		{
			id = value.Attributes["Id"].Value;
		}
		if (value.HasAttribute("Target"))
		{
			target = value.Attributes["Target"].Value;
		}
	}
}
