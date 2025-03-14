using System.Xml;

namespace System.Security.Cryptography.Xml;

public class EncryptionMethod
{
	private string algorithm;

	private int keySize;

	public string KeyAlgorithm
	{
		get
		{
			return algorithm;
		}
		set
		{
			algorithm = value;
		}
	}

	public int KeySize
	{
		get
		{
			return keySize;
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("The key size should be a non negative integer.");
			}
			keySize = value;
		}
	}

	public EncryptionMethod()
	{
		KeyAlgorithm = null;
	}

	public EncryptionMethod(string strAlgorithm)
	{
		KeyAlgorithm = strAlgorithm;
	}

	public XmlElement GetXml()
	{
		return GetXml(new XmlDocument());
	}

	internal XmlElement GetXml(XmlDocument document)
	{
		XmlElement xmlElement = document.CreateElement("EncryptionMethod", "http://www.w3.org/2001/04/xmlenc#");
		if (KeySize != 0)
		{
			XmlElement xmlElement2 = document.CreateElement("KeySize", "http://www.w3.org/2001/04/xmlenc#");
			xmlElement2.InnerText = $"{keySize}";
			xmlElement.AppendChild(xmlElement2);
		}
		if (KeyAlgorithm != null)
		{
			xmlElement.SetAttribute("Algorithm", KeyAlgorithm);
		}
		return xmlElement;
	}

	public void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.LocalName != "EncryptionMethod" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
		{
			throw new CryptographicException("Malformed EncryptionMethod element.");
		}
		KeyAlgorithm = null;
		foreach (XmlNode childNode in value.ChildNodes)
		{
			if (!(childNode is XmlWhitespace))
			{
				switch (childNode.LocalName)
				{
				case "KeySize":
					KeySize = int.Parse(childNode.InnerText);
					break;
				}
			}
		}
		if (value.HasAttribute("Algorithm"))
		{
			KeyAlgorithm = value.Attributes["Algorithm"].Value;
		}
	}
}
