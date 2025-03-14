using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdQName : XsdName
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.QName;

	public override XmlTypeCode TypeCode => XmlTypeCode.QName;

	public override Type ValueType => typeof(XmlQualifiedName);

	internal XsdQName()
	{
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		if (nameTable == null)
		{
			throw new ArgumentNullException("name table");
		}
		if (nsmgr == null)
		{
			throw new ArgumentNullException("namespace manager");
		}
		XmlQualifiedName xmlQualifiedName = XmlQualifiedName.Parse(s, nsmgr, considerDefaultNamespace: true);
		nameTable.Add(xmlQualifiedName.Name);
		nameTable.Add(xmlQualifiedName.Namespace);
		return xmlQualifiedName;
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return new QNameValueType(ParseValue(s, nameTable, nsmgr) as XmlQualifiedName);
	}
}
