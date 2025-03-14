using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;

namespace System.CodeDom.Compiler;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public sealed class Executor
{
	private class ProcessResultReader
	{
		private StreamReader reader;

		private string file;

		public ProcessResultReader(StreamReader reader, string file)
		{
			this.reader = reader;
			this.file = file;
		}

		public void Read()
		{
			StreamWriter streamWriter = new StreamWriter(file);
			try
			{
				string value;
				while ((value = reader.ReadLine()) != null)
				{
					streamWriter.WriteLine(value);
				}
			}
			finally
			{
				streamWriter.Close();
			}
		}
	}

	private Executor()
	{
	}

	public static void ExecWait(string cmd, TempFileCollection tempFiles)
	{
		string outputName = null;
		string errorName = null;
		ExecWaitWithCapture(cmd, Environment.CurrentDirectory, tempFiles, ref outputName, ref errorName);
	}

	[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"ControlPrincipal\"/>\n</PermissionSet>\n")]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static int ExecWaitWithCapture(IntPtr userToken, string cmd, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName)
	{
		using (WindowsIdentity.Impersonate(userToken))
		{
			return InternalExecWaitWithCapture(cmd, currentDir, tempFiles, ref outputName, ref errorName);
		}
	}

	public static int ExecWaitWithCapture(IntPtr userToken, string cmd, TempFileCollection tempFiles, ref string outputName, ref string errorName)
	{
		return ExecWaitWithCapture(userToken, cmd, Environment.CurrentDirectory, tempFiles, ref outputName, ref errorName);
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static int ExecWaitWithCapture(string cmd, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName)
	{
		return InternalExecWaitWithCapture(cmd, currentDir, tempFiles, ref outputName, ref errorName);
	}

	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
	public static int ExecWaitWithCapture(string cmd, TempFileCollection tempFiles, ref string outputName, ref string errorName)
	{
		return InternalExecWaitWithCapture(cmd, Environment.CurrentDirectory, tempFiles, ref outputName, ref errorName);
	}

	private static int InternalExecWaitWithCapture(string cmd, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName)
	{
		if (cmd == null || cmd.Length == 0)
		{
			throw new ExternalException(global::Locale.GetText("No command provided for execution."));
		}
		if (outputName == null)
		{
			outputName = tempFiles.AddExtension("out");
		}
		if (errorName == null)
		{
			errorName = tempFiles.AddExtension("err");
		}
		int num = -1;
		Process process = new Process();
		process.StartInfo.FileName = cmd;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.WorkingDirectory = currentDir;
		try
		{
			process.Start();
			ProcessResultReader processResultReader = new ProcessResultReader(process.StandardOutput, outputName);
			ProcessResultReader @object = new ProcessResultReader(process.StandardError, errorName);
			Thread thread = new Thread(@object.Read);
			thread.Start();
			processResultReader.Read();
			thread.Join();
			process.WaitForExit();
		}
		finally
		{
			num = process.ExitCode;
			process.Close();
		}
		return num;
	}
}
