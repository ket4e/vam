using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdByte : XsdShort
{
	public override XmlTypeCode TypeCode => XmlTypeCode.Byte;

	public override Type ValueType => typeof(sbyte);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToSByte(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is sbyte && y is sbyte)
		{
			if ((sbyte)x == (sbyte)y)
			{
				return XsdOrdering.Equal;
			}
			if ((sbyte)x < (sbyte)y)
			{
				return XsdOrdering.LessThan;
			}
			return XsdOrdering.GreaterThan;
		}
		return XsdOrdering.Indeterminate;
	}
}
