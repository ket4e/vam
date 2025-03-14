using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization;

internal class DictionaryTypeMap : SerializationMap, ICollectionTypeMap
{
	private Type key_type;

	private Type value_type;

	private XmlQualifiedName dict_qname;

	private XmlQualifiedName item_qname;

	private XmlQualifiedName key_qname;

	private XmlQualifiedName value_qname;

	private MethodInfo add_method;

	private CollectionDataContractAttribute a;

	private static readonly XmlQualifiedName kvpair_key_qname = new XmlQualifiedName("Key", "http://schemas.microsoft.com/2003/10/Serialization/Arrays");

	private static readonly XmlQualifiedName kvpair_value_qname = new XmlQualifiedName("Value", "http://schemas.microsoft.com/2003/10/Serialization/Arrays");

	private Type pair_type;

	private PropertyInfo pair_key_property;

	private PropertyInfo pair_value_property;

	private string ContractNamespace => (a == null || string.IsNullOrEmpty(a.Namespace)) ? "http://schemas.microsoft.com/2003/10/Serialization/Arrays" : a.Namespace;

	public Type KeyType => key_type;

	public Type ValueType => value_type;

	internal virtual string CurrentNamespace
	{
		get
		{
			string text = item_qname.Namespace;
			if (text == "http://schemas.microsoft.com/2003/10/Serialization/")
			{
				text = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
			}
			return text;
		}
	}

	public DictionaryTypeMap(Type type, CollectionDataContractAttribute a, KnownTypeCollection knownTypes)
		: base(type, XmlQualifiedName.Empty, knownTypes)
	{
		this.a = a;
		key_type = typeof(object);
		value_type = typeof(object);
		Type genericDictionaryInterface = GetGenericDictionaryInterface(RuntimeType);
		if (genericDictionaryInterface != null)
		{
			InterfaceMapping interfaceMap = RuntimeType.GetInterfaceMap(genericDictionaryInterface);
			for (int i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
			{
				if (interfaceMap.InterfaceMethods[i].Name == "Add")
				{
					add_method = interfaceMap.TargetMethods[i];
					break;
				}
			}
			Type[] genericArguments = genericDictionaryInterface.GetGenericArguments();
			key_type = genericArguments[0];
			value_type = genericArguments[1];
			if (add_method == null)
			{
				add_method = type.GetMethod("Add", genericArguments);
			}
		}
		base.XmlName = GetDictionaryQName();
		item_qname = GetItemQName();
		key_qname = GetKeyQName();
		value_qname = GetValueQName();
	}

	private static Type GetGenericDictionaryInterface(Type type)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(IDictionary<, >))
			{
				return type2;
			}
		}
		return null;
	}

	internal virtual XmlQualifiedName GetDictionaryQName()
	{
		if (a != null && !string.IsNullOrEmpty(a.Name))
		{
			return new XmlQualifiedName(a.Name, ContractNamespace);
		}
		return new XmlQualifiedName("ArrayOf" + GetItemQName().Name, "http://schemas.microsoft.com/2003/10/Serialization/Arrays");
	}

	internal virtual XmlQualifiedName GetItemQName()
	{
		if (a != null && !string.IsNullOrEmpty(a.ItemName))
		{
			return new XmlQualifiedName(a.ItemName, ContractNamespace);
		}
		return new XmlQualifiedName("KeyValueOf" + KnownTypes.GetQName(key_type).Name + KnownTypes.GetQName(value_type).Name, "http://schemas.microsoft.com/2003/10/Serialization/Arrays");
	}

	internal virtual XmlQualifiedName GetKeyQName()
	{
		if (a != null && !string.IsNullOrEmpty(a.KeyName))
		{
			return new XmlQualifiedName(a.KeyName, ContractNamespace);
		}
		return kvpair_key_qname;
	}

	internal virtual XmlQualifiedName GetValueQName()
	{
		if (a != null && !string.IsNullOrEmpty(a.ValueName))
		{
			return new XmlQualifiedName(a.ValueName, ContractNamespace);
		}
		return kvpair_value_qname;
	}

	public override void SerializeNonReference(object graph, XmlFormatterSerializer serializer)
	{
		if (add_method != null)
		{
			if (pair_type == null)
			{
				pair_type = typeof(KeyValuePair<, >).MakeGenericType(add_method.DeclaringType.GetGenericArguments());
				pair_key_property = pair_type.GetProperty("Key");
				pair_value_property = pair_type.GetProperty("Value");
			}
			{
				foreach (object item in (IEnumerable)graph)
				{
					serializer.WriteStartElement(item_qname.Name, item_qname.Namespace, CurrentNamespace);
					serializer.WriteStartElement(key_qname.Name, key_qname.Namespace, CurrentNamespace);
					serializer.Serialize(pair_key_property.PropertyType, pair_key_property.GetValue(item, null));
					serializer.WriteEndElement();
					serializer.WriteStartElement(value_qname.Name, value_qname.Namespace, CurrentNamespace);
					serializer.Serialize(pair_value_property.PropertyType, pair_value_property.GetValue(item, null));
					serializer.WriteEndElement();
					serializer.WriteEndElement();
				}
				return;
			}
		}
		foreach (DictionaryEntry item2 in (IEnumerable)graph)
		{
			serializer.WriteStartElement(item_qname.Name, item_qname.Namespace, CurrentNamespace);
			serializer.WriteStartElement(key_qname.Name, key_qname.Namespace, CurrentNamespace);
			serializer.Serialize(key_type, item2.Key);
			serializer.WriteEndElement();
			serializer.WriteStartElement(value_qname.Name, value_qname.Namespace, CurrentNamespace);
			serializer.Serialize(value_type, item2.Value);
			serializer.WriteEndElement();
			serializer.WriteEndElement();
		}
	}

	private object CreateInstance()
	{
		if (RuntimeType.IsInterface)
		{
			if (RuntimeType.IsGenericType && Array.IndexOf(RuntimeType.GetGenericTypeDefinition().GetInterfaces(), typeof(IDictionary<, >)) >= 0)
			{
				Type[] genericArguments = RuntimeType.GetGenericArguments();
				return Activator.CreateInstance(typeof(Dictionary<, >).MakeGenericType(genericArguments[0], genericArguments[1]));
			}
			return new Hashtable();
		}
		return Activator.CreateInstance(RuntimeType, nonPublic: true);
	}

	public override object DeserializeEmptyContent(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		return DeserializeContent(reader, deserializer);
	}

	public override object DeserializeContent(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		object obj = CreateInstance();
		int num = ((reader.NodeType != 0) ? (reader.Depth - 1) : reader.Depth);
		while (reader.NodeType == XmlNodeType.Element && reader.Depth > num)
		{
			if (reader.IsEmptyElement)
			{
				throw new XmlException($"Unexpected empty element for dictionary entry: name {reader.Name}");
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			object obj2 = deserializer.Deserialize(key_type, reader);
			reader.MoveToContent();
			object obj3 = deserializer.Deserialize(value_type, reader);
			reader.ReadEndElement();
			if (obj is IDictionary)
			{
				((IDictionary)obj).Add(obj2, obj3);
				continue;
			}
			if (add_method != null)
			{
				add_method.Invoke(obj, new object[2] { obj2, obj3 });
				continue;
			}
			throw new NotImplementedException($"Type {RuntimeType} is not supported");
		}
		return obj;
	}

	public override List<DataMemberInfo> GetMembers()
	{
		throw new NotImplementedException();
	}

	public override XmlSchemaType GetSchemaType(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
	{
		throw new NotImplementedException();
	}
}
