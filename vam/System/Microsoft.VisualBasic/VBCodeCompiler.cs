using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualBasic;

internal class VBCodeCompiler : Microsoft.VisualBasic.VBCodeGenerator, ICodeCompiler
{
	private static string windowsMonoPath;

	private static string windowsvbncPath;

	static VBCodeCompiler()
	{
		if (Path.DirectorySeparatorChar == '\\')
		{
			PropertyInfo property = typeof(Environment).GetProperty("GacPath", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo getMethod = property.GetGetMethod(nonPublic: true);
			string directoryName = Path.GetDirectoryName((string)getMethod.Invoke(null, null));
			windowsMonoPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(directoryName)), "bin\\mono.bat");
			if (!File.Exists(windowsMonoPath))
			{
				windowsMonoPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(directoryName)), "bin\\mono.exe");
			}
			windowsvbncPath = Path.Combine(directoryName, "2.0\\vbnc.exe");
		}
	}

	public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit e)
	{
		return CompileAssemblyFromDomBatch(options, new CodeCompileUnit[1] { e });
	}

	public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		try
		{
			return CompileFromDomBatch(options, ea);
		}
		finally
		{
			options.TempFiles.Delete();
		}
	}

	public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
	{
		return CompileAssemblyFromFileBatch(options, new string[1] { fileName });
	}

	public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		try
		{
			return CompileFromFileBatch(options, fileNames);
		}
		finally
		{
			options.TempFiles.Delete();
		}
	}

	public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
	{
		return CompileAssemblyFromSourceBatch(options, new string[1] { source });
	}

	public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		try
		{
			return CompileFromSourceBatch(options, sources);
		}
		finally
		{
			options.TempFiles.Delete();
		}
	}

	private static string BuildArgs(CompilerParameters options, string[] fileNames)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("/quiet ");
		if (options.GenerateExecutable)
		{
			stringBuilder.Append("/target:exe ");
		}
		else
		{
			stringBuilder.Append("/target:library ");
		}
		if (options.TreatWarningsAsErrors)
		{
			stringBuilder.Append("/warnaserror ");
		}
		if (options.OutputAssembly == null || options.OutputAssembly.Length == 0)
		{
			string extension = ((!options.GenerateExecutable) ? "dll" : "exe");
			options.OutputAssembly = GetTempFileNameWithExtension(options.TempFiles, extension, !options.GenerateInMemory);
		}
		stringBuilder.AppendFormat("/out:\"{0}\" ", options.OutputAssembly);
		bool flag = false;
		if (options.ReferencedAssemblies != null)
		{
			StringEnumerator enumerator = options.ReferencedAssemblies.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					if (string.Compare(current, "Microsoft.VisualBasic", ignoreCase: true, CultureInfo.InvariantCulture) == 0)
					{
						flag = true;
					}
					stringBuilder.AppendFormat("/r:\"{0}\" ", current);
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
		if (!flag)
		{
			stringBuilder.Append("/r:\"Microsoft.VisualBasic.dll\" ");
		}
		if (options.CompilerOptions != null)
		{
			stringBuilder.Append(options.CompilerOptions);
			stringBuilder.Append(" ");
		}
		foreach (string arg in fileNames)
		{
			stringBuilder.AppendFormat(" \"{0}\" ", arg);
		}
		return stringBuilder.ToString();
	}

	private static CompilerError CreateErrorFromString(string error_string)
	{
		CompilerError compilerError = new CompilerError();
		Regex regex = new Regex("^(\\s*(?<file>.*)?\\((?<line>\\d*)(,(?<column>\\d*))?\\)\\s+)?:\\s*(?<level>Error|Warning)?\\s*(?<number>.*):\\s(?<message>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		Match match = regex.Match(error_string);
		if (!match.Success)
		{
			return null;
		}
		if (string.Empty != match.Result("${file}"))
		{
			compilerError.FileName = match.Result("${file}").Trim();
		}
		if (string.Empty != match.Result("${line}"))
		{
			compilerError.Line = int.Parse(match.Result("${line}"));
		}
		if (string.Empty != match.Result("${column}"))
		{
			compilerError.Column = int.Parse(match.Result("${column}"));
		}
		if (match.Result("${level}").Trim() == "Warning")
		{
			compilerError.IsWarning = true;
		}
		compilerError.ErrorNumber = match.Result("${number}");
		compilerError.ErrorText = match.Result("${message}");
		return compilerError;
	}

	private static string GetTempFileNameWithExtension(TempFileCollection temp_files, string extension, bool keepFile)
	{
		return temp_files.AddExtension(extension, keepFile);
	}

	private static string GetTempFileNameWithExtension(TempFileCollection temp_files, string extension)
	{
		return temp_files.AddExtension(extension);
	}

	private CompilerResults CompileFromFileBatch(CompilerParameters options, string[] fileNames)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (fileNames == null)
		{
			throw new ArgumentNullException("fileNames");
		}
		CompilerResults compilerResults = new CompilerResults(options.TempFiles);
		Process process = new Process();
		string text = string.Empty;
		if (Path.DirectorySeparatorChar == '\\')
		{
			process.StartInfo.FileName = windowsMonoPath;
			process.StartInfo.Arguments = windowsvbncPath + ' ' + BuildArgs(options, fileNames);
		}
		else
		{
			process.StartInfo.FileName = "vbnc";
			process.StartInfo.Arguments = BuildArgs(options, fileNames);
		}
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		try
		{
			process.Start();
		}
		catch (Exception ex)
		{
			if (ex is Win32Exception ex2)
			{
				throw new SystemException($"Error running {process.StartInfo.FileName}: {Win32Exception.W32ErrorMessage(ex2.NativeErrorCode)}");
			}
			throw;
		}
		try
		{
			text = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
		}
		finally
		{
			compilerResults.NativeCompilerReturnValue = process.ExitCode;
			process.Close();
		}
		bool flag = true;
		if (compilerResults.NativeCompilerReturnValue == 1)
		{
			flag = false;
			string[] array = text.Split(Environment.NewLine.ToCharArray());
			string[] array2 = array;
			foreach (string error_string in array2)
			{
				CompilerError compilerError = CreateErrorFromString(error_string);
				if (compilerError != null)
				{
					compilerResults.Errors.Add(compilerError);
				}
			}
		}
		if ((!flag && !compilerResults.Errors.HasErrors) || (compilerResults.NativeCompilerReturnValue != 0 && compilerResults.NativeCompilerReturnValue != 1))
		{
			flag = false;
			CompilerError value = new CompilerError(string.Empty, 0, 0, "VBNC_CRASH", text);
			compilerResults.Errors.Add(value);
		}
		if (flag)
		{
			if (options.GenerateInMemory)
			{
				using FileStream fileStream = File.OpenRead(options.OutputAssembly);
				byte[] array3 = new byte[fileStream.Length];
				fileStream.Read(array3, 0, array3.Length);
				compilerResults.CompiledAssembly = Assembly.Load(array3, null, options.Evidence);
				fileStream.Close();
			}
			else
			{
				compilerResults.CompiledAssembly = Assembly.LoadFrom(options.OutputAssembly);
				compilerResults.PathToAssembly = options.OutputAssembly;
			}
		}
		else
		{
			compilerResults.CompiledAssembly = null;
		}
		return compilerResults;
	}

	private CompilerResults CompileFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (ea == null)
		{
			throw new ArgumentNullException("ea");
		}
		string[] array = new string[ea.Length];
		StringCollection referencedAssemblies = options.ReferencedAssemblies;
		for (int i = 0; i < ea.Length; i++)
		{
			CodeCompileUnit codeCompileUnit = ea[i];
			array[i] = GetTempFileNameWithExtension(options.TempFiles, i + ".vb");
			FileStream fileStream = new FileStream(array[i], FileMode.OpenOrCreate);
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
		}
		return CompileAssemblyFromFileBatch(options, array);
	}

	private CompilerResults CompileFromSourceBatch(CompilerParameters options, string[] sources)
	{
		if (options == null)
		{
			throw new ArgumentNullException("options");
		}
		if (sources == null)
		{
			throw new ArgumentNullException("sources");
		}
		string[] array = new string[sources.Length];
		for (int i = 0; i < sources.Length; i++)
		{
			array[i] = GetTempFileNameWithExtension(options.TempFiles, i + ".vb");
			FileStream fileStream = new FileStream(array[i], FileMode.OpenOrCreate);
			using (StreamWriter streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(sources[i]);
				streamWriter.Close();
			}
			fileStream.Close();
		}
		return CompileFromFileBatch(options, array);
	}
}
