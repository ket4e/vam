using System.Xml.Serialization;

namespace System.Xml.Schema;

public class XmlSchemaInclude : XmlSchemaExternal
{
	private const string xmlname = "include";

	private XmlSchemaAnnotation annotation;

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

	internal static XmlSchemaInclude Read(XmlSchemaReader reader, ValidationEventHandler h)
	{
		XmlSchemaInclude xmlSchemaInclude = new XmlSchemaInclude();
		reader.MoveToElement();
		if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "include")
		{
			XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaInclude.Read, name=" + reader.Name, null);
			reader.SkipToEnd();
			return null;
		}
		xmlSchemaInclude.LineNumber = reader.LineNumber;
		xmlSchemaInclude.LinePosition = reader.LinePosition;
		xmlSchemaInclude.SourceUri = reader.BaseURI;
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "id")
			{
				xmlSchemaInclude.Id = reader.Value;
			}
			else if (reader.Name == "schemaLocation")
			{
				xmlSchemaInclude.SchemaLocation = reader.Value;
			}
			else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for include", null);
			}
			else
			{
				XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaInclude);
			}
		}
		reader.MoveToElement();
		if (reader.IsEmptyElement)
		{
			return xmlSchemaInclude;
		}
		int num = 1;
		while (reader.ReadNextElement())
		{
			if (reader.NodeType == XmlNodeType.EndElement)
			{
				if (reader.LocalName != "include")
				{
					XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaInclude.Read, name=" + reader.Name, null);
				}
				break;
			}
			if (num <= 1 && reader.LocalName == "annotation")
			{
				num = 2;
				XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
				if (xmlSchemaAnnotation != null)
				{
					xmlSchemaInclude.Annotation = xmlSchemaAnnotation;
				}
			}
			else
			{
				reader.RaiseInvalidElementError();
			}
		}
		return xmlSchemaInclude;
	}
}
