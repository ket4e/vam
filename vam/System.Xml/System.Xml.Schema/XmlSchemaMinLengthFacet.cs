namespace System.Xml.Schema;

public class XmlSchemaMinLengthFacet : XmlSchemaNumericFacet
{
	private const string xmlname = "minLength";

	internal override Facet ThisFacet => Facet.minLength;

	internal static XmlSchemaMinLengthFacet Read(XmlSchemaReader reader, ValidationEventHandler h)
	{
		XmlSchemaMinLengthFacet xmlSchemaMinLengthFacet = new XmlSchemaMinLengthFacet();
		reader.MoveToElement();
		if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "minLength")
		{
			XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaMinLengthFacet.Read, name=" + reader.Name, null);
			reader.Skip();
			return null;
		}
		xmlSchemaMinLengthFacet.LineNumber = reader.LineNumber;
		xmlSchemaMinLengthFacet.LinePosition = reader.LinePosition;
		xmlSchemaMinLengthFacet.SourceUri = reader.BaseURI;
		while (reader.MoveToNextAttribute())
		{
			if (reader.Name == "id")
			{
				xmlSchemaMinLengthFacet.Id = reader.Value;
			}
			else if (reader.Name == "fixed")
			{
				xmlSchemaMinLengthFacet.IsFixed = XmlSchemaUtil.ReadBoolAttribute(reader, out var innerExcpetion);
				if (innerExcpetion != null)
				{
					XmlSchemaObject.error(h, reader.Value + " is not a valid value for fixed attribute", innerExcpetion);
				}
			}
			else if (reader.Name == "value")
			{
				xmlSchemaMinLengthFacet.Value = reader.Value;
			}
			else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for minLength", null);
			}
			else
			{
				XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaMinLengthFacet);
			}
		}
		reader.MoveToElement();
		if (reader.IsEmptyElement)
		{
			return xmlSchemaMinLengthFacet;
		}
		int num = 1;
		while (reader.ReadNextElement())
		{
			if (reader.NodeType == XmlNodeType.EndElement)
			{
				if (reader.LocalName != "minLength")
				{
					XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaMinLengthFacet.Read, name=" + reader.Name, null);
				}
				break;
			}
			if (num <= 1 && reader.LocalName == "annotation")
			{
				num = 2;
				XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
				if (xmlSchemaAnnotation != null)
				{
					xmlSchemaMinLengthFacet.Annotation = xmlSchemaAnnotation;
				}
			}
			else
			{
				reader.RaiseInvalidElementError();
			}
		}
		return xmlSchemaMinLengthFacet;
	}
}
