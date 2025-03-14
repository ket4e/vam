using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms;

public static class ListBindingHelper
{
	public static object GetList(object list)
	{
		if (list is IListSource)
		{
			return ((IListSource)list).GetList();
		}
		return list;
	}

	public static object GetList(object dataSource, string dataMember)
	{
		dataSource = GetList(dataSource);
		if (dataSource == null || dataMember == null || dataMember.Length == 0)
		{
			return dataSource;
		}
		PropertyDescriptor propertyDescriptor = GetListItemProperties(dataSource).Find(dataMember, ignoreCase: true);
		if (propertyDescriptor == null)
		{
			throw new ArgumentException("dataMember");
		}
		object obj = null;
		if (dataSource is ICurrencyManagerProvider currencyManagerProvider && currencyManagerProvider.CurrencyManager != null)
		{
			CurrencyManager currencyManager = currencyManagerProvider.CurrencyManager;
			if (currencyManager != null && currencyManager.Count > 0 && currencyManager.Current != null)
			{
				obj = currencyManager.Current;
			}
		}
		if (obj == null)
		{
			if (dataSource is IEnumerable)
			{
				if (dataSource is IList)
				{
					IList list = (IList)dataSource;
					obj = ((list.Count <= 0) ? null : list[0]);
				}
				else
				{
					IEnumerator enumerator = ((IEnumerable)dataSource).GetEnumerator();
					if (enumerator != null && enumerator.MoveNext())
					{
						obj = enumerator.Current;
					}
				}
			}
			else
			{
				obj = dataSource;
			}
		}
		if (obj != null)
		{
			return propertyDescriptor.GetValue(obj);
		}
		return null;
	}

	public static Type GetListItemType(object list)
	{
		return GetListItemType(list, string.Empty);
	}

	public static Type GetListItemType(object dataSource, string dataMember)
	{
		if (dataSource == null)
		{
			return null;
		}
		if (dataMember != null && dataMember.Length > 0)
		{
			PropertyDescriptor property = GetProperty(dataSource, dataMember);
			if (property == null)
			{
				return typeof(object);
			}
			return property.PropertyType;
		}
		if (dataSource is Array)
		{
			return dataSource.GetType().GetElementType();
		}
		if (dataSource is IEnumerable)
		{
			IEnumerator enumerator = ((IEnumerable)dataSource).GetEnumerator();
			if (enumerator.MoveNext() && enumerator.Current != null)
			{
				return enumerator.Current.GetType();
			}
			if (dataSource is IList || dataSource.GetType() == typeof(IList<>))
			{
				PropertyInfo propertyByReflection = GetPropertyByReflection(dataSource.GetType(), "Item");
				if (propertyByReflection != null)
				{
					return propertyByReflection.PropertyType;
				}
			}
			return typeof(object);
		}
		return dataSource.GetType();
	}

	public static PropertyDescriptorCollection GetListItemProperties(object list)
	{
		return GetListItemProperties(list, null);
	}

	public static PropertyDescriptorCollection GetListItemProperties(object list, PropertyDescriptor[] listAccessors)
	{
		list = GetList(list);
		if (list == null)
		{
			return new PropertyDescriptorCollection(null);
		}
		if (list is ITypedList)
		{
			return ((ITypedList)list).GetItemProperties(listAccessors);
		}
		if (listAccessors == null || listAccessors.Length == 0)
		{
			Type listItemType = GetListItemType(list);
			return TypeDescriptor.GetProperties(listItemType, new Attribute[1]
			{
				new BrowsableAttribute(browsable: true)
			});
		}
		Type propertyType = listAccessors[0].PropertyType;
		if (typeof(IList).IsAssignableFrom(propertyType) || typeof(IList<>).IsAssignableFrom(propertyType))
		{
			PropertyInfo propertyByReflection = GetPropertyByReflection(propertyType, "Item");
			return TypeDescriptor.GetProperties(propertyByReflection.PropertyType);
		}
		return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
	}

	public static PropertyDescriptorCollection GetListItemProperties(object dataSource, string dataMember, PropertyDescriptor[] listAccessors)
	{
		throw new NotImplementedException();
	}

	public static string GetListName(object list, PropertyDescriptor[] listAccessors)
	{
		if (list == null)
		{
			return string.Empty;
		}
		Type listItemType = GetListItemType(list);
		return listItemType.Name;
	}

	private static PropertyDescriptor GetProperty(object obj, string property_name)
	{
		return TypeDescriptor.GetProperties(obj, new Attribute[1]
		{
			new BrowsableAttribute(browsable: true)
		})[property_name];
	}

	private static PropertyInfo GetPropertyByReflection(Type type, string property_name)
	{
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.Name == property_name)
			{
				return propertyInfo;
			}
		}
		return null;
	}
}
