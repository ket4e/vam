using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace System.ComponentModel;

public class ComponentResourceManager : ResourceManager
{
	public ComponentResourceManager()
	{
	}

	public ComponentResourceManager(Type t)
		: base(t)
	{
	}

	public void ApplyResources(object value, string objectName)
	{
		ApplyResources(value, objectName, null);
	}

	public virtual void ApplyResources(object value, string objectName, CultureInfo culture)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (objectName == null)
		{
			throw new ArgumentNullException("objectName");
		}
		if (culture == null)
		{
			culture = CultureInfo.CurrentUICulture;
		}
		Hashtable hashtable = ((!IgnoreCase) ? new Hashtable() : CollectionsUtil.CreateCaseInsensitiveHashtable());
		BuildResources(culture, hashtable);
		string text = objectName + ".";
		CompareOptions options = (IgnoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		Type type = value.GetType();
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
		if (IgnoreCase)
		{
			bindingFlags |= BindingFlags.IgnoreCase;
		}
		foreach (DictionaryEntry item in hashtable)
		{
			string text2 = (string)item.Key;
			if (!culture.CompareInfo.IsPrefix(text2, text, options))
			{
				continue;
			}
			string name = text2.Substring(text.Length);
			PropertyInfo property = type.GetProperty(name, bindingFlags);
			if (property != null && property.CanWrite)
			{
				object value2 = item.Value;
				if (value2 == null || property.PropertyType.IsInstanceOfType(value2))
				{
					property.SetValue(value, value2, null);
				}
			}
		}
	}

	private void BuildResources(CultureInfo culture, Hashtable resources)
	{
		if (culture != culture.Parent)
		{
			BuildResources(culture.Parent, resources);
		}
		ResourceSet resourceSet = GetResourceSet(culture, createIfNotExists: true, tryParents: false);
		if (resourceSet == null)
		{
			return;
		}
		foreach (DictionaryEntry item in resourceSet)
		{
			resources[(string)item.Key] = item.Value;
		}
	}
}
