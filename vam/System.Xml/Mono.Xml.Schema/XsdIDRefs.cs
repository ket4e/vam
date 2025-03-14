using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdIDRefs : XsdName
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.IDREFS;

	[System.MonoTODO]
	public override XmlTypeCode TypeCode => XmlTypeCode.Item;

	public override Type ValueType => typeof(string[]);

	internal XsdIDRefs()
	{
	}

	public override object ParseValue(string value, XmlNameTable nt, IXmlNamespaceResolver nsmgr)
	{
		return GetValidatedArray(value, nt);
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return new StringArrayValueType(GetValidatedArray(s, nameTable));
	}

	private string[] GetValidatedArray(string value, XmlNameTable nt)
	{
		string[] array = ParseListValue(value, nt);
		for (int i = 0; i < array.Length; i++)
		{
			XmlConvert.VerifyNCName(array[i]);
		}
		return array;
	}
}
