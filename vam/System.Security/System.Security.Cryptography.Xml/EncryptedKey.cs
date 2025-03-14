using System.Xml;

namespace System.Security.Cryptography.Xml;

public sealed class EncryptedKey : EncryptedType
{
	private string carriedKeyName;

	private string recipient;

	private ReferenceList referenceList;

	public string CarriedKeyName
	{
		get
		{
			return carriedKeyName;
		}
		set
		{
			carriedKeyName = value;
		}
	}

	public string Recipient
	{
		get
		{
			return recipient;
		}
		set
		{
			recipient = value;
		}
	}

	public ReferenceList ReferenceList => referenceList;

	public EncryptedKey()
	{
		referenceList = new ReferenceList();
	}

	public void AddReference(DataReference dataReference)
	{
		ReferenceList.Add(dataReference);
	}

	public void AddReference(KeyReference keyReference)
	{
		ReferenceList.Add(keyReference);
	}

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
		XmlElement xmlElement = document.CreateElement("EncryptedKey", "http://www.w3.org/2001/04/xmlenc#");
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
		if (ReferenceList.Count > 0)
		{
			XmlElement xmlElement3 = document.CreateElement("ReferenceList", "http://www.w3.org/2001/04/xmlenc#");
			foreach (EncryptedReference reference in ReferenceList)
			{
				xmlElement3.AppendChild(reference.GetXml(document));
			}
			xmlElement.AppendChild(xmlElement3);
		}
		if (CarriedKeyName != null)
		{
			XmlElement xmlElement4 = document.CreateElement("CarriedKeyName", "http://www.w3.org/2001/04/xmlenc#");
			xmlElement4.InnerText = CarriedKeyName;
			xmlElement.AppendChild(xmlElement4);
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
		if (Recipient != null)
		{
			xmlElement.SetAttribute("Recipient", Recipient);
		}
		return xmlElement;
	}

	public override void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.LocalName != "EncryptedKey" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
		{
			throw new CryptographicException("Malformed EncryptedKey element.");
		}
		EncryptionMethod = null;
		EncryptionMethod = null;
		EncryptionProperties.Clear();
		ReferenceList.Clear();
		CarriedKeyName = null;
		Id = null;
		Type = null;
		MimeType = null;
		Encoding = null;
		Recipient = null;
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
			case "ReferenceList":
				foreach (XmlNode childNode2 in ((XmlElement)childNode).ChildNodes)
				{
					if (!(childNode2 is XmlWhitespace))
					{
						switch (childNode2.LocalName)
						{
						case "DataReference":
						{
							DataReference dataReference = new DataReference();
							dataReference.LoadXml((XmlElement)childNode2);
							AddReference(dataReference);
							break;
						}
						case "KeyReference":
						{
							KeyReference keyReference = new KeyReference();
							keyReference.LoadXml((XmlElement)childNode2);
							AddReference(keyReference);
							break;
						}
						}
					}
				}
				break;
			case "CarriedKeyName":
				CarriedKeyName = ((XmlElement)childNode).InnerText;
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
		if (value.HasAttribute("Recipient"))
		{
			Encoding = value.Attributes["Recipient"].Value;
		}
	}
}
