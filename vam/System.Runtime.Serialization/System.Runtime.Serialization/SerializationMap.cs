using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization;

internal abstract class SerializationMap
{
	public const BindingFlags AllInstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public readonly KnownTypeCollection KnownTypes;

	public readonly Type RuntimeType;

	public bool IsReference;

	public List<DataMemberInfo> Members;

	private XmlSchemaSet schema_set;

	private Dictionary<Type, XmlQualifiedName> qname_table = new Dictionary<Type, XmlQualifiedName>();

	public virtual bool OutputXsiType => true;

	public XmlQualifiedName XmlName { get; set; }

	protected SerializationMap(Type type, XmlQualifiedName qname, KnownTypeCollection knownTypes)
	{
		KnownTypes = knownTypes;
		RuntimeType = type;
		if (qname.Namespace == null)
		{
			qname = new XmlQualifiedName(qname.Name, "http://schemas.datacontract.org/2004/07/" + type.Namespace);
		}
		XmlName = qname;
		Members = new List<DataMemberInfo>();
	}

	public CollectionDataContractAttribute GetCollectionDataContractAttribute(Type type)
	{
		object[] customAttributes = type.GetCustomAttributes(typeof(CollectionDataContractAttribute), inherit: false);
		return (customAttributes.Length != 0) ? ((CollectionDataContractAttribute)customAttributes[0]) : null;
	}

	public DataMemberAttribute GetDataMemberAttribute(MemberInfo mi)
	{
		object[] customAttributes = mi.GetCustomAttributes(typeof(DataMemberAttribute), inherit: false);
		if (customAttributes.Length == 0)
		{
			return null;
		}
		return (DataMemberAttribute)customAttributes[0];
	}

	private bool IsPrimitive(Type type)
	{
		return Type.GetTypeCode(type) != TypeCode.Object || type == typeof(object);
	}

