using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdUnsignedShort : XsdUnsignedInt
{
	public override XmlTypeCode TypeCode => XmlTypeCode.UnsignedShort;

	public override Type ValueType => typeof(ushort);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToUInt16(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is ushort && y is ushort)
		{
			if ((ushort)x == (ushort)y)
			{
				return XsdOrdering.Equal;
			}
			if ((ushort)x < (ushort)y)
			{
				return XsdOrdering.LessThan;
			}
			return XsdOrdering.GreaterThan;
		}
		return XsdOrdering.Indeterminate;
	}
}
