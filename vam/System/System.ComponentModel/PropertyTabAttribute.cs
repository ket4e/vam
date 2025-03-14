using System.Reflection;

namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public class PropertyTabAttribute : Attribute
{
	private Type[] tabs;

	private PropertyTabScope[] scopes;

	public Type[] TabClasses => tabs;

	public PropertyTabScope[] TabScopes => scopes;

	protected string[] TabClassNames
	{
		get
		{
			string[] array = new string[tabs.Length];
			for (int i = 0; i < tabs.Length; i++)
			{
				array[i] = tabs[i].Name;
			}
			return array;
		}
	}

	public PropertyTabAttribute()
	{
		tabs = Type.EmptyTypes;
		scopes = new PropertyTabScope[0];
	}

	public PropertyTabAttribute(string tabClassName)
		: this(tabClassName, PropertyTabScope.Component)
	{
	}

	public PropertyTabAttribute(Type tabClass)
		: this(tabClass, PropertyTabScope.Component)
	{
	}

	public PropertyTabAttribute(string tabClassName, PropertyTabScope tabScope)
	{
		if (tabClassName == null)
		{
			throw new ArgumentNullException("tabClassName");
		}
		InitializeArrays(new string[1] { tabClassName }, new PropertyTabScope[1] { tabScope });
	}

	public PropertyTabAttribute(Type tabClass, PropertyTabScope tabScope)
	{
		if (tabClass == null)
		{
			throw new ArgumentNullException("tabClass");
		}
		InitializeArrays(new Type[1] { tabClass }, new PropertyTabScope[1] { tabScope });
	}

	public override bool Equals(object other)
	{
		if (other is PropertyTabAttribute)
		{
			return Equals((PropertyTabAttribute)other);
		}
		return false;
	}

	public bool Equals(PropertyTabAttribute other)
	{
		if (other != this)
		{
			if (other.TabClasses.Length != tabs.Length)
			{
				return false;
			}
			for (int i = 0; i < tabs.Length; i++)
			{
				if (tabs[i] != other.TabClasses[i])
				{
					return false;
				}
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	protected void InitializeArrays(string[] tabClassNames, PropertyTabScope[] tabScopes)
	{
		if (tabScopes == null)
		{
			throw new ArgumentNullException("tabScopes");
		}
		if (tabClassNames == null)
		{
			throw new ArgumentNullException("tabClassNames");
		}
		scopes = tabScopes;
		tabs = new Type[tabClassNames.Length];
		for (int i = 0; i < tabClassNames.Length; i++)
		{
			tabs[i] = GetTypeFromName(tabClassNames[i]);
		}
	}

	protected void InitializeArrays(Type[] tabClasses, PropertyTabScope[] tabScopes)
	{
		if (tabScopes == null)
		{
			throw new ArgumentNullException("tabScopes");
		}
		if (tabClasses == null)
		{
			throw new ArgumentNullException("tabClasses");
		}
		if (tabClasses.Length != tabScopes.Length)
		{
			throw new ArgumentException("tabClasses.Length != tabScopes.Length");
		}
		tabs = tabClasses;
		scopes = tabScopes;
	}

	private Type GetTypeFromName(string typeName)
	{
		if (typeName == null)
		{
			throw new ArgumentNullException("typeName");
		}
		int num = typeName.IndexOf(",");
		if (num != -1)
		{
			string name = typeName.Substring(0, num);
			string assemblyString = typeName.Substring(num + 1);
			Assembly assembly = Assembly.Load(assemblyString);
			if (assembly != null)
			{
				return assembly.GetType(name, throwOnError: true);
			}
		}
		return Type.GetType(typeName, throwOnError: true);
	}
}
