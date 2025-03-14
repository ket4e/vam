using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization;

internal class SharedContractMap : SerializationMap
{
	public SharedContractMap(Type type, XmlQualifiedName qname, KnownTypeCollection knownTypes)
		: base(type, qname, knownTypes)
	{
	}

	internal void Initialize()
	{
		Type type = RuntimeType;
		List<DataMemberInfo> list = new List<DataMemberInfo>();
		object[] customAttributes = type.GetCustomAttributes(typeof(DataContractAttribute), inherit: false);
		IsReference = customAttributes.Length > 0 && ((DataContractAttribute)customAttributes[0]).IsReference;
		while (type != null)
		{
			XmlQualifiedName qName = KnownTypes.GetQName(type);
			list = GetMembers(type, qName, declared_only: true);
			list.Sort(DataMemberInfo.DataMemberInfoComparer.Instance);
			Members.InsertRange(0, list);
			list.Clear();
			type = type.BaseType;
		}
	}

	private List<DataMemberInfo> GetMembers(Type type, XmlQualifiedName qname, bool declared_only)
	{
		List<DataMemberInfo> list = new List<DataMemberInfo>();
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		if (declared_only)
		{
			bindingFlags |= BindingFlags.DeclaredOnly;
		}
		PropertyInfo[] properties = type.GetProperties(bindingFlags);
		foreach (PropertyInfo propertyInfo in properties)
		{
			DataMemberAttribute dataMemberAttribute = GetDataMemberAttribute(propertyInfo);
			if (dataMemberAttribute != null)
			{
				KnownTypes.TryRegister(propertyInfo.PropertyType);
				SerializationMap serializationMap = KnownTypes.FindUserMap(propertyInfo.PropertyType);
				if (!propertyInfo.CanRead || (!propertyInfo.CanWrite && !(serializationMap is ICollectionTypeMap)))
				{
					throw new InvalidDataContractException($"DataMember property '{propertyInfo}' on type '{propertyInfo.DeclaringType}' must have both getter and setter.");
				}
				list.Add(CreateDataMemberInfo(dataMemberAttribute, propertyInfo, propertyInfo.PropertyType));
			}
		}
		FieldInfo[] fields = type.GetFields(bindingFlags);
		foreach (FieldInfo fieldInfo in fields)
		{
			DataMemberAttribute dataMemberAttribute2 = GetDataMemberAttribute(fieldInfo);
			if (dataMemberAttribute2 != null)
			{
				list.Add(CreateDataMemberInfo(dataMemberAttribute2, fieldInfo, fieldInfo.FieldType));
			}
		}
		return list;
	}

	public override List<DataMemberInfo> GetMembers()
	{
		return Members;
	}
}
