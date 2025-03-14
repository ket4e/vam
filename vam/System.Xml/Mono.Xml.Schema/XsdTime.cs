using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdTime : XsdAnySimpleType
{
	private static string[] timeFormats = new string[24]
	{
		"HH:mm:ss", "HH:mm:ss.f", "HH:mm:ss.ff", "HH:mm:ss.fff", "HH:mm:ss.ffff", "HH:mm:ss.fffff", "HH:mm:ss.ffffff", "HH:mm:ss.fffffff", "HH:mm:sszzz", "HH:mm:ss.fzzz",
		"HH:mm:ss.ffzzz", "HH:mm:ss.fffzzz", "HH:mm:ss.ffffzzz", "HH:mm:ss.fffffzzz", "HH:mm:ss.ffffffzzz", "HH:mm:ss.fffffffzzz", "HH:mm:ssZ", "HH:mm:ss.fZ", "HH:mm:ss.ffZ", "HH:mm:ss.fffZ",
		"HH:mm:ss.ffffZ", "HH:mm:ss.fffffZ", "HH:mm:ss.ffffffZ", "HH:mm:ss.fffffffZ"
	};

	internal override XmlSchemaFacet.Facet AllowedFacets => XsdAnySimpleType.durationAllowedFacets;

	public override XmlTokenizedType TokenizedType => XmlTokenizedType.CDATA;

	public override XmlTypeCode TypeCode => XmlTypeCode.Time;

	public override Type ValueType => typeof(DateTime);

	public override XsdOrderedFacet Ordered => XsdOrderedFacet.Partial;

	internal XsdTime()
	{
		WhitespaceValue = XsdWhitespaceFacet.Collapse;
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return DateTime.ParseExact(Normalize(s), timeFormats, null, DateTimeStyles.None);
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
