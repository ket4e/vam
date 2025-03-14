using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdUnsignedByte : XsdUnsignedShort
{
	public override XmlTypeCode TypeCode => XmlTypeCode.UnsignedByte;

	public override Type ValueType => typeof(byte);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToByte(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is byte && y is byte)
		{
			if ((byte)x == (byte)y)
			{
				return XsdOrdering.Equal;
			}
			if ((byte)x < (byte)y)
			{
				return XsdOrdering.LessThan;
			}
			return XsdOrdering.GreaterThan;
		}
		return XsdOrdering.Indeterminate;
	}
}
