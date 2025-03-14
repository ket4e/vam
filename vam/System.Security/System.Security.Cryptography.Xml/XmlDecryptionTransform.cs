using System.Collections;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class XmlDecryptionTransform : Transform
{
	private const string NamespaceUri = "http://www.w3.org/2002/07/decrypt#";

	private EncryptedXml encryptedXml;

	private Type[] inputTypes;

	private Type[] outputTypes;

	private object inputObj;

	private ArrayList exceptUris;

	public EncryptedXml EncryptedXml
	{
		get
		{
			return encryptedXml;
		}
		set
		{
			encryptedXml = value;
		}
	}

	public override Type[] InputTypes
	{
		get
		{
			if (inputTypes == null)
			{
				inputTypes = new Type[2]
				{
					typeof(Stream),
					typeof(XmlDocument)
				};
			}
			return inputTypes;
		}
	}

	public override Type[] OutputTypes
	{
		get
		{
			if (outputTypes == null)
			{
				outputTypes = new Type[1] { typeof(XmlDocument) };
			}
			return outputTypes;
		}
	}

	public XmlDecryptionTransform()
	{
		base.Algorithm = "http://www.w3.org/2002/07/decrypt#XML";
		encryptedXml = new EncryptedXml();
		exceptUris = new ArrayList();
	}

	public void AddExceptUri(string uri)
	{
		exceptUris.Add(uri);
	}

	private void ClearExceptUris()
	{
		exceptUris.Clear();
	}

	[System.MonoTODO("Verify")]
	protected override XmlNodeList GetInnerXml()
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.AppendChild(xmlDocument.CreateElement("DecryptionTransform"));
		foreach (object exceptUri in exceptUris)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("Except", "http://www.w3.org/2002/07/decrypt#");
			xmlElement.Attributes.Append(xmlDocument.CreateAttribute("URI", "http://www.w3.org/2002/07/decrypt#"));
			xmlElement.Attributes["URI", "http://www.w3.org/2002/07/decrypt#"].Value = (string)exceptUri;
			xmlDocument.DocumentElement.AppendChild(xmlElement);
		}
		return xmlDocument.GetElementsByTagName("Except", "http://www.w3.org/2002/07/decrypt#");
	}

	[System.MonoTODO("Verify processing of ExceptURIs")]
	public override object GetOutput()
	{
		XmlDocument xmlDocument;
		if (inputObj is Stream)
		{
			xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.XmlResolver = GetResolver();
			xmlDocument.Load(new XmlSignatureStreamReader(new StreamReader(inputObj as Stream)));
		}
		else
		{
			if (!(inputObj is XmlDocument))
			{
				throw new NullReferenceException();
			}
			xmlDocument = inputObj as XmlDocument;
		}
		XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item == xmlDocument.DocumentElement && exceptUris.Contains("#xpointer(/)"))
			{
				break;
			}
			foreach (string exceptUri in exceptUris)
			{
				if (IsTargetElement((XmlElement)item, exceptUri.Substring(1)))
				{
					break;
				}
			}
			EncryptedData encryptedData = new EncryptedData();
			encryptedData.LoadXml((XmlElement)item);
			SymmetricAlgorithm decryptionKey = EncryptedXml.GetDecryptionKey(encryptedData, encryptedData.EncryptionMethod.KeyAlgorithm);
			EncryptedXml.ReplaceData((XmlElement)item, EncryptedXml.DecryptData(encryptedData, decryptionKey));
		}
		return xmlDocument;
	}

	public override object GetOutput(Type type)
	{
		if (type == typeof(Stream))
		{
			return GetOutput();
		}
		throw new ArgumentException("type");
	}

	[System.MonoTODO("verify")]
	protected virtual bool IsTargetElement(XmlElement inputElement, string idValue)
	{
		if (inputElement == null || idValue == null)
		{
			return false;
		}
		return inputElement.Attributes["id"].Value == idValue;
	}

	[System.MonoTODO("This doesn't seem to work in .NET")]
	public override void LoadInnerXml(XmlNodeList nodeList)
	{
		if (nodeList == null)
		{
			throw new NullReferenceException();
		}
		ClearExceptUris();
		foreach (XmlNode node in nodeList)
		{
			XmlElement xmlElement = node as XmlElement;
			if (xmlElement.NamespaceURI.Equals("http://www.w3.org/2002/07/decrypt#") && xmlElement.LocalName.Equals("Except"))
			{
				string value = xmlElement.Attributes["URI", "http://www.w3.org/2002/07/decrypt#"].Value;
				if (!value.StartsWith("#"))
				{
					throw new CryptographicException("A Uri attribute is required for a CipherReference element.");
				}
				AddExceptUri(value);
			}
		}
	}

	public override void LoadInput(object obj)
	{
		inputObj = obj;
	}
}
