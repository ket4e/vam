using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdDecimal : XsdAnySimpleType
{
	internal override XmlSchemaFacet.Facet AllowedFacets => XsdAnySimpleType.decimalAllowedFacets;

	public override XmlTokenizedType TokenizedType => XmlTokenizedType.None;

	public override XmlTypeCode TypeCode => XmlTypeCode.Decimal;

	public override Type ValueType => typeof(decimal);

	public override bool Bounded => false;

	public override bool Finite => false;

	public override bool Numeric => true;

	public override XsdOrderedFacet Ordered => XsdOrderedFacet.Total;

	internal XsdDecimal()
	{
		WhitespaceValue = XsdWhitespaceFacet.Collapse;
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToDecimal(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is decimal && y is decimal)
		{
			int num = decimal.Compare((decimal)x, (decimal)y);
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
