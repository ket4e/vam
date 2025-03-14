namespace System.Xml.Schema;

public abstract class XmlSchemaSimpleTypeContent : XmlSchemaAnnotated
{
	internal XmlSchemaSimpleType OwnerType;

	internal object ActualBaseSchemaType => OwnerType.BaseSchemaType;

	internal virtual string Normalize(string s, XmlNameTable nt, XmlNamespaceManager nsmgr)
	{
		return s;
	}
}
