using System.Xml;

namespace System.Runtime.Serialization;

internal class CollectionContractTypeMap : CollectionTypeMap
{
	internal override string CurrentNamespace => base.XmlName.Namespace;

	public CollectionContractTypeMap(Type type, CollectionDataContractAttribute a, Type elementType, XmlQualifiedName qname, KnownTypeCollection knownTypes)
		: base(type, elementType, qname, knownTypes)
	{
		IsReference = a.IsReference;
	}
}
