using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdDateTime : XsdAnySimpleType
{
	internal override XmlSchemaFacet.Facet AllowedFacets => XsdAnySimpleType.durationAllowedFacets;

	public override XmlTokenizedType TokenizedType => XmlTokenizedType.CDATA;

	public override XmlTypeCode TypeCode => XmlTypeCode.DateTime;

	public override Type ValueType => typeof(DateTime);

	public override bool Bounded => false;

	public override bool Finite => false;

	public override bool Numeric => false;

	public override XsdOrderedFacet Ordered => XsdOrderedFacet.Partial;

	internal XsdDateTime()
	{
		WhitespaceValue = XsdWhitespaceFacet.Collapse;
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToDateTime(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is DateTime && y is DateTime)
		{
			int num = DateTime.Compare((DateTime)x, (DateTime)y);
			if (num < 0)
			{
				return XsdOrdering.LessThan;
			}
			if (num > 0)
			{
				return XsdOrdering.GreaterThan;
			}
			return XsdOrdering.Equal;
		}
		return XsdOrdering.Indeterminate;
	}
}
