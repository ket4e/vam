using System.Xml.Serialization;

namespace System.Xml.Schema;

public class XmlSchemaImport : XmlSchemaExternal
{
	private const string xmlname = "import";

	private XmlSchemaAnnotation annotation;

	private string nameSpace;

	[XmlAttribute("namespace", DataType = "anyURI")]
	public string Namespace
	{
		get
		{
			return nameSpace;
		}
		set
		{
			nameSpace = value;
		}
	}

	[XmlElement("annotation", Type = typeof(XmlSchemaAnnotation))]
	public XmlSchemaAnnotation Annotation
	{
		get
		{
			return annotation;
		}
		set
		{
			annotation = value;
		}
	}

	internal static XmlSchemaImport Read(XmlSchemaReader reader, ValidationEventHandler h)
	{
		XmlSchemaImport xmlSchemaImport = new XmlSchemaImport();
		reader.MoveToElement();
		if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "import")
		{
			XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaImport.Read, name=" + reader.Name, null);
			reader.SkipToEnd();
			return null;
		}
		xmlSchemaImport.LineNumber = reader.LineNumber;
		xmlSchemaImport.LinePosition = reader.LinePosition;
		xmlSchemaImport.SourceUri = reader.BaseURI;
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "id")
			{
				xmlSchemaImport.Id = reader.Value;
			}
			else if (reader.Name == "namespace")
			{
				xmlSchemaImport.nameSpace = reader.Value;
			}
			else if (reader.Name == "schemaLocation")
			{
				xmlSchemaImport.SchemaLocation = reader.Value;
			}
			else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for import", null);
			}
			else
			{
				XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaImport);
			}
		}
		reader.MoveToElement();
		if (reader.IsEmptyElement)
		{
			return xmlSchemaImport;
		}
		int num = 1;
		while (reader.ReadNextElement())
		{
			if (reader.NodeType == XmlNodeType.EndElement)
			{
				if (reader.LocalName != "import")
				{
					XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaImport.Read, name=" + reader.Name, null);
				}
				break;
			}
			if (num <= 1 && reader.LocalName == "annotation")
			{
				num = 2;
				XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
				if (xmlSchemaAnnotation != null)
				{
					xmlSchemaImport.Annotation = xmlSchemaAnnotation;
				}
			}
			else
			{
				reader.RaiseInvalidElementError();
			}
		}
		return xmlSchemaImport;
	}
}
