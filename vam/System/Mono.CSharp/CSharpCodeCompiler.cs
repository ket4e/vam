using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Mono.CSharp;

internal class CSharpCodeCompiler : Mono.CSharp.CSharpCodeGenerator, ICodeCompiler
{
	private static string windowsMcsPath;

	private static string windowsMonoPath;

	private Mutex mcsOutMutex;

	private StringCollection mcsOutput;

	public CSharpCodeCompiler()
	{
	}

	public CSharpCodeCompiler(IDictionary<string, string> providerOptions)
		: base(providerOptions)
	{
	}

	static CSharpCodeCompiler()
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
			if (!File.Exists(windowsMonoPath))
			{
				windowsMonoPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(directoryName))), "mono\\mono\\mini\\mono.exe");
			}
			if (!File.Exists(windowsMonoPath))
			{
				throw new FileNotFoundException("Windows mono path not found: " + windowsMonoPath);
			}
			windowsMcsPath = Path.Combine(directoryName, "2.0\\gmcs.exe");
			if (!File.Exists(windowsMcsPath))
			{
				windowsMcsPath = Path.Combine(Path.GetDirectoryName(directoryName), "lib\\net_2_0\\gmcs.exe");
			}
			if (!File.Exists(windowsMcsPath))
			{
				throw new FileNotFoundException("Windows mcs path not found: " + windowsMcsPath);
			}
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
		if (Path.DirectorySeparatorChar == '\\')
		{
			process.StartInfo.FileName = windowsMonoPath;
			process.StartInfo.Arguments = "\"" + windowsMcsPath + "\" " + BuildArgs(options, fileNames, base.ProviderOptions);
		}
		else
		{
			process.StartInfo.FileName = "gmcs";
			process.StartInfo.Arguments = BuildArgs(options, fileNames, base.ProviderOptions);
		}
		mcsOutput = new StringCollection();
		mcsOutMutex = new Mutex();
		string text = Environment.GetEnvironmentVariable("MONO_PATH");
		if (text == null)
		{
			text = string.Empty;
		}
		string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
		if (privateBinPath != null && privateBinPath.Length > 0)
		{
			text = $"{privateBinPath}:{text}";
		}
		if (text.Length > 0)
		{
			StringDictionary environmentVariables = process.StartInfo.EnvironmentVariables;
			if (environmentVariables.ContainsKey("MONO_PATH"))
			{
				environmentVariables["MONO_PATH"] = text;
			}
			else
			{
				environmentVariables.Add("MONO_PATH", text);
			}
		}
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.ErrorDataReceived += McsStderrDataReceived;
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
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();
			compilerResults.NativeCompilerReturnValue = process.ExitCode;
		}
		finally
		{
			process.CancelErrorRead();
			process.CancelOutputRead();
			process.Close();
		}
		StringCollection stringCollection = mcsOutput;
		bool flag = true;
		StringEnumerator enumerator = mcsOutput.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				CompilerError compilerError = CreateErrorFromString(current);
				if (compilerError != null)
				{
					compilerResults.Errors.Add(compilerError);
					if (!compilerError.IsWarning)
					{
						flag = false;
					}
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
		if (stringCollection.Count > 0)
		{
			stringCollection.Insert(0, process.StartInfo.FileName + " " + process.StartInfo.Arguments + Environment.NewLine);
			compilerResults.Output = stringCollection;
		}
		if (flag)
		{
			if (!File.Exists(options.OutputAssembly))
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringEnumerator enumerator2 = stringCollection.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						string current2 = enumerator2.Current;
						stringBuilder.Append(current2 + Environment.NewLine);
					}
				}
				finally
				{
					if (enumerator2 is IDisposable disposable2)
					{
						disposable2.Dispose();
					}
				}
				throw new Exception("Compiler failed to produce the assembly. Output: '" + stringBuilder.ToString() + "'");
			}
			if (options.GenerateInMemory)
			{
				using FileStream fileStream = File.OpenRead(options.OutputAssembly);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				compilerResults.CompiledAssembly = Assembly.Load(array, null, options.Evidence);
				fileStream.Close();
			}
			else
			{
				compilerResults.PathToAssembly = options.OutputAssembly;
			}
		}
		else
		{
			compilerResults.CompiledAssembly = null;
		}
		return compilerResults;
	}

	private void McsStderrDataReceived(object sender, DataReceivedEventArgs args)
	{
		if (args.Data != null)
		{
			mcsOutMutex.WaitOne();
			mcsOutput.Add(args.Data);
			mcsOutMutex.ReleaseMutex();
		}
	}

	private static string BuildArgs(CompilerParameters options, string[] fileNames, IDictionary<string, string> providerOptions)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (options.GenerateExecutable)
		{
			stringBuilder.Append("/target:exe ");
		}
		else
		{
			stringBuilder.Append("/target:library ");
		}
		string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
		if (privateBinPath != null && privateBinPath.Length > 0)
		{
			stringBuilder.AppendFormat("/lib:\"{0}\" ", privateBinPath);
		}
		if (options.Win32Resource != null)
		{
			stringBuilder.AppendFormat("/win32res:\"{0}\" ", options.Win32Resource);
		}
		if (options.IncludeDebugInformation)
		{
			stringBuilder.Append("/debug+ /optimize- ");
		}
		else
		{
			stringBuilder.Append("/debug- /optimize+ ");
		}
		if (options.TreatWarningsAsErrors)
		{
			stringBuilder.Append("/warnaserror ");
		}
		if (options.WarningLevel >= 0)
		{
			stringBuilder.AppendFormat("/warn:{0} ", options.WarningLevel);
		}
		if (options.OutputAssembly == null || options.OutputAssembly.Length == 0)
		{
			string extension = ((!options.GenerateExecutable) ? "dll" : "exe");
			options.OutputAssembly = GetTempFileNameWithExtension(options.TempFiles, extension, !options.GenerateInMemory);
		}
		stringBuilder.AppendFormat("/out:\"{0}\" ", options.OutputAssembly);
		StringEnumerator enumerator = options.ReferencedAssemblies.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (current != null && current.Length != 0)
				{
					stringBuilder.AppendFormat("/r:\"{0}\" ", current);
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
		if (options.CompilerOptions != null)
		{
			stringBuilder.Append(options.CompilerOptions);
			stringBuilder.Append(" ");
		}
		StringEnumerator enumerator2 = options.EmbeddedResources.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				string current2 = enumerator2.Current;
				stringBuilder.AppendFormat("/resource:\"{0}\" ", current2);
			}
		}
		finally
		{
			if (enumerator2 is IDisposable disposable2)
			{
				disposable2.Dispose();
			}
		}
		StringEnumerator enumerator3 = options.LinkedResources.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				string current3 = enumerator3.Current;
				stringBuilder.AppendFormat("/linkresource:\"{0}\" ", current3);
			}
		}
		finally
		{
			if (enumerator3 is IDisposable disposable3)
			{
				disposable3.Dispose();
			}
		}
		if (providerOptions != null && providerOptions.Count > 0)
		{
			if (!providerOptions.TryGetValue("CompilerVersion", out var value))
			{
				value = "2.0";
			}
			if (value.Length >= 1 && value[0] == 'v')
			{
				value = value.Substring(1);
			}
			switch (value)
			{
			case "2.0":
				stringBuilder.Append("/langversion:ISO-2");
				break;
			}
		}
		stringBuilder.Append(" -- ");
		foreach (string arg in fileNames)
		{
			stringBuilder.AppendFormat("\"{0}\" ", arg);
		}
		return stringBuilder.ToString();
	}

	private static CompilerError CreateErrorFromString(string error_string)
	{
		if (error_string.StartsWith("BETA"))
		{
			return null;
		}
		if (error_string == null || error_string == string.Empty)
		{
			return null;
		}
		CompilerError compilerError = new CompilerError();
		Regex regex = new Regex("^(\\s*(?<file>.*)\\((?<line>\\d*)(,(?<column>\\d*))?\\)(:)?\\s+)*(?<level>\\w+)\\s*(?<number>.*):\\s(?<message>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		Match match = regex.Match(error_string);
		if (!match.Success)
		{
			compilerError.ErrorText = error_string;
			compilerError.IsWarning = false;
			compilerError.ErrorNumber = string.Empty;
			return compilerError;
		}
		if (string.Empty != match.Result("${file}"))
		{
			compilerError.FileName = match.Result("${file}");
		}
		if (string.Empty != match.Result("${line}"))
		{
			compilerError.Line = int.Parse(match.Result("${line}"));
		}
		if (string.Empty != match.Result("${column}"))
		{
			compilerError.Column = int.Parse(match.Result("${column}"));
		}
		string text = match.Result("${level}");
		if (text == "warning")
		{
			compilerError.IsWarning = true;
		}
		else if (text != "error")
		{
			return null;
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
			array[i] = GetTempFileNameWithExtension(options.TempFiles, i + ".cs");
			FileStream fileStream = new FileStream(array[i], FileMode.OpenOrCreate);
			StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
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
			array[i] = GetTempFileNameWithExtension(options.TempFiles, i + ".cs");
			FileStream fileStream = new FileStream(array[i], FileMode.OpenOrCreate);
			using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
			{
				streamWriter.Write(sources[i]);
				streamWriter.Close();
			}
			fileStream.Close();
		}
		return CompileFromFileBatch(options, array);
	}
}
