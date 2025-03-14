using System.Collections;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization;

internal class XmlFormatterDeserializer
{
	private KnownTypeCollection types;

	private IDataContractSurrogate surrogate;

	private Hashtable references = new Hashtable();

	public Hashtable References => references;

	private XmlFormatterDeserializer(KnownTypeCollection knownTypes, IDataContractSurrogate surrogate)
	{
		types = knownTypes;
		this.surrogate = surrogate;
	}

	public static object Deserialize(XmlReader reader, Type type, KnownTypeCollection knownTypes, IDataContractSurrogate surrogate, string name, string ns, bool verifyObjectName)
	{
		reader.MoveToContent();
		if (verifyObjectName && (reader.NodeType != XmlNodeType.Element || reader.LocalName != name || reader.NamespaceURI != ns))
		{
			throw new SerializationException($"Expected element '{name}' in namespace '{ns}', but found {reader.NodeType} node '{reader.LocalName}' in namespace '{reader.NamespaceURI}'");
		}
		return new XmlFormatterDeserializer(knownTypes, surrogate).Deserialize(type, reader);
	}

	private static void Verify(KnownTypeCollection knownTypes, Type type, string name, string Namespace, XmlReader reader)
	{
		XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(reader.Name, reader.NamespaceURI);
		if (xmlQualifiedName.Name == name && xmlQualifiedName.Namespace == Namespace)
		{
			return;
		}
		for (Type type2 = type; type2 != null; type2 = type2.BaseType)
		{
			if (knownTypes.GetQName(type2) == xmlQualifiedName)
			{
				return;
			}
		}
		XmlQualifiedName qName = knownTypes.GetQName(type);
		throw new SerializationException($"Expecting element '{qName.Name}' from namespace '{qName.Namespace}'. Encountered 'Element' with name '{xmlQualifiedName.Name}', namespace '{xmlQualifiedName.Namespace}'");
	}

	public object Deserialize(Type type, XmlReader reader)
	{
		string attribute = reader.GetAttribute("Id", "http://schemas.microsoft.com/2003/10/Serialization/");
		object obj = DeserializeCore(type, reader);
		if (attribute != null)
		{
			references.Add(attribute, obj);
		}
		return obj;
	}

	public object DeserializeCore(Type type, XmlReader reader)
	{
		XmlQualifiedName xmlQualifiedName = types.GetQName(type);
		string attribute = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
		if (attribute != null)
		{
			string[] array = attribute.Split(':');
			xmlQualifiedName = ((array.Length <= 1) ? new XmlQualifiedName(attribute, reader.NamespaceURI) : new XmlQualifiedName(array[1], reader.LookupNamespace(reader.NameTable.Get(array[0]))));
		}
		string attribute2 = reader.GetAttribute("Ref", "http://schemas.microsoft.com/2003/10/Serialization/");
		if (attribute2 != null)
		{
			object obj = references[attribute2];
			if (obj == null)
			{
				throw new SerializationException($"Deserialized object with reference Id '{attribute2}' was not found");
			}
			reader.Skip();
			return obj;
		}
		if (reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true")
		{
			reader.Skip();
			if (!type.IsValueType)
			{
				return null;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return null;
			}
			throw new SerializationException($"Value type {type} cannot be null.");
		}
		if (KnownTypeCollection.GetPrimitiveTypeFromName(xmlQualifiedName.Name) != null)
		{
			string s;
			if (reader.IsEmptyElement)
			{
				reader.Read();
				if (type.IsValueType)
				{
					return Activator.CreateInstance(type);
				}
				s = string.Empty;
			}
			else
			{
				s = reader.ReadElementContentAsString();
			}
			return KnownTypeCollection.PredefinedTypeStringToObject(s, xmlQualifiedName.Name, reader);
		}
		return DeserializeByMap(xmlQualifiedName, type, reader);
	}

	private object DeserializeByMap(XmlQualifiedName name, Type type, XmlReader reader)
	{
		SerializationMap serializationMap = types.FindUserMap(name);
		if (serializationMap == null && (name.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/Arrays" || name.Namespace.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal)))
		{
			Type typeFromNamePair = GetTypeFromNamePair(name.Name, name.Namespace);
			types.TryRegister(typeFromNamePair);
			serializationMap = types.FindUserMap(name);
		}
		if (serializationMap == null)
		{
			throw new SerializationException($"Unknown type {type} is used for DataContract with reference of name {name}. Any derived types of a data contract or a data member should be added to KnownTypes.");
		}
		return serializationMap.DeserializeObject(reader, this);
	}

	private Type GetTypeFromNamePair(string name, string ns)
	{
		Type primitiveTypeFromName = KnownTypeCollection.GetPrimitiveTypeFromName(name);
		if (primitiveTypeFromName != null)
		{
			return primitiveTypeFromName;
		}
		if (name.StartsWith("ArrayOf", StringComparison.Ordinal) && ns == "http://schemas.microsoft.com/2003/10/Serialization/Arrays")
		{
			return GetTypeFromNamePair(name.Substring(7), string.Empty).MakeArrayType();
		}
		int length = "http://schemas.datacontract.org/2004/07/".Length;
		string text = ((ns.Length <= length) ? null : ns.Substring(length));
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] array = assembly.GetTypes();
			foreach (Type type in array)
			{
				DataContractAttribute customAttribute = type.GetCustomAttribute<DataContractAttribute>(inherit: true);
				if (customAttribute != null && customAttribute.Name == name && customAttribute.Namespace == ns)
				{
					return type;
				}
				if (text != null && type.Name == name && type.Namespace == text)
				{
					return type;
				}
			}
		}
		throw new XmlException($"Type not found; name: {name}, namespace: {ns}");
	}
}
