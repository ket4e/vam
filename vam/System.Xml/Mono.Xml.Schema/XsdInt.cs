using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdInt : XsdLong
{
	public override XmlTypeCode TypeCode => XmlTypeCode.Int;

	public override Type ValueType => typeof(int);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToInt32(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is int && y is int)
		{
			if ((int)x == (int)y)
			{
				return XsdOrdering.Equal;
			}
			if ((int)x < (int)y)
			{
				return XsdOrdering.LessThan;
			}
			return XsdOrdering.GreaterThan;
		}
		return XsdOrdering.Indeterminate;
	}
}
