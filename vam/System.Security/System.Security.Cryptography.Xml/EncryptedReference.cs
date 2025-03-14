using System.Xml;

namespace System.Security.Cryptography.Xml;

public abstract class EncryptedReference
{
	private bool cacheValid;

	private string referenceType;

	private string uri;

	private TransformChain tc;

	[System.MonoTODO]
	protected internal bool CacheValid => cacheValid;

	protected string ReferenceType
	{
		get
		{
			return referenceType;
		}
		set
		{
			referenceType = value;
		}
	}

	public TransformChain TransformChain
	{
		get
		{
			return tc;
		}
		set
		{
			tc = value;
		}
	}

	public string Uri
	{
		get
		{
			return uri;
		}
		set
		{
			uri = value;
		}
	}

	protected EncryptedReference()
	{
		TransformChain = new TransformChain();
	}

	protected EncryptedReference(string uri)
	{
		Uri = uri;
		TransformChain = new TransformChain();
	}

	protected EncryptedReference(string uri, TransformChain tc)
		: this()
	{
		Uri = uri;
		TransformChain = tc;
	}

	public void AddTransform(Transform transform)
	{
		TransformChain.Add(transform);
	}

	public virtual XmlElement GetXml()
	{
		return GetXml(new XmlDocument());
	}

	internal virtual XmlElement GetXml(XmlDocument document)
	{
		XmlElement xmlElement = document.CreateElement(ReferenceType, "http://www.w3.org/2001/04/xmlenc#");
		xmlElement.SetAttribute("URI", Uri);
		if (TransformChain != null && TransformChain.Count > 0)
		{
			XmlElement xmlElement2 = document.CreateElement("Transforms", "http://www.w3.org/2001/04/xmlenc#");
			foreach (Transform item in TransformChain)
			{
				xmlElement2.AppendChild(document.ImportNode(item.GetXml(), deep: true));
			}
			xmlElement.AppendChild(xmlElement2);
		}
		return xmlElement;
	}

	[System.MonoTODO("Make compliant.")]
	public virtual void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		Uri = null;
		TransformChain = new TransformChain();
		foreach (XmlNode childNode in value.ChildNodes)
		{
			if (childNode is XmlWhitespace)
			{
				continue;
			}
			switch (childNode.LocalName)
			{
			case "Transforms":
				foreach (XmlNode item in ((XmlElement)childNode).GetElementsByTagName("Transform", "http://www.w3.org/2000/09/xmldsig#"))
				{
					Transform transform = null;
					switch (((XmlElement)item).Attributes["Algorithm"].Value)
					{
					case "http://www.w3.org/2000/09/xmldsig#base64":
						transform = new XmlDsigBase64Transform();
						break;
					case "http://www.w3.org/TR/2001/REC-xml-c14n-20010315":
						transform = new XmlDsigC14NTransform();
						break;
					case "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments":
						transform = new XmlDsigC14NWithCommentsTransform();
						break;
					case "http://www.w3.org/2000/09/xmldsig#enveloped-signature":
						transform = new XmlDsigEnvelopedSignatureTransform();
						break;
					case "http://www.w3.org/TR/1999/REC-xpath-19991116":
						transform = new XmlDsigXPathTransform();
						break;
					case "http://www.w3.org/TR/1999/REC-xslt-19991116":
						transform = new XmlDsigXsltTransform();
						break;
					case "http://www.w3.org/2001/10/xml-exc-c14n#":
						transform = new XmlDsigExcC14NTransform();
						break;
					case "http://www.w3.org/2001/10/xml-exc-c14n#WithComments":
						transform = new XmlDsigExcC14NWithCommentsTransform();
						break;
					case "http://www.w3.org/2002/07/decrypt#XML":
						transform = new XmlDecryptionTransform();
						break;
					default:
						continue;
					}
					transform.LoadInnerXml(((XmlElement)item).ChildNodes);
					TransformChain.Add(transform);
				}
				break;
			}
		}
		if (value.HasAttribute("URI"))
		{
			Uri = value.Attributes["URI"].Value;
		}
	}
}
