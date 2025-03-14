using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization;

internal class EnumMap : SerializationMap
{
	private List<EnumMemberInfo> enum_members;

	private bool flag_attr;

	public EnumMap(Type type, XmlQualifiedName qname, KnownTypeCollection knownTypes)
		: base(type, qname, knownTypes)
	{
		bool flag = false;
		object[] customAttributes = RuntimeType.GetCustomAttributes(typeof(DataContractAttribute), inherit: false);
		if (customAttributes.Length != 0)
		{
			flag = true;
		}
		flag_attr = type.GetCustomAttributes(typeof(FlagsAttribute), inherit: false).Length > 0;
		enum_members = new List<EnumMemberInfo>();
		BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public;
		FieldInfo[] fields = RuntimeType.GetFields(bindingAttr);
		foreach (FieldInfo fieldInfo in fields)
		{
			string name = fieldInfo.Name;
			if (flag)
			{
				EnumMemberAttribute enumMemberAttribute = GetEnumMemberAttribute(fieldInfo);
				if (enumMemberAttribute == null)
				{
					continue;
				}
				if (enumMemberAttribute.Value != null)
				{
					name = enumMemberAttribute.Value;
				}
			}
			enum_members.Add(new EnumMemberInfo(name, fieldInfo.GetValue(null)));
		}
	}

	private EnumMemberAttribute GetEnumMemberAttribute(MemberInfo mi)
	{
		object[] customAttributes = mi.GetCustomAttributes(typeof(EnumMemberAttribute), inherit: false);
		if (customAttributes.Length == 0)
		{
			return null;
		}
		return (EnumMemberAttribute)customAttributes[0];
	}

	public override XmlSchemaType GetSchemaType(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
	{
		if (generated_schema_types.ContainsKey(base.XmlName))
		{
			return generated_schema_types[base.XmlName];
		}
		XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
		xmlSchemaSimpleType.Name = base.XmlName.Name;
		XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)(xmlSchemaSimpleType.Content = new XmlSchemaSimpleTypeRestriction());
		xmlSchemaSimpleTypeRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		foreach (EnumMemberInfo enum_member in enum_members)
		{
			XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = new XmlSchemaEnumerationFacet();
			xmlSchemaEnumerationFacet.Value = enum_member.XmlName;
			xmlSchemaSimpleTypeRestriction.Facets.Add(xmlSchemaEnumerationFacet);
		}
		generated_schema_types[base.XmlName] = xmlSchemaSimpleType;
		XmlSchema schema = GetSchema(schemas, base.XmlName.Namespace);
		XmlSchemaElement schemaElement = GetSchemaElement(base.XmlName, xmlSchemaSimpleType);
		schemaElement.IsNillable = true;
		schema.Items.Add(xmlSchemaSimpleType);
		schema.Items.Add(schemaElement);
		return xmlSchemaSimpleType;
	}

	public override void Serialize(object graph, XmlFormatterSerializer serializer)
	{
		foreach (EnumMemberInfo enum_member in enum_members)
		{
			if (object.Equals(enum_member.Value, graph))
			{
				serializer.Writer.WriteString(enum_member.XmlName);
				return;
			}
		}
		throw new SerializationException($"Enum value '{graph}' is invalid for type '{RuntimeType}' and cannot be serialized.");
	}

	public override object DeserializeEmptyContent(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		if (!flag_attr)
		{
			throw new SerializationException($"Enum value '' is invalid for type '{RuntimeType}' and cannot be deserialized.");
		}
		return Enum.ToObject(RuntimeType, 0);
	}

	public override object DeserializeContent(XmlReader reader, XmlFormatterDeserializer deserializer)
	{
		string text = ((reader.NodeType == XmlNodeType.Text) ? reader.ReadContentAsString() : string.Empty);
		if (text != string.Empty)
		{
			foreach (EnumMemberInfo enum_member in enum_members)
			{
				if (enum_member.XmlName == text)
				{
					return enum_member.Value;
				}
			}
		}
		if (!flag_attr)
		{
			throw new SerializationException($"Enum value '{text}' is invalid for type '{RuntimeType}' and cannot be deserialized.");
		}
		return Enum.ToObject(RuntimeType, 0);
	}
}
