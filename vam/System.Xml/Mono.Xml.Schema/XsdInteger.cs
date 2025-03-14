using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdInteger : XsdDecimal
{
	public override XmlTypeCode TypeCode => XmlTypeCode.Integer;

	public override Type ValueType => typeof(decimal);

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return ParseValueType(s, nameTable, nsmgr);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		decimal num = XmlConvert.ToDecimal(Normalize(s));
		if (decimal.Floor(num) != num)
		{
			throw new FormatException("Integer contains point number.");
		}
		return num;
	}
}
