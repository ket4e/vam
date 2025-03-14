using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.CodeDom.Compiler;

[ComVisible(true)]
[ToolboxItem(false)]
public abstract class CodeDomProvider : Component
{
	public virtual string FileExtension => string.Empty;

	public virtual LanguageOptions LanguageOptions => LanguageOptions.None;

	private static System.CodeDom.Compiler.CodeDomConfigurationHandler Config => ConfigurationManager.GetSection("system.codedom") as System.CodeDom.Compiler.CodeDomConfigurationHandler;

	[Obsolete("ICodeCompiler is obsolete")]
	public abstract ICodeCompiler CreateCompiler();

	[Obsolete("ICodeGenerator is obsolete")]
	public abstract ICodeGenerator CreateGenerator();

	public virtual ICodeGenerator CreateGenerator(string fileName)
	{
		return CreateGenerator();
	}

	public virtual ICodeGenerator CreateGenerator(TextWriter output)
	{
		return CreateGenerator();
	}

	[Obsolete("ICodeParser is obsolete")]
	public virtual ICodeParser CreateParser()
	{
		return null;
	}

	public virtual TypeConverter GetConverter(Type type)
	{
		return TypeDescriptor.GetConverter(type);
	}

	public virtual CompilerResults CompileAssemblyFromDom(CompilerParameters options, params CodeCompileUnit[] compilationUnits)
	{
		ICodeCompiler codeCompiler = CreateCompiler();
		if (codeCompiler == null)
		{
			throw GetNotImplemented();
		}
		return codeCompiler.CompileAssemblyFromDomBatch(options, compilationUnits);
	}

	public virtual CompilerResults CompileAssemblyFromFile(CompilerParameters options, params string[] fileNames)
	{
		ICodeCompiler codeCompiler = CreateCompiler();
		if (codeCompiler == null)
		{
			throw GetNotImplemented();
		}
		return codeCompiler.CompileAssemblyFromFileBatch(options, fileNames);
	}

	public virtual CompilerResults CompileAssemblyFromSource(CompilerParameters options, params string[] fileNames)
	{
		ICodeCompiler codeCompiler = CreateCompiler();
		if (codeCompiler == null)
		{
			throw GetNotImplemented();
		}
		return codeCompiler.CompileAssemblyFromSourceBatch(options, fileNames);
	}

	public virtual string CreateEscapedIdentifier(string value)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		return codeGenerator.CreateEscapedIdentifier(value);
	}

	[ComVisible(false)]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public static CodeDomProvider CreateProvider(string language)
	{
		return GetCompilerInfo(language)?.CreateProvider();
	}

	public virtual string CreateValidIdentifier(string value)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		return codeGenerator.CreateValidIdentifier(value);
	}

	public virtual void GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, TextWriter writer, CodeGeneratorOptions options)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		codeGenerator.GenerateCodeFromCompileUnit(compileUnit, writer, options);
	}

	public virtual void GenerateCodeFromExpression(CodeExpression expression, TextWriter writer, CodeGeneratorOptions options)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		codeGenerator.GenerateCodeFromExpression(expression, writer, options);
	}

	public virtual void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
	{
		throw GetNotImplemented();
	}

	public virtual void GenerateCodeFromNamespace(CodeNamespace codeNamespace, TextWriter writer, CodeGeneratorOptions options)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		codeGenerator.GenerateCodeFromNamespace(codeNamespace, writer, options);
	}

	public virtual void GenerateCodeFromStatement(CodeStatement statement, TextWriter writer, CodeGeneratorOptions options)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		codeGenerator.GenerateCodeFromStatement(statement, writer, options);
	}

	public virtual void GenerateCodeFromType(CodeTypeDeclaration codeType, TextWriter writer, CodeGeneratorOptions options)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		codeGenerator.GenerateCodeFromType(codeType, writer, options);
	}

	[ComVisible(false)]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public static CompilerInfo[] GetAllCompilerInfo()
	{
		return (Config != null) ? Config.CompilerInfos : null;
	}

	[ComVisible(false)]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public static CompilerInfo GetCompilerInfo(string language)
	{
		if (language == null)
		{
			throw new ArgumentNullException("language");
		}
		if (Config == null)
		{
			return null;
		}
		System.CodeDom.Compiler.CompilerCollection compilers = Config.Compilers;
		return compilers[language];
	}

	[ComVisible(false)]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public static string GetLanguageFromExtension(string extension)
	{
		if (extension == null)
		{
			throw new ArgumentNullException("extension");
		}
		if (Config != null)
		{
			return Config.Compilers.GetLanguageFromExtension(extension);
		}
		return null;
	}

	public virtual string GetTypeOutput(CodeTypeReference type)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		return codeGenerator.GetTypeOutput(type);
	}

	[ComVisible(false)]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public static bool IsDefinedExtension(string extension)
	{
		if (extension == null)
		{
			throw new ArgumentNullException("extension");
		}
		if (Config != null)
		{
			return Config.Compilers.GetCompilerInfoForExtension(extension) != null;
		}
		return false;
	}

	[ComVisible(false)]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public static bool IsDefinedLanguage(string language)
	{
		if (language == null)
		{
			throw new ArgumentNullException("language");
		}
		if (Config != null)
		{
			return Config.Compilers.GetCompilerInfoForLanguage(language) != null;
		}
		return false;
	}

	public virtual bool IsValidIdentifier(string value)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		return codeGenerator.IsValidIdentifier(value);
	}

	public virtual CodeCompileUnit Parse(TextReader codeStream)
	{
		ICodeParser codeParser = CreateParser();
		if (codeParser == null)
		{
			throw GetNotImplemented();
		}
		return codeParser.Parse(codeStream);
	}

	public virtual bool Supports(GeneratorSupport supports)
	{
		ICodeGenerator codeGenerator = CreateGenerator();
		if (codeGenerator == null)
		{
			throw GetNotImplemented();
		}
		return codeGenerator.Supports(supports);
	}

	private Exception GetNotImplemented()
	{
		return new NotImplementedException();
	}
}
