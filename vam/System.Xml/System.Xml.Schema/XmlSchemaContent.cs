namespace System.Xml.Schema;

public abstract class XmlSchemaContent : XmlSchemaAnnotated
{
	internal object actualBaseSchemaType;

	internal virtual bool IsExtension => false;

	internal virtual XmlQualifiedName GetBaseTypeName()
	{
		return null;
	}

	internal virtual XmlSchemaParticle GetParticle()
	{
		return null;
	}
}
