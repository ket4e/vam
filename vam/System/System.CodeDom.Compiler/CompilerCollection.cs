using System.Collections.Generic;
using System.Configuration;

namespace System.CodeDom.Compiler;

[ConfigurationCollection(typeof(System.CodeDom.Compiler.Compiler), AddItemName = "compiler", CollectionType = ConfigurationElementCollectionType.BasicMap)]
internal sealed class CompilerCollection : ConfigurationElementCollection
{
	private static ConfigurationPropertyCollection properties;

	private static List<CompilerInfo> compiler_infos;

	private static Dictionary<string, CompilerInfo> compiler_languages;

	private static Dictionary<string, CompilerInfo> compiler_extensions;

	protected override bool ThrowOnDuplicate => false;

	public string[] AllKeys
	{
		get
		{
			string[] array = new string[compiler_infos.Count];
			for (int i = 0; i < Count; i++)
			{
				array[i] = compiler_infos[i].Languages;
			}
			return array;
		}
	}

	public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

	protected override string ElementName => "compiler";

	protected override ConfigurationPropertyCollection Properties => properties;

	public System.CodeDom.Compiler.Compiler this[int index] => (System.CodeDom.Compiler.Compiler)BaseGet(index);

	public new CompilerInfo this[string language] => GetCompilerInfoForLanguage(language);

	public CompilerInfo[] CompilerInfos => compiler_infos.ToArray();

	static CompilerCollection()
	{
		properties = new ConfigurationPropertyCollection();
		compiler_infos = new List<CompilerInfo>();
		compiler_languages = new Dictionary<string, CompilerInfo>(16, StringComparer.OrdinalIgnoreCase);
		compiler_extensions = new Dictionary<string, CompilerInfo>(6, StringComparer.OrdinalIgnoreCase);
		CompilerInfo compilerInfo = new CompilerInfo();
		compilerInfo.Languages = "c#;cs;csharp";
		compilerInfo.Extensions = ".cs";
		compilerInfo.TypeName = "Microsoft.CSharp.CSharpCodeProvider, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
		compilerInfo.ProviderOptions = new Dictionary<string, string>(1);
		compilerInfo.ProviderOptions["CompilerVersion"] = "2.0";
		AddCompilerInfo(compilerInfo);
		compilerInfo = new CompilerInfo();
		compilerInfo.Languages = "vb;vbs;visualbasic;vbscript";
		compilerInfo.Extensions = ".vb";
		compilerInfo.TypeName = "Microsoft.VisualBasic.VBCodeProvider, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
		compilerInfo.ProviderOptions = new Dictionary<string, string>(1);
		compilerInfo.ProviderOptions["CompilerVersion"] = "2.0";
		AddCompilerInfo(compilerInfo);
		compilerInfo = new CompilerInfo();
		compilerInfo.Languages = "js;jscript;javascript";
		compilerInfo.Extensions = ".js";
		compilerInfo.TypeName = "Microsoft.JScript.JScriptCodeProvider, Microsoft.JScript, Version=8.0.1100.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		compilerInfo.ProviderOptions = new Dictionary<string, string>(1);
		compilerInfo.ProviderOptions["CompilerVersion"] = "2.0";
		AddCompilerInfo(compilerInfo);
		compilerInfo = new CompilerInfo();
		compilerInfo.Languages = "vj#;vjs;vjsharp";
		compilerInfo.Extensions = ".jsl;.java";
		compilerInfo.TypeName = "Microsoft.VJSharp.VJSharpCodeProvider, VJSharpCodeProvider, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		compilerInfo.ProviderOptions = new Dictionary<string, string>(1);
		compilerInfo.ProviderOptions["CompilerVersion"] = "2.0";
		AddCompilerInfo(compilerInfo);
		compilerInfo = new CompilerInfo();
		compilerInfo.Languages = "c++;mc;cpp";
		compilerInfo.Extensions = ".h";
		compilerInfo.TypeName = "Microsoft.VisualC.CppCodeProvider, CppCodeProvider, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		compilerInfo.ProviderOptions = new Dictionary<string, string>(1);
		compilerInfo.ProviderOptions["CompilerVersion"] = "2.0";
		AddCompilerInfo(compilerInfo);
	}

	private static void AddCompilerInfo(CompilerInfo ci)
	{
		ci.Init();
		compiler_infos.Add(ci);
		string[] languages = ci.GetLanguages();
		if (languages != null)
		{
			string[] array = languages;
			foreach (string key in array)
			{
				compiler_languages[key] = ci;
			}
		}
		string[] extensions = ci.GetExtensions();
		if (extensions != null)
		{
			string[] array2 = extensions;
			foreach (string key2 in array2)
			{
				compiler_extensions[key2] = ci;
			}
		}
	}

	private static void AddCompilerInfo(System.CodeDom.Compiler.Compiler compiler)
	{
		CompilerInfo compilerInfo = new CompilerInfo();
		compilerInfo.Languages = compiler.Language;
		compilerInfo.Extensions = compiler.Extension;
		compilerInfo.TypeName = compiler.Type;
		compilerInfo.ProviderOptions = compiler.ProviderOptionsDictionary;
		compilerInfo.CompilerOptions = compiler.CompilerOptions;
		compilerInfo.WarningLevel = compiler.WarningLevel;
		AddCompilerInfo(compilerInfo);
	}

	protected override void BaseAdd(ConfigurationElement element)
	{
		if (element is System.CodeDom.Compiler.Compiler compiler)
		{
			AddCompilerInfo(compiler);
		}
		base.BaseAdd(element);
	}

	protected override ConfigurationElement CreateNewElement()
	{
		return new System.CodeDom.Compiler.Compiler();
	}

	public CompilerInfo GetCompilerInfoForLanguage(string language)
	{
		if (compiler_languages.Count == 0)
		{
			return null;
		}
		if (compiler_languages.TryGetValue(language, out var value))
		{
			return value;
		}
		return null;
	}

	public CompilerInfo GetCompilerInfoForExtension(string extension)
	{
		if (compiler_extensions.Count == 0)
		{
			return null;
		}
		if (compiler_extensions.TryGetValue(extension, out var value))
		{
			return value;
		}
		return null;
	}

	public string GetLanguageFromExtension(string extension)
	{
		CompilerInfo compilerInfoForExtension = GetCompilerInfoForExtension(extension);
		if (compilerInfoForExtension == null)
		{
			return null;
		}
		string[] languages = compilerInfoForExtension.GetLanguages();
		if (languages != null && languages.Length > 0)
		{
			return languages[0];
		}
		return null;
	}

	public System.CodeDom.Compiler.Compiler Get(int index)
	{
		return (System.CodeDom.Compiler.Compiler)BaseGet(index);
	}

	public System.CodeDom.Compiler.Compiler Get(string language)
	{
		return (System.CodeDom.Compiler.Compiler)BaseGet(language);
	}

	protected override object GetElementKey(ConfigurationElement element)
	{
		return ((System.CodeDom.Compiler.Compiler)element).Language;
	}

	public string GetKey(int index)
	{
		return (string)BaseGetKey(index);
	}
}
