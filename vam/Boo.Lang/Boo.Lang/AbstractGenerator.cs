using System;
using System.Collections;

namespace Boo.Lang;

public abstract class AbstractGenerator : IEnumerable
{
	public abstract IEnumerator GetEnumerator();

	public override string ToString()
	{
		EnumeratorItemTypeAttribute enumeratorItemTypeAttribute = (EnumeratorItemTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(EnumeratorItemTypeAttribute));
		return $"generator({enumeratorItemTypeAttribute.ItemType})";
	}
}
