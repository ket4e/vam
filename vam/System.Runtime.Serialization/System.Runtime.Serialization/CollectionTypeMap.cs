using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization;

internal class CollectionTypeMap : SerializationMap, ICollectionTypeMap
{
	private Type element_type;

	internal XmlQualifiedName element_qname;

	private MethodInfo add_method;

	public override bool OutputXsiType => false;

	internal virtual string CurrentNamespace
	{
		get
		{
			string text = element_qname.Namespace;
			if (text == "http://schemas.microsoft.com/2003/10/Serialization/")
			{
				text = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
			}
			return text;
		}
	}

	public CollectionTypeMap(Type type, Type elementType, XmlQualifiedName qname, KnownTypeCollection knownTypes)
		: base(type, qname, knownTypes)
	{
		element_type = elementType;
		element_qname = KnownTypes.GetQName(element_type);
		Type genericCollectionInterface = GetGenericCollectionInterface(RuntimeType);
		if (genericCollectionInterface == null)
		{
			return;
		}
		if (RuntimeType.IsInterface)
		{
			add_method = RuntimeType.GetMethod("Add", genericCollectionInterface.GetGenericArguments());
			return;
		}
		InterfaceMapping interfaceMap = RuntimeType.GetInterfaceMap(genericCollectionInterface);
		for (int i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
		{
			if (interfaceMap.InterfaceMethods[i].Name == "Add")
			{
				add_method = interfaceMap.TargetMethods[i];
				break;
			}
		}
		if (add_method == null)
		{
			add_method = type.GetMethod("Add", genericCollectionInterface.GetGenericArguments());
		}
	}

	private static Type GetGenericCollectionInterface(Type type)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(ICollection<>))
			{
				return type2;
			}
		}
		return null;
	}

	public override void SerializeNonReference(object graph, XmlFormatterSerializer serializer)
	{
		foreach (object item in (IEnumerable)graph)
		{
			serializer.WriteStartElement(element_qname.Name, base.XmlName.Namespace, CurrentNamespace);
			serializer.Serialize(element_type, item);
			serializer.WriteEndElement();
		}
	}

	private object CreateInstance()
	{
		if (RuntimeType.IsArray)
		{
			return new ArrayList();
		}
		if (RuntimeType.IsInterface)
		{
			Type genericCollectionInterface = GetGenericCollectionInterface(RuntimeType);
			if (genericCollectionInterface != null)
			{
				return Activator.CreateInstance(typeof(List<>).MakeGenericType(RuntimeType.GetGenericArguments()[0]));
			}
			return new ArrayList();
		}
		return Activator.CreateInstance(RuntimeType, nonPublic: true);
	}

	public override object DeserializeEmptyContent(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		return CreateInstance();
	}

	public override object DeserializeContent(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		object obj = CreateInstance();
		int num = ((reader.NodeType != 0) ? (reader.Depth - 1) : reader.Depth);
		while (reader.NodeType == XmlNodeType.Element && reader.Depth > num)
		{
			object obj2 = deserializer.Deserialize(element_type, reader);
			if (obj is IList)
			{
				((IList)obj).Add(obj2);
			}
			else
			{
				if (add_method == null)
				{
					throw new NotImplementedException($"Type {RuntimeType} is not supported");
				}
				add_method.Invoke(obj, new object[1] { obj2 });
			}
			reader.MoveToContent();
		}
		if (RuntimeType.IsArray)
		{
			return ((ArrayList)obj).ToArray(element_type);
		}
		return obj;
	}

	public override List<DataMemberInfo> GetMembers()
	{
		throw new NotImplementedException();
	}

	public override XmlSchemaType GetSchemaType(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
	{
		if (generated_schema_types.ContainsKey(base.XmlName))
		{
			return null;
		}
		if (generated_schema_types.ContainsKey(base.XmlName))
		{
			return generated_schema_types[base.XmlName];
		}
		XmlQualifiedName qualifiedName = GetQualifiedName(element_type);
		XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
		xmlSchemaComplexType.Name = base.XmlName.Name;
		XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
		XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
		xmlSchemaElement.MinOccurs = 0m;
		xmlSchemaElement.MaxOccursString = "unbounded";
		xmlSchemaElement.Name = qualifiedName.Name;
		KnownTypes.Add(element_type);
		SerializationMap serializationMap = KnownTypes.FindUserMap(element_type);
		if (serializationMap != null)
		{
			serializationMap.GetSchemaType(schemas, generated_schema_types);
			xmlSchemaElement.IsNillable = true;
		}
		xmlSchemaElement.SchemaTypeName = qualifiedName;
		xmlSchemaSequence.Items.Add(xmlSchemaElement);
		xmlSchemaComplexType.Particle = xmlSchemaSequence;
		XmlSchema schema = GetSchema(schemas, base.XmlName.Namespace);
		schema.Items.Add(xmlSchemaComplexType);
		schema.Items.Add(GetSchemaElement(base.XmlName, xmlSchemaComplexType));
		schemas.Reprocess(schema);
		generated_schema_types[base.XmlName] = xmlSchemaComplexType;
		return xmlSchemaComplexType;
	}
}
