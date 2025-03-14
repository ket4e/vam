using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization;

internal class SharedTypeMap : SerializationMap
{
	public SharedTypeMap(Type type, XmlQualifiedName qname, KnownTypeCollection knownTypes)
		: base(type, qname, knownTypes)
	{
		Members = GetMembers(type, base.XmlName, declared_only: false);
	}

	private List<DataMemberInfo> GetMembers(Type type, XmlQualifiedName qname, bool declared_only)
	{
		List<DataMemberInfo> list = new List<DataMemberInfo>();
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		if (declared_only)
		{
			bindingFlags |= BindingFlags.DeclaredOnly;
		}
		FieldInfo[] fields = type.GetFields(bindingFlags);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.GetCustomAttributes(typeof(NonSerializedAttribute), inherit: false).Length <= 0)
			{
				if (fieldInfo.IsInitOnly)
				{
					throw new InvalidDataContractException($"DataMember field {fieldInfo} must not be read-only.");
				}
				DataMemberAttribute dma = new DataMemberAttribute();
				list.Add(CreateDataMemberInfo(dma, fieldInfo, fieldInfo.FieldType));
			}
		}
		list.Sort(DataMemberInfo.DataMemberInfoComparer.Instance);
		return list;
	}

	public override List<DataMemberInfo> GetMembers()
	{
		return Members;
	}
}
