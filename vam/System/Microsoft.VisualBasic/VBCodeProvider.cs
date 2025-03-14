using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;

namespace Microsoft.VisualBasic;

[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class VBCodeProvider : CodeDomProvider
{
	public override string FileExtension => "vb";

	public override LanguageOptions LanguageOptions => LanguageOptions.CaseInsensitive;

	public VBCodeProvider()
	{
	}

	public VBCodeProvider(IDictionary<string, string> providerOptions)
	{
	}

	[Obsolete("Use CodeDomProvider class")]
	public override ICodeCompiler CreateCompiler()
	{
		return new Microsoft.VisualBasic.VBCodeCompiler();
	}

	[Obsolete("Use CodeDomProvider class")]
	public override ICodeGenerator CreateGenerator()
	{
		return new Microsoft.VisualBasic.VBCodeGenerator();
	}

	public override TypeConverter GetConverter(Type type)
	{
		return TypeDescriptor.GetConverter(type);
	}

	[System.MonoTODO]
	public override void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
	{
		throw new NotImplementedException();
	}
}
