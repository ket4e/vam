using System.CodeDom.Compiler;
using System.Globalization;
using System.Xml;
using Microsoft.CSharp;

namespace Mono.Xml.Xsl;

internal class CSharpCompilerInfo : ScriptCompilerInfo
{
	public override CodeDomProvider CodeDomProvider => new CSharpCodeProvider();

	public override string Extension => ".cs";

	public override string SourceTemplate => "// This file is automatically created by Mono managed XSLT engine.\n// Created time: {0}\nusing System;\nusing System.Collections;\nusing System.Text;\nusing System.Text.RegularExpressions;\nusing System.Xml;\nusing System.Xml.XPath;\nusing System.Xml.Xsl;\nusing Microsoft.VisualBasic;\n\nnamespace GeneratedAssembly\n{\npublic class Script{1}\n{\n\t{2}\n}\n}";

	public CSharpCompilerInfo()
	{
		CompilerCommand = "mcs";
		DefaultCompilerOptions = "/t:library /r:System.dll /r:System.Xml.dll /r:Microsoft.VisualBasic.dll";
	}

	public override string FormatSource(IXmlLineInfo li, string file, string source)
	{
		if (li == null)
		{
			return source;
		}
		return string.Format(CultureInfo.InvariantCulture, "#line {0} \"{1}\"\n{2}", li.LineNumber, file, source);
	}
}
