using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdDouble : XsdAnySimpleType
{
	internal override XmlSchemaFacet.Facet AllowedFacets => XsdAnySimpleType.durationAllowedFacets;

	public override bool Bounded => true;

	public override bool Finite => true;

	public override bool Numeric => true;

	public override XsdOrderedFacet Ordered => XsdOrderedFacet.Total;

	public override XmlTypeCode TypeCode => XmlTypeCode.Double;

	public override Type ValueType => typeof(double);

	internal XsdDouble()
	{
		WhitespaceValue = XsdWhitespaceFacet.Collapse;
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToDouble(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is double && y is double)
		{
			if ((double)x == (double)y)
			{
				return XsdOrdering.Equal;
			}
			if ((double)x < (double)y)
			{
				return XsdOrdering.LessThan;
			}
			return XsdOrdering.GreaterThan;
		}
		return XsdOrdering.Indeterminate;
	}
}
