using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XdtYearMonthDuration : XsdDuration
{
	public override XmlTypeCode TypeCode => XmlTypeCode.YearMonthDuration;

	public override Type ValueType => typeof(TimeSpan);

	public override bool Bounded => false;

	public override bool Finite => false;

	public override bool Numeric => false;

	public override XsdOrderedFacet Ordered => XsdOrderedFacet.Partial;

	internal XdtYearMonthDuration()
	{
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToTimeSpan(Normalize(s));
	}
}
