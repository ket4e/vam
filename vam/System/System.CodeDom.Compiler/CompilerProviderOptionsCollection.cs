using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace System.CodeDom.Compiler;

[ConfigurationCollection(typeof(System.CodeDom.Compiler.CompilerProviderOption), CollectionType = ConfigurationElementCollectionType.BasicMap, AddItemName = "providerOption")]
internal sealed class CompilerProviderOptionsCollection : ConfigurationElementCollection
{
	private static ConfigurationPropertyCollection properties;

	public string[] AllKeys
	{
		get
		{
			int count = Count;
			string[] array = new string[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = this[i].Name;
			}
			return array;
		}
	}

	protected override string ElementName => "providerOption";

	protected override ConfigurationPropertyCollection Properties => properties;

	public Dictionary<string, string> ProviderOptions
	{
		get
		{
			int count = Count;
			if (count == 0)
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(count);
			for (int i = 0; i < count; i++)
			{
				System.CodeDom.Compiler.CompilerProviderOption compilerProviderOption = this[i];
				dictionary.Add(compilerProviderOption.Name, compilerProviderOption.Value);
			}
			return dictionary;
		}
	}

	public System.CodeDom.Compiler.CompilerProviderOption this[int index] => (System.CodeDom.Compiler.CompilerProviderOption)BaseGet(index);

	public new System.CodeDom.Compiler.CompilerProviderOption this[string name]
	{
		get
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					System.CodeDom.Compiler.CompilerProviderOption compilerProviderOption = (System.CodeDom.Compiler.CompilerProviderOption)enumerator.Current;
					if (compilerProviderOption.Name == name)
					{
						return compilerProviderOption;
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return null;
		}
	}

	static CompilerProviderOptionsCollection()
	{
		properties = new ConfigurationPropertyCollection();
	}

	protected override ConfigurationElement CreateNewElement()
	{
		return new System.CodeDom.Compiler.CompilerProviderOption();
	}

	public System.CodeDom.Compiler.CompilerProviderOption Get(int index)
	{
		return (System.CodeDom.Compiler.CompilerProviderOption)BaseGet(index);
	}

	public System.CodeDom.Compiler.CompilerProviderOption Get(string name)
	{
		return (System.CodeDom.Compiler.CompilerProviderOption)BaseGet(name);
	}

	protected override object GetElementKey(ConfigurationElement element)
	{
		return ((System.CodeDom.Compiler.CompilerProviderOption)element).Name;
	}

	public string GetKey(int index)
	{
		return (string)BaseGetKey(index);
	}
}
