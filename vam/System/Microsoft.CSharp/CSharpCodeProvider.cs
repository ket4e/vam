using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using Mono.CSharp;

namespace Microsoft.CSharp;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class CSharpCodeProvider : CodeDomProvider
{
	private IDictionary<string, string> providerOptions;

	public override string FileExtension => "cs";

	public CSharpCodeProvider()
	{
	}

	public CSharpCodeProvider(IDictionary<string, string> providerOptions)
	{
		this.providerOptions = providerOptions;
	}

	[Obsolete("Use CodeDomProvider class")]
	public override ICodeCompiler CreateCompiler()
	{
		if (providerOptions != null && providerOptions.Count > 0)
		{
			return new Mono.CSharp.CSharpCodeCompiler(providerOptions);
		}
		return new Mono.CSharp.CSharpCodeCompiler();
	}

	[Obsolete("Use CodeDomProvider class")]
	public override ICodeGenerator CreateGenerator()
	{
		if (providerOptions != null && providerOptions.Count > 0)
		{
			return new Mono.CSharp.CSharpCodeGenerator(providerOptions);
		}
		return new Mono.CSharp.CSharpCodeGenerator();
	}

	[System.MonoTODO]
	public override TypeConverter GetConverter(Type Type)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public override void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
	{
		throw new NotImplementedException();
	}
}
