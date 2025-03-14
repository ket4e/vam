using System.Collections.Generic;
using System.Reflection;

namespace System.Runtime.Serialization;

internal class DefaultTypeMap : SerializationMap
{
	public DefaultTypeMap(Type type, KnownTypeCollection knownTypes)
		: base(type, KnownTypeCollection.GetContractQName(type, null, null), knownTypes)
	{
		Members.AddRange(GetDefaultMembers());
	}

	private List<DataMemberInfo> GetDefaultMembers()
	{
		List<DataMemberInfo> list = new List<DataMemberInfo>();
		MemberInfo[] members = RuntimeType.GetMembers();
		foreach (MemberInfo memberInfo in members)
		{
			Type type = null;
			type = ((memberInfo is FieldInfo fieldInfo) ? fieldInfo.FieldType : null);
			if (memberInfo is PropertyInfo propertyInfo && propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0)
			{
				type = propertyInfo.PropertyType;
			}
			if (type != null && memberInfo.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), inherit: false).Length == 0)
			{
				list.Add(new DataMemberInfo(memberInfo, new DataMemberAttribute(), null, null));
			}
		}
		list.Sort(DataMemberInfo.DataMemberInfoComparer.Instance);
		return list;
	}
}
