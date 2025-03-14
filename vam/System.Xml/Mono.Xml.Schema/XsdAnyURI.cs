using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdAnyURI : XsdString
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.CDATA;

	public override XmlTypeCode TypeCode => XmlTypeCode.AnyUri;

	public override Type ValueType => typeof(Uri);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return new XmlSchemaUri(Normalize(s));
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return new UriValueType((XmlSchemaUri)ParseValue(s, nameTable, nsmgr));
	}
}
