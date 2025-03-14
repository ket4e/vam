using System.Xml;

namespace System.Security.Cryptography.Xml;

public class RSAKeyValue : KeyInfoClause
{
	private RSA rsa;

	public RSA Key
	{
		get
		{
			return rsa;
		}
		set
		{
			rsa = value;
		}
	}

	public RSAKeyValue()
	{
		rsa = RSA.Create();
	}

	public RSAKeyValue(RSA key)
	{
		rsa = key;
	}

	public override XmlElement GetXml()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("KeyValue", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement.SetAttribute("xmlns", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement.InnerXml = rsa.ToXmlString(includePrivateParameters: false);
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
		rsa.FromXmlString(value.InnerXml);
	}
}
