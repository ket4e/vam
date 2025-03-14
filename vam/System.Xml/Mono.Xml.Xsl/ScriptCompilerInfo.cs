using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl;

internal abstract class ScriptCompilerInfo
{
	private string compilerCommand;

	private string defaultCompilerOptions;

	public virtual string CompilerCommand
	{
		get
		{
			return compilerCommand;
		}
		set
		{
			compilerCommand = value;
		}
	}

	public virtual string DefaultCompilerOptions
	{
		get
		{
			return defaultCompilerOptions;
		}
		set
		{
			defaultCompilerOptions = value;
		}
	}

	public abstract CodeDomProvider CodeDomProvider { get; }

	public abstract string Extension { get; }

	public abstract string SourceTemplate { get; }

	public abstract string FormatSource(IXmlLineInfo li, string file, string code);

	public virtual string GetCompilerArguments(string targetFileName)
	{
		return DefaultCompilerOptions + " " + targetFileName;
	}

	public virtual Type GetScriptClass(string code, string classSuffix, XPathNavigator scriptNode, Evidence evidence)
	{
		SecurityManager.ResolvePolicy(evidence)?.Demand();
		string text = "Script" + classSuffix;
		string text2 = "GeneratedAssembly." + text;
		try
		{
			Type type = Type.GetType(text2);
			if (type != null)
			{
				return type;
			}
		}
		catch
		{
		}
		try
		{
			Type type2 = Assembly.LoadFrom(text + ".dll").GetType(text2);
			if (type2 != null)
			{
				return type2;
			}
		}
		catch
		{
		}
		ICodeCompiler codeCompiler = CodeDomProvider.CreateCompiler();
		CompilerParameters compilerParameters = new CompilerParameters();
		compilerParameters.CompilerOptions = DefaultCompilerOptions;
		string text3 = string.Empty;
		try
		{
			if (scriptNode.BaseURI != string.Empty)
			{
				text3 = new Uri(scriptNode.BaseURI).LocalPath;
			}
		}
		catch (FormatException)
		{
		}
		if (text3 == string.Empty)
		{
			text3 = "__baseURI_not_supplied__";
		}
		IXmlLineInfo li = scriptNode as IXmlLineInfo;
		string code2 = SourceTemplate.Replace("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture)).Replace("{1}", classSuffix).Replace("{2}", code);
		code2 = FormatSource(li, text3, code2);
		CompilerResults compilerResults = codeCompiler.CompileAssemblyFromSource(compilerParameters, code2);
		foreach (CompilerError error in compilerResults.Errors)
		{
			if (!error.IsWarning)
			{
				throw new XsltException("Stylesheet script compile error: \n" + FormatErrorMessage(compilerResults), null, scriptNode);
			}
		}
		if (compilerResults.CompiledAssembly == null)
		{
			throw new XsltCompileException("Cannot compile stylesheet script", null, scriptNode);
		}
		return compilerResults.CompiledAssembly.GetType(text2);
	}

	private string FormatErrorMessage(CompilerResults res)
	{
		string text = string.Empty;
		foreach (CompilerError error in res.Errors)
		{
			object[] array = new object[7]
			{
				"\n",
				error.FileName,
				(error.Line <= 0) ? string.Empty : (" line " + error.Line),
				(!error.IsWarning) ? " ERROR: " : " WARNING: ",
				error.ErrorNumber,
				": ",
				error.ErrorText
			};
			text = string.Concat(text, string.Concat(array));
		}
		return text;
	}
}
