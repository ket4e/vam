using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdEntity : XsdName
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.ENTITY;

	public override XmlTypeCode TypeCode => XmlTypeCode.Entity;

	public override Type ValueType => typeof(string);

	internal XsdEntity()
	{
	}
}
