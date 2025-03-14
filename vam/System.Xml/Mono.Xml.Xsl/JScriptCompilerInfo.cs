using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Xml;

namespace Mono.Xml.Xsl;

internal class JScriptCompilerInfo : ScriptCompilerInfo
{
	private static Type providerType;

	public override CodeDomProvider CodeDomProvider
	{
		get
		{
			if (providerType == null)
			{
				Assembly assembly = Assembly.LoadWithPartialName("Microsoft.JScript", null);
				if (assembly != null)
				{
					providerType = assembly.GetType("Microsoft.JScript.JScriptCodeProvider");
				}
			}
			return (CodeDomProvider)Activator.CreateInstance(providerType);
		}
	}

	public override string Extension => ".js";

	public override string SourceTemplate => "// This file is automatically created by Mono managed XSLT engine.\n// Created time: {0}\nimport System;\nimport System.Collections;\nimport System.Text;\nimport System.Text.RegularExpressions;\nimport System.Xml;\nimport System.Xml.XPath;\nimport System.Xml.Xsl;\nimport Microsoft.VisualBasic;\n\npackage GeneratedAssembly\n{\nclass Script{1} {\n\t{2}\n}\n}\n";

	public JScriptCompilerInfo()
	{
		CompilerCommand = "mjs";
		DefaultCompilerOptions = "/t:library /r:Microsoft.VisualBasic.dll";
	}

	public override string FormatSource(IXmlLineInfo li, string file, string source)
	{
		return source;
	}
}
