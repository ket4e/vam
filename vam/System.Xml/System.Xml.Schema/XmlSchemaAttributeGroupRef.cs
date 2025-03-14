using System.Xml.Serialization;

namespace System.Xml.Schema;

public class XmlSchemaAttributeGroupRef : XmlSchemaAnnotated
{
	private const string xmlname = "attributeGroup";

	private XmlQualifiedName refName;

	[XmlAttribute("ref")]
	public XmlQualifiedName RefName
	{
		get
		{
			return refName;
		}
		set
		{
			refName = value;
		}
	}

	public XmlSchemaAttributeGroupRef()
	{
		refName = XmlQualifiedName.Empty;
	}

	internal override int Compile(ValidationEventHandler h, XmlSchema schema)
	{
		if (CompilationId == schema.CompilationId)
		{
			return 0;
		}
		errorCount = 0;
		if (RefName == null || RefName.IsEmpty)
		{
			error(h, "ref must be present");
		}
		else if (!XmlSchemaUtil.CheckQName(RefName))
		{
			error(h, "ref must be a valid qname");
		}
		XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
		CompilationId = schema.CompilationId;
		return errorCount;
	}

	internal override int Validate(ValidationEventHandler h, XmlSchema schema)
	{
		return errorCount;
	}

	internal static XmlSchemaAttributeGroupRef Read(XmlSchemaReader reader, ValidationEventHandler h)
	{
		XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = new XmlSchemaAttributeGroupRef();
		reader.MoveToElement();
		if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "attributeGroup")
		{
			XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAttributeGroupRef.Read, name=" + reader.Name, null);
			reader.SkipToEnd();
			return null;
		}
		xmlSchemaAttributeGroupRef.LineNumber = reader.LineNumber;
		xmlSchemaAttributeGroupRef.LinePosition = reader.LinePosition;
		xmlSchemaAttributeGroupRef.SourceUri = reader.BaseURI;
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "id")
			{
				xmlSchemaAttributeGroupRef.Id = reader.Value;
			}
			else if (reader.Name == "ref")
			{
				xmlSchemaAttributeGroupRef.refName = XmlSchemaUtil.ReadQNameAttribute(reader, out var innerEx);
				if (innerEx != null)
				{
					XmlSchemaObject.error(h, reader.Value + " is not a valid value for ref attribute", innerEx);
				}
			}
			else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for attributeGroup in this context", null);
			}
			else
			{
				XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaAttributeGroupRef);
			}
		}
		reader.MoveToElement();
		if (reader.IsEmptyElement)
		{
			return xmlSchemaAttributeGroupRef;
		}
		int num = 1;
		while (reader.ReadNextElement())
		{
			if (reader.NodeType == XmlNodeType.EndElement)
			{
				if (reader.LocalName != "attributeGroup")
				{
					XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaAttributeGroupRef.Read, name=" + reader.Name, null);
				}
				break;
			}
			if (num <= 1 && reader.LocalName == "annotation")
			{
				num = 2;
				XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
				if (xmlSchemaAnnotation != null)
				{
					xmlSchemaAttributeGroupRef.Annotation = xmlSchemaAnnotation;
				}
			}
			else
			{
				reader.RaiseInvalidElementError();
			}
		}
		return xmlSchemaAttributeGroupRef;
	}
}
