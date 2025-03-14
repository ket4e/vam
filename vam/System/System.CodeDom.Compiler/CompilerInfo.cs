using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Security.Permissions;

namespace System.CodeDom.Compiler;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public sealed class CompilerInfo
{
	internal string Languages;

	internal string Extensions;

	internal string TypeName;

	internal int WarningLevel;

	internal string CompilerOptions;

	internal Dictionary<string, string> ProviderOptions;

	private bool inited;

	private Type type;

	public Type CodeDomProviderType
	{
		get
		{
			if (type == null)
			{
				type = Type.GetType(TypeName, throwOnError: false);
				if (type == null)
				{
					throw new ConfigurationErrorsException("Unable to locate compiler type '" + TypeName + "'");
				}
			}
			return type;
		}
	}

	public bool IsCodeDomProviderTypeValid => type != null;

	internal CompilerInfo()
	{
	}

	internal void Init()
	{
		if (!inited)
		{
			inited = true;
			type = Type.GetType(TypeName);
			if (type != null && !typeof(CodeDomProvider).IsAssignableFrom(type))
			{
				type = null;
			}
		}
	}

	public CompilerParameters CreateDefaultCompilerParameters()
	{
		CompilerParameters compilerParameters = new CompilerParameters();
		if (CompilerOptions == null)
		{
			compilerParameters.CompilerOptions = string.Empty;
		}
		else
		{
			compilerParameters.CompilerOptions = CompilerOptions;
		}
		compilerParameters.WarningLevel = WarningLevel;
		return compilerParameters;
	}

	public CodeDomProvider CreateProvider()
	{
		Type codeDomProviderType = CodeDomProviderType;
		if (ProviderOptions != null && ProviderOptions.Count > 0)
		{
			ConstructorInfo constructor = codeDomProviderType.GetConstructor(new Type[1] { typeof(Dictionary<string, string>) });
			if (constructor != null)
			{
				return (CodeDomProvider)constructor.Invoke(new object[1] { ProviderOptions });
			}
		}
		return (CodeDomProvider)Activator.CreateInstance(codeDomProviderType);
	}

	public override bool Equals(object o)
	{
		if (!(o is CompilerInfo))
		{
			return false;
		}
		CompilerInfo compilerInfo = (CompilerInfo)o;
		return compilerInfo.TypeName == TypeName;
	}

	public override int GetHashCode()
	{
		return TypeName.GetHashCode();
	}

	public string[] GetExtensions()
	{
		return Extensions.Split(';');
	}

	public string[] GetLanguages()
	{
		return Languages.Split(';');
	}
}
