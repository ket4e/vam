using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdShort : XsdInt
{
	public override XmlTypeCode TypeCode => XmlTypeCode.Short;

	public override Type ValueType => typeof(short);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return XmlConvert.ToInt16(Normalize(s));
	}

	internal override XsdOrdering Compare(object x, object y)
	{
		if (x is short && y is short)
		{
			if ((short)x == (short)y)
			{
				return XsdOrdering.Equal;
			}
			if ((short)x < (short)y)
			{
				return XsdOrdering.LessThan;
			}
			return XsdOrdering.GreaterThan;
		}
		return XsdOrdering.Indeterminate;
	}
}
