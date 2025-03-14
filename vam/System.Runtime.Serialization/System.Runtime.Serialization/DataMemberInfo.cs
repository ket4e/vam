using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Runtime.Serialization;

internal class DataMemberInfo
{
	public class DataMemberInfoComparer : IComparer<DataMemberInfo>, IComparer
	{
		public static readonly DataMemberInfoComparer Instance = new DataMemberInfoComparer();

		private DataMemberInfoComparer()
		{
		}

		public int Compare(object o1, object o2)
		{
			return Compare((DataMemberInfo)o1, (DataMemberInfo)o2);
		}

		public int Compare(DataMemberInfo d1, DataMemberInfo d2)
		{
			if (d1.Order == d2.Order)
			{
				return string.CompareOrdinal(d1.XmlName, d2.XmlName);
			}
			return d1.Order - d2.Order;
		}
	}

	public readonly int Order;

	public readonly bool IsRequired;

	public readonly string XmlName;

	public readonly MemberInfo Member;

	public readonly string XmlNamespace;

	public readonly string XmlRootNamespace;

	public readonly Type MemberType;

	public DataMemberInfo(MemberInfo member, DataMemberAttribute dma, string rootNamespce, string ns)
	{
		if (dma == null)
		{
			throw new ArgumentNullException("dma");
		}
		Order = dma.Order;
		Member = member;
		IsRequired = dma.IsRequired;
		XmlName = ((dma.Name == null) ? member.Name : dma.Name);
		XmlNamespace = ns;
		XmlRootNamespace = rootNamespce;
		if (Member is FieldInfo)
		{
			MemberType = ((FieldInfo)Member).FieldType;
		}
		else
		{
			MemberType = ((PropertyInfo)Member).PropertyType;
		}
	}
}
