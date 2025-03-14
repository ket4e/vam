using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class Reference
{
	private TransformChain chain;

	private string digestMethod;

	private byte[] digestValue;

	private string id;

	private string uri;

	private string type;

	private Stream stream;

	private XmlElement element;

	public string DigestMethod
	{
		get
		{
			return digestMethod;
		}
		set
		{
			element = null;
			digestMethod = value;
		}
	}

	public byte[] DigestValue
	{
		get
		{
			return digestValue;
		}
		set
		{
			element = null;
			digestValue = value;
		}
	}

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			element = null;
			id = value;
		}
	}

	public TransformChain TransformChain
	{
		get
		{
			return chain;
		}
		[ComVisible(false)]
		set
		{
			chain = value;
		}
	}

	public string Type
	{
		get
		{
			return type;
		}
		set
		{
			element = null;
			type = value;
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
			element = null;
			uri = value;
		}
	}

	public Reference()
	{
		chain = new TransformChain();
		digestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
	}

	[System.MonoTODO("There is no description about how it is used.")]
	public Reference(Stream stream)
		: this()
	{
		this.stream = stream;
	}

	public Reference(string uri)
		: this()
	{
		this.uri = uri;
	}

	public void AddTransform(Transform transform)
	{
		chain.Add(transform);
	}

	public XmlElement GetXml()
	{
		if (element != null)
		{
			return element;
		}
		if (digestMethod == null)
		{
			throw new CryptographicException("DigestMethod");
		}
		if (digestValue == null)
		{
			throw new NullReferenceException("DigestValue");
		}
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("Reference", "http://www.w3.org/2000/09/xmldsig#");
		if (id != null)
		{
			xmlElement.SetAttribute("Id", id);
		}
		if (uri != null)
		{
			xmlElement.SetAttribute("URI", uri);
		}
		if (type != null)
		{
			xmlElement.SetAttribute("Type", type);
		}
		if (chain.Count > 0)
		{
			XmlElement xmlElement2 = xmlDocument.CreateElement("Transforms", "http://www.w3.org/2000/09/xmldsig#");
			foreach (Transform item in chain)
			{
				XmlNode xml = item.GetXml();
				XmlNode newChild = xmlDocument.ImportNode(xml, deep: true);
				xmlElement2.AppendChild(newChild);
			}
			xmlElement.AppendChild(xmlElement2);
		}
		XmlElement xmlElement3 = xmlDocument.CreateElement("DigestMethod", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement3.SetAttribute("Algorithm", digestMethod);
		xmlElement.AppendChild(xmlElement3);
		XmlElement xmlElement4 = xmlDocument.CreateElement("DigestValue", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement4.InnerText = Convert.ToBase64String(digestValue);
		xmlElement.AppendChild(xmlElement4);
		return xmlElement;
	}

	private string GetAttribute(XmlElement xel, string attribute)
	{
		return xel.Attributes[attribute]?.InnerText;
	}

	public void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.LocalName != "Reference" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
		{
			throw new CryptographicException();
		}
		id = GetAttribute(value, "Id");
		uri = GetAttribute(value, "URI");
		type = GetAttribute(value, "Type");
		XmlNodeList elementsByTagName = value.GetElementsByTagName("Transform", "http://www.w3.org/2000/09/xmldsig#");
		if (elementsByTagName != null && elementsByTagName.Count > 0)
		{
			Transform transform = null;
			foreach (XmlNode item in elementsByTagName)
			{
				string attribute = GetAttribute((XmlElement)item, "Algorithm");
				transform = (Transform)CryptoConfig.CreateFromName(attribute);
				if (transform == null)
				{
					throw new CryptographicException("Unknown transform {0}.", attribute);
				}
				if (item.ChildNodes.Count > 0)
				{
					transform.LoadInnerXml(item.ChildNodes);
				}
				AddTransform(transform);
			}
		}
		DigestMethod = XmlSignature.GetAttributeFromElement(value, "Algorithm", "DigestMethod");
		XmlElement childElement = XmlSignature.GetChildElement(value, "DigestValue", "http://www.w3.org/2000/09/xmldsig#");
		if (childElement != null)
		{
			DigestValue = Convert.FromBase64String(childElement.InnerText);
		}
		element = value;
	}
}
