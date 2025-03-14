using System.Xml;

namespace System.Security.Cryptography.Xml;

public class DataObject
{
	private XmlElement element;

	private bool propertyModified;

	public XmlNodeList Data
	{
		get
		{
			return element.ChildNodes;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = (XmlElement)xmlDocument.ImportNode(element, deep: true);
			while (xmlElement.LastChild != null)
			{
				xmlElement.RemoveChild(xmlElement.LastChild);
			}
			foreach (XmlNode item in value)
			{
				xmlElement.AppendChild(xmlDocument.ImportNode(item, deep: true));
			}
			element = xmlElement;
			propertyModified = true;
		}
	}

	public string Encoding
	{
		get
		{
			return GetField("Encoding");
		}
		set
		{
			SetField("Encoding", value);
		}
	}

	public string Id
	{
		get
		{
			return GetField("Id");
		}
		set
		{
			SetField("Id", value);
		}
	}

	public string MimeType
	{
		get
		{
			return GetField("MimeType");
		}
		set
		{
			SetField("MimeType", value);
		}
	}

	public DataObject()
	{
		Build(null, null, null, null);
	}

	public DataObject(string id, string mimeType, string encoding, XmlElement data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		Build(id, mimeType, encoding, data);
	}

	private void Build(string id, string mimeType, string encoding, XmlElement data)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("Object", "http://www.w3.org/2000/09/xmldsig#");
		if (id != null)
		{
			xmlElement.SetAttribute("Id", id);
		}
		if (mimeType != null)
		{
			xmlElement.SetAttribute("MimeType", mimeType);
		}
		if (encoding != null)
		{
			xmlElement.SetAttribute("Encoding", encoding);
		}
		if (data != null)
		{
			XmlNode newChild = xmlDocument.ImportNode(data, deep: true);
			xmlElement.AppendChild(newChild);
		}
		element = xmlElement;
	}

	private string GetField(string attribute)
	{
		return element.Attributes[attribute]?.Value;
	}

	private void SetField(string attribute, string value)
	{
		if (value != null)
		{
			if (propertyModified)
			{
				element.SetAttribute(attribute, value);
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.ImportNode(element, deep: true) as XmlElement;
			xmlElement.SetAttribute(attribute, value);
			element = xmlElement;
			propertyModified = true;
		}
	}

	public XmlElement GetXml()
	{
		if (propertyModified)
		{
			XmlElement xmlElement = element;
			XmlDocument xmlDocument = new XmlDocument();
			element = xmlDocument.CreateElement("Object", "http://www.w3.org/2000/09/xmldsig#");
			foreach (XmlAttribute attribute in xmlElement.Attributes)
			{
				switch (attribute.Name)
				{
				case "Id":
				case "Encoding":
				case "MimeType":
					element.SetAttribute(attribute.Name, attribute.Value);
					break;
				}
			}
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				element.AppendChild(xmlDocument.ImportNode(childNode, deep: true));
			}
		}
		return element;
	}

	public void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		element = value;
		propertyModified = false;
	}
}
