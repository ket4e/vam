using System.Xml;

namespace System.Security.Cryptography.Xml;

public sealed class EncryptedData : EncryptedType
{
	public override XmlElement GetXml()
	{
		return GetXml(new XmlDocument());
	}

	internal XmlElement GetXml(XmlDocument document)
	{
		if (CipherData == null)
		{
			throw new CryptographicException("Cipher data is not specified.");
		}
		XmlElement xmlElement = document.CreateElement("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
		if (EncryptionMethod != null)
		{
			xmlElement.AppendChild(EncryptionMethod.GetXml(document));
		}
		if (base.KeyInfo != null)
		{
			xmlElement.AppendChild(document.ImportNode(base.KeyInfo.GetXml(), deep: true));
		}
		if (CipherData != null)
		{
			xmlElement.AppendChild(CipherData.GetXml(document));
		}
		if (EncryptionProperties.Count > 0)
		{
			XmlElement xmlElement2 = document.CreateElement("EncryptionProperties", "http://www.w3.org/2001/04/xmlenc#");
			foreach (EncryptionProperty encryptionProperty in EncryptionProperties)
			{
				xmlElement2.AppendChild(encryptionProperty.GetXml(document));
			}
			xmlElement.AppendChild(xmlElement2);
		}
		if (Id != null)
		{
			xmlElement.SetAttribute("Id", Id);
		}
		if (Type != null)
		{
			xmlElement.SetAttribute("Type", Type);
		}
		if (MimeType != null)
		{
			xmlElement.SetAttribute("MimeType", MimeType);
		}
		if (Encoding != null)
		{
			xmlElement.SetAttribute("Encoding", Encoding);
		}
		return xmlElement;
	}

	public override void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.LocalName != "EncryptedData" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
		{
			throw new CryptographicException("Malformed EncryptedData element.");
		}
		EncryptionMethod = null;
		EncryptionMethod = null;
		EncryptionProperties.Clear();
		Id = null;
		Type = null;
		MimeType = null;
		Encoding = null;
		foreach (XmlNode childNode in value.ChildNodes)
		{
			if (childNode is XmlWhitespace)
			{
				continue;
			}
			switch (childNode.LocalName)
			{
			case "EncryptionMethod":
				EncryptionMethod = new EncryptionMethod();
				EncryptionMethod.LoadXml((XmlElement)childNode);
				break;
			case "KeyInfo":
				base.KeyInfo = new KeyInfo();
				base.KeyInfo.LoadXml((XmlElement)childNode);
				break;
			case "CipherData":
				CipherData = new CipherData();
				CipherData.LoadXml((XmlElement)childNode);
				break;
			case "EncryptionProperties":
				foreach (XmlElement item in ((XmlElement)childNode).GetElementsByTagName("EncryptionProperty", "http://www.w3.org/2001/04/xmlenc#"))
				{
					EncryptionProperties.Add(new EncryptionProperty(item));
				}
				break;
			}
		}
		if (value.HasAttribute("Id"))
		{
			Id = value.Attributes["Id"].Value;
		}
		if (value.HasAttribute("Type"))
		{
			Type = value.Attributes["Type"].Value;
		}
		if (value.HasAttribute("MimeType"))
		{
			MimeType = value.Attributes["MimeType"].Value;
		}
		if (value.HasAttribute("Encoding"))
		{
			Encoding = value.Attributes["Encoding"].Value;
		}
	}
}
