using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdBoolean : XsdAnySimpleType
{
	internal override XmlSchemaFacet.Facet AllowedFacets => XsdAnySimpleType.booleanAllowedFacets;

	public override XmlTokenizedType TokenizedType
	{
		get
		{
			if (XmlSchemaUtil.StrictMsCompliant)
			{
				return XmlTokenizedType.None;
			}
			return XmlTokenizedType.CDATA;
		}
	}

	public override XmlTypeCode TypeCode => XmlTypeCode.Boolean;

	public override Type ValueType => typeof(bool);

	public override bool Bounded => false;

	public override bool Finite => true;

	public override bool Numeric => false;

	public override XsdOrderedFacet Ordered => XsdOrderedFacet.Total;

	internal XsdBoolean()
	{
		WhitespaceValue = XsdWhitespaceFacet.Collapse;
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToBoolean(Normalize(s));
	}
}
