using System.Collections;
using System.Text;

namespace System.Xml.Serialization;

public class XmlElementAttributes : CollectionBase
{
	public XmlElementAttribute this[int index]
	{
		get
		{
			return (XmlElementAttribute)base.List[index];
		}
		set
		{
			base.List[index] = value;
		}
	}

	public int Add(XmlElementAttribute attribute)
	{
		return base.List.Add(attribute);
	}

	public bool Contains(XmlElementAttribute attribute)
	{
		return base.List.Contains(attribute);
	}

	public int IndexOf(XmlElementAttribute attribute)
	{
		return base.List.IndexOf(attribute);
	}

	public void Insert(int index, XmlElementAttribute attribute)
	{
		base.List.Insert(index, attribute);
	}

	public void Remove(XmlElementAttribute attribute)
	{
		base.List.Remove(attribute);
	}

	public void CopyTo(XmlElementAttribute[] array, int index)
	{
		base.List.CopyTo(array, index);
	}

	internal void AddKeyHash(StringBuilder sb)
	{
		if (Count != 0)
		{
			sb.Append("XEAS ");
			for (int i = 0; i < Count; i++)
			{
				this[i].AddKeyHash(sb);
			}
			sb.Append('|');
		}
	}
}
