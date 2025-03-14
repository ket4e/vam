using System.Xml;

namespace System.Security.Cryptography.Xml;

public class KeyInfoName : KeyInfoClause
{
	private string name;

	public string Value
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public KeyInfoName()
	{
	}

	public KeyInfoName(string keyName)
	{
		name = keyName;
	}

	public override XmlElement GetXml()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("KeyName", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement.InnerText = name;
		return xmlElement;
	}

	public override void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		if (value.LocalName != "KeyName" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
		{
			name = string.Empty;
		}
		else
		{
			name = value.InnerText;
		}
	}
}
