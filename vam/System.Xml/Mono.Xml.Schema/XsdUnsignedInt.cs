using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdUnsignedInt : XsdUnsignedLong
{
	public override XmlTypeCode TypeCode => XmlTypeCode.UnsignedInt;

	public override Type ValueType => typeof(uint);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToUInt32(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is uint && y is uint)
		{
			if ((uint)x == (uint)y)
			{
				return XsdOrdering.Equal;
			}
			if ((uint)x < (uint)y)
			{
				return XsdOrdering.LessThan;
			}
			return XsdOrdering.GreaterThan;
		}
		return XsdOrdering.Indeterminate;
	}
}
