using System.Xml;

namespace System.Security.Cryptography.Xml;

public class DSAKeyValue : KeyInfoClause
{
	private DSA dsa;

	public DSA Key
	{
		get
		{
			return dsa;
		}
		set
		{
			dsa = value;
		}
	}

	public DSAKeyValue()
	{
		dsa = DSA.Create();
	}

	public DSAKeyValue(DSA key)
	{
		dsa = key;
	}

	public override XmlElement GetXml()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("KeyValue", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement.SetAttribute("xmlns", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement.InnerXml = dsa.ToXmlString(includePrivateParameters: false);
		return xmlElement;
	}

	public override void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		if (value.LocalName != "KeyValue" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
		{
			throw new CryptographicException("value");
		}
		dsa.FromXmlString(value.InnerXml);
	}
}
