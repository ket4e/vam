using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public sealed class CipherData
{
	private byte[] cipherValue;

	private CipherReference cipherReference;

	public CipherReference CipherReference
	{
		get
		{
			return cipherReference;
		}
		set
		{
			if (CipherValue != null)
			{
				throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
			}
			cipherReference = value;
		}
	}

	public byte[] CipherValue
	{
		get
		{
			return cipherValue;
		}
		set
		{
			if (CipherReference != null)
			{
				throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
			}
			cipherValue = value;
		}
	}

	public CipherData()
	{
	}

	public CipherData(byte[] cipherValue)
	{
		CipherValue = cipherValue;
	}

	public CipherData(CipherReference cipherReference)
	{
		CipherReference = cipherReference;
	}

	public XmlElement GetXml()
	{
		return GetXml(new XmlDocument());
	}

	internal XmlElement GetXml(XmlDocument document)
	{
		if (CipherReference == null && CipherValue == null)
		{
			throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
		}
		XmlElement xmlElement = document.CreateElement("CipherData", "http://www.w3.org/2001/04/xmlenc#");
		if (CipherReference != null)
		{
			xmlElement.AppendChild(document.ImportNode(cipherReference.GetXml(), deep: true));
		}
		if (CipherValue != null)
		{
			XmlElement xmlElement2 = document.CreateElement("CipherValue", "http://www.w3.org/2001/04/xmlenc#");
			StreamReader streamReader = new StreamReader(new CryptoStream(new MemoryStream(cipherValue), new ToBase64Transform(), CryptoStreamMode.Read));
			xmlElement2.InnerText = streamReader.ReadToEnd();
			streamReader.Close();
			xmlElement.AppendChild(xmlElement2);
		}
		return xmlElement;
	}

	public void LoadXml(XmlElement value)
	{
		CipherReference = null;
		CipherValue = null;
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.LocalName != "CipherData" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
		{
			throw new CryptographicException("Malformed Cipher Data element.");
		}
		foreach (XmlNode childNode in value.ChildNodes)
		{
			if (!(childNode is XmlWhitespace))
			{
				switch (childNode.LocalName)
				{
				case "CipherReference":
					cipherReference = new CipherReference();
					cipherReference.LoadXml((XmlElement)childNode);
					break;
				case "CipherValue":
					CipherValue = Convert.FromBase64String(childNode.InnerText);
					break;
				}
			}
		}
		if (CipherReference == null && CipherValue == null)
		{
			throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
		}
	}
}
