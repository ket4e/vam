using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdPositiveInteger : XsdNonNegativeInteger
{
	public override XmlTypeCode TypeCode => XmlTypeCode.PositiveInteger;

	public override Type ValueType => typeof(decimal);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToDecimal(Normalize(s));
	}
}