	public virtual XmlSchemaType GetSchemaType(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
	{
		if (IsPrimitive(RuntimeType))
		{
			return null;
		}
		if (generated_schema_types.ContainsKey(XmlName))
		{
			return generated_schema_types[XmlName];
		}
		XmlSchemaComplexType xmlSchemaComplexType = null;
		xmlSchemaComplexType = new XmlSchemaComplexType();
		xmlSchemaComplexType.Name = XmlName.Name;
		generated_schema_types[XmlName] = xmlSchemaComplexType;
		if (RuntimeType.BaseType == typeof(object))
		{
			xmlSchemaComplexType.Particle = GetSequence(schemas, generated_schema_types);
		}
		else
		{
			XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = new XmlSchemaComplexContentExtension();
			XmlSchemaComplexContent xmlSchemaComplexContent = (XmlSchemaComplexContent)(xmlSchemaComplexType.ContentModel = new XmlSchemaComplexContent());
			xmlSchemaComplexContent.Content = xmlSchemaComplexContentExtension;
			KnownTypes.Add(RuntimeType.BaseType);
			SerializationMap serializationMap = KnownTypes.FindUserMap(RuntimeType.BaseType);
			serializationMap.GetSchemaType(schemas, generated_schema_types);
			xmlSchemaComplexContentExtension.Particle = GetSequence(schemas, generated_schema_types);
			xmlSchemaComplexContentExtension.BaseTypeName = GetQualifiedName(RuntimeType.BaseType);
		}
		XmlSchemaElement schemaElement = GetSchemaElement(XmlName, xmlSchemaComplexType);
		XmlSchema schema = GetSchema(schemas, XmlName.Namespace);
		schema.Items.Add(xmlSchemaComplexType);
		schema.Items.Add(schemaElement);
		schemas.Reprocess(schema);
		return xmlSchemaComplexType;
	}

	private XmlSchemaSequence GetSequence(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
	{
		List<DataMemberInfo> members = GetMembers();
		XmlSchema schema = GetSchema(schemas, XmlName.Namespace);
		XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
		foreach (DataMemberInfo item in members)
		{
			if (!item.MemberType.IsAbstract && typeof(Delegate).IsAssignableFrom(item.MemberType))
			{
				continue;
			}
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = item.XmlName;
			KnownTypes.Add(item.MemberType);
			SerializationMap serializationMap = KnownTypes.FindUserMap(item.MemberType);
			if (serializationMap != null)
			{
				XmlSchemaType schemaType = serializationMap.GetSchemaType(schemas, generated_schema_types);
				if (schemaType is XmlSchemaComplexType)
				{
					xmlSchemaElement.IsNillable = true;
				}
			}
			else if (item.MemberType == typeof(string))
			{
				xmlSchemaElement.IsNillable = true;
			}
			xmlSchemaElement.MinOccurs = 0m;
			xmlSchemaElement.SchemaTypeName = GetQualifiedName(item.MemberType);
			AddImport(schema, xmlSchemaElement.SchemaTypeName.Namespace);
			xmlSchemaSequence.Items.Add(xmlSchemaElement);
		}
		schemas.Reprocess(schema);
		return xmlSchemaSequence;
	}

	private void AddImport(XmlSchema schema, string ns)
	{
		if (ns == "http://www.w3.org/2001/XMLSchema" || schema.TargetNamespace == ns)
		{
			return;
		}
		foreach (XmlSchemaObject include in schema.Includes)
		{
			if (!(include is XmlSchemaImport xmlSchemaImport) || !(xmlSchemaImport.Namespace == ns))
			{
				continue;
			}
			return;
		}
		XmlSchemaImport xmlSchemaImport2 = new XmlSchemaImport();
		xmlSchemaImport2.Namespace = ns;
		schema.Includes.Add(xmlSchemaImport2);
	}

	public virtual List<DataMemberInfo> GetMembers()
	{
		throw new NotImplementedException($"Implement me for {this}");
	}

	protected XmlSchemaElement GetSchemaElement(XmlQualifiedName qname, XmlSchemaType schemaType)
	{
		XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
		xmlSchemaElement.Name = qname.Name;
		xmlSchemaElement.SchemaTypeName = qname;
		if (schemaType is XmlSchemaComplexType)
		{
			xmlSchemaElement.IsNillable = true;
		}
		return xmlSchemaElement;
	}

	protected XmlSchema GetSchema(XmlSchemaSet schemas, string ns)
	{
		ICollection collection = schemas.Schemas(ns);
		if (collection.Count > 0)
		{
			if (collection.Count > 1)
			{
				throw new Exception($"More than 1 schema for namespace '{ns}' found.");
			}
			{
				IEnumerator enumerator = collection.GetEnumerator();
				try
				{
					if (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						return current as XmlSchema;
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
		XmlSchema xmlSchema = new XmlSchema();
		xmlSchema.TargetNamespace = ns;
		xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
		schemas.Add(xmlSchema);
		return xmlSchema;
	}

	protected XmlQualifiedName GetQualifiedName(Type type)
	{
		if (qname_table.ContainsKey(type))
		{
			return qname_table[type];
		}
		XmlQualifiedName xmlQualifiedName = KnownTypes.GetQName(type);
		if (xmlQualifiedName.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
		{
			xmlQualifiedName = new XmlQualifiedName(xmlQualifiedName.Name, "http://www.w3.org/2001/XMLSchema");
		}
		qname_table[type] = xmlQualifiedName;
		return xmlQualifiedName;
	}

	public virtual void Serialize(object graph, XmlFormatterSerializer serializer)
	{
		string text = null;
		if (IsReference)
		{
			text = (string)serializer.References[graph];
			if (text != null)
			{
				serializer.Writer.WriteAttributeString("z", "Ref", "http://schemas.microsoft.com/2003/10/Serialization/", text);
				return;
			}
			text = "i" + (serializer.References.Count + 1);
			serializer.References.Add(graph, text);
		}
		else if (serializer.SerializingObjects.Contains(graph))
		{
			throw new SerializationException($"Circular reference of an object in the object graph was found: '{graph}' of type {graph.GetType()}");
		}
		serializer.SerializingObjects.Add(graph);
		if (text != null)
		{
			serializer.Writer.WriteAttributeString("z", "Id", "http://schemas.microsoft.com/2003/10/Serialization/", text);
		}
		SerializeNonReference(graph, serializer);
		serializer.SerializingObjects.Remove(graph);
	}

	public virtual void SerializeNonReference(object graph, XmlFormatterSerializer serializer)
	{
		foreach (DataMemberInfo member in Members)
		{
			FieldInfo fieldInfo = member.Member as FieldInfo;
			PropertyInfo propertyInfo = ((fieldInfo != null) ? null : ((PropertyInfo)member.Member));
			Type type = ((fieldInfo == null) ? propertyInfo.PropertyType : fieldInfo.FieldType);
			object graph2 = ((fieldInfo == null) ? propertyInfo.GetValue(graph, null) : fieldInfo.GetValue(graph));
			serializer.WriteStartElement(member.XmlName, member.XmlRootNamespace, member.XmlNamespace);
			serializer.Serialize(type, graph2);
			serializer.WriteEndElement();
		}
	}

	public virtual object DeserializeObject(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		bool isEmptyElement = reader.IsEmptyElement;
		reader.ReadStartElement();
		reader.MoveToContent();
		object result = ((!isEmptyElement) ? DeserializeContent(reader, deserializer) : DeserializeEmptyContent(reader, deserializer));
		reader.MoveToContent();
		if (!isEmptyElement && reader.NodeType == XmlNodeType.EndElement)
		{
			reader.ReadEndElement();
		}
		else if (!isEmptyElement && reader.NodeType != 0)
		{
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			throw new SerializationException(string.Format("Deserializing type '{3}'. Expecting state 'EndElement'. Encountered state '{0}' with name '{1}' with namespace '{2}'.{4}", reader.NodeType, reader.Name, reader.NamespaceURI, RuntimeType.FullName, (xmlLineInfo == null || !xmlLineInfo.HasLineInfo()) ? string.Empty : $" {reader.BaseURI}({xmlLineInfo.LineNumber},{xmlLineInfo.LinePosition})"));
		}
		return result;
	}

	public virtual object DeserializeEmptyContent(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		return DeserializeContent(reader, deserializer, empty: true);
	}

	public virtual object DeserializeContent(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		return DeserializeContent(reader, deserializer, empty: false);
	}

	private object DeserializeContent(XmlReader reader, XmlFormatterDeserializer deserializer, bool empty)
	{
		object uninitializedObject = FormatterServices.GetUninitializedObject(RuntimeType);
		int num = ((reader.NodeType != 0) ? (reader.Depth - 1) : reader.Depth);
		bool[] array = new bool[Members.Count];
		int num2 = -1;
		int val = -1;
		while (!empty && reader.NodeType == XmlNodeType.Element && reader.Depth > num)
		{
			DataMemberInfo dataMemberInfo = null;
			int i;
			for (i = 0; i < Members.Count && Members[i].Order < 0; i++)
			{
				if (reader.LocalName == Members[i].XmlName && reader.NamespaceURI == Members[i].XmlRootNamespace)
				{
					num2 = i;
					dataMemberInfo = Members[i];
					break;
				}
			}
			for (i = Math.Max(i, val); i < Members.Count; i++)
			{
				if (dataMemberInfo != null)
				{
					break;
				}
				if (reader.LocalName == Members[i].XmlName && reader.NamespaceURI == Members[i].XmlRootNamespace)
				{
					num2 = i;
					val = i;
					dataMemberInfo = Members[i];
					break;
				}
			}
			if (dataMemberInfo == null)
			{
				reader.Skip();
				continue;
			}
			SetValue(dataMemberInfo, uninitializedObject, deserializer.Deserialize(dataMemberInfo.MemberType, reader));
			array[num2] = true;
			reader.MoveToContent();
		}
		for (int j = 0; j < Members.Count; j++)
		{
			if (!array[j] && Members[j].IsRequired)
			{
				throw MissingRequiredMember(Members[j], reader);
			}
		}
		return uninitializedObject;
	}

	protected Exception MissingRequiredMember(DataMemberInfo dmi, XmlReader reader)
	{
		IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
		return new ArgumentException($"Data contract member {new XmlQualifiedName(dmi.XmlName, dmi.XmlNamespace)} for the type {RuntimeType} is required, but missing in the input XML.{((xmlLineInfo == null || !xmlLineInfo.HasLineInfo()) ? null : $" {reader.BaseURI}({xmlLineInfo.LineNumber},{xmlLineInfo.LinePosition})")}");
	}

	protected void SetValue(DataMemberInfo dmi, object obj, object value)
	{
		try
		{
			if (dmi.Member is PropertyInfo)
			{
				((PropertyInfo)dmi.Member).SetValue(obj, value, null);
			}
			else
			{
				((FieldInfo)dmi.Member).SetValue(obj, value);
			}
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException($"Failed to set value of type {value?.GetType()} for property {dmi.Member}", innerException);
		}
	}

	protected DataMemberInfo CreateDataMemberInfo(DataMemberAttribute dma, MemberInfo mi, Type type)
	{
		KnownTypes.Add(type);
		XmlQualifiedName qName = KnownTypes.GetQName(type);
		string @namespace = KnownTypes.GetQName(mi.DeclaringType).Namespace;
		if (KnownTypeCollection.GetPrimitiveTypeFromName(qName.Name) != null)
		{
			return new DataMemberInfo(mi, dma, @namespace, null);
		}
		return new DataMemberInfo(mi, dma, @namespace, qName.Namespace);
	}
}
