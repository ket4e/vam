using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace System.CodeDom.Compiler;

public abstract class CodeCompiler : CodeGenerator, ICodeCompiler
{
	protected abstract string CompilerName { get; }

	protected abstract string FileExtension { get; }

	CompilerResults ICodeCompiler.CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit e)
	{
		return FromDom(options, e);
	}

	CompilerResults ICodeCompiler.CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
	{
		return FromDomBatch(options, ea);
	}

	CompilerResults ICodeCompiler.CompileAssemblyFromFile(CompilerParameters options, string fileName)
	{
		return FromFile(options, fileName);
	}

	CompilerResults ICodeCompiler.CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
	{
		return FromFileBatch(options, fileNames);
	}

	CompilerResults ICodeCompiler.CompileAssemblyFromSource(CompilerParameters options, string source)
	{
		return FromSource(options, source);
	}

	CompilerResults ICodeCompiler.CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
	{
		return FromSourceBatch(options, sources);
	}

	protected abstract string CmdArgsFromParameters(CompilerParameters options);

	protected virtual CompilerResults FromDom(CompilerParameters options, CodeCompileUnit e)
	{
		return FromDomBatch(options, new CodeCompileUnit[1] { e });
	}

	protected virtual CompilerResults FromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
	{
		string[] array = new string[ea.Length];
		int num = 0;
		if (options == null)
		{
			options = new CompilerParameters();
		}
		StringCollection referencedAssemblies = options.ReferencedAssemblies;
		foreach (CodeCompileUnit codeCompileUnit in ea)
		{
			array[num] = Path.ChangeExtension(Path.GetTempFileName(), FileExtension);
			FileStream fileStream = new FileStream(array[num], FileMode.OpenOrCreate);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			if (codeCompileUnit.ReferencedAssemblies != null)
			{
				StringEnumerator enumerator = codeCompileUnit.ReferencedAssemblies.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						if (!referencedAssemblies.Contains(current))
						{
							referencedAssemblies.Add(current);
						}
					}
				}
				finally
				{
					if (enumerator is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
			}
			((ICodeGenerator)this).GenerateCodeFromCompileUnit(codeCompileUnit, (TextWriter)streamWriter, new CodeGeneratorOptions());
			streamWriter.Close();
			fileStream.Close();
			num++;
		}
		return Compile(options, array, keepFiles: false);
	}

	protected virtual CompilerResults FromFile(CompilerParameters options, string fileName)
	{
		return FromFileBatch(options, new string[1] { fileName });
	}

	protected virtual CompilerResults FromFileBatch(CompilerParameters options, string[] fileNames)
	{
		return Compile(options, fileNames, keepFiles: true);
	}

	protected virtual CompilerResults FromSource(CompilerParameters options, string source)
	{
		return FromSourceBatch(options, new string[1] { source });
	}

	protected virtual CompilerResults FromSourceBatch(CompilerParameters options, string[] sources)
	{
		string[] array = new string[sources.Length];
		int num = 0;
		foreach (string value in sources)
		{
			array[num] = Path.ChangeExtension(Path.GetTempFileName(), FileExtension);
			FileStream fileStream = new FileStream(array[num], FileMode.OpenOrCreate);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(value);
			streamWriter.Close();
			fileStream.Close();
			num++;
		}
		return Compile(options, array, keepFiles: false);
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	private CompilerResults Compile(CompilerParameters options, string[] fileNames, bool keepFiles)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (fileNames == null)
		{
			throw new ArgumentNullException("fileNames");
		}
		options.TempFiles = new TempFileCollection();
		foreach (string fileName in fileNames)
		{
			options.TempFiles.AddFile(fileName, keepFiles);
		}
		options.TempFiles.KeepFiles = keepFiles;
		string outputName = string.Empty;
		string errorName = string.Empty;
		string cmd = CompilerName + " " + CmdArgsFromParameters(options);
		CompilerResults compilerResults = new CompilerResults(new TempFileCollection());
		compilerResults.NativeCompilerReturnValue = Executor.ExecWaitWithCapture(cmd, options.TempFiles, ref outputName, ref errorName);
		string[] array = outputName.Split(Environment.NewLine.ToCharArray());
		string[] array2 = array;
		foreach (string line in array2)
		{
			ProcessCompilerOutputLine(compilerResults, line);
		}
		if (compilerResults.Errors.Count == 0)
		{
			compilerResults.PathToAssembly = options.OutputAssembly;
		}
		return compilerResults;
	}

	[System.MonoTODO]
	protected virtual string GetResponseFileCmdArgs(CompilerParameters options, string cmdArgs)
	{
		throw new NotImplementedException();
	}

	protected static string JoinStringArray(string[] sa, string separator)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = sa.Length;
		if (num > 1)
		{
			for (int i = 0; i < num - 1; i++)
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(sa[i]);
				stringBuilder.Append("\"");
				stringBuilder.Append(separator);
			}
		}
		if (num > 0)
		{
			stringBuilder.Append("\"");
			stringBuilder.Append(sa[num - 1]);
			stringBuilder.Append("\"");
		}
		return stringBuilder.ToString();
	}

	protected abstract void ProcessCompilerOutputLine(CompilerResults results, string line);
}
