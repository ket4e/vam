using System.Runtime.InteropServices;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class KeyInfoRetrievalMethod : KeyInfoClause
{
	private string URI;

	private XmlElement element;

	private string type;

	[ComVisible(false)]
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
			return URI;
		}
		set
		{
			element = null;
			URI = value;
		}
	}

	public KeyInfoRetrievalMethod()
	{
	}

	public KeyInfoRetrievalMethod(string strUri)
	{
		URI = strUri;
	}

	public KeyInfoRetrievalMethod(string strUri, string strType)
		: this(strUri)
	{
		Type = strType;
	}

	public override XmlElement GetXml()
	{
		if (element != null)
		{
			return element;
		}
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("RetrievalMethod", "http://www.w3.org/2000/09/xmldsig#");
		if (URI != null && URI.Length > 0)
		{
			xmlElement.SetAttribute("URI", URI);
		}
		if (Type != null)
		{
			xmlElement.SetAttribute("Type", Type);
		}
		return xmlElement;
	}

	public override void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		if (value.LocalName != "RetrievalMethod" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
		{
			URI = string.Empty;
			return;
		}
		URI = value.Attributes["URI"].Value;
		if (value.HasAttribute("Type"))
		{
			Type = value.Attributes["Type"].Value;
		}
		element = value;
	}
}
