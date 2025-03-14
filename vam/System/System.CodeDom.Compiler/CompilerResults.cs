using System.Collections.Specialized;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;

namespace System.CodeDom.Compiler;

[Serializable]
[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class CompilerResults
{
	private Assembly compiledAssembly;

	private CompilerErrorCollection errors = new CompilerErrorCollection();

	private Evidence evidence;

	private int nativeCompilerReturnValue;

	private StringCollection output = new StringCollection();

	private string pathToAssembly;

	private TempFileCollection tempFiles;

	public Assembly CompiledAssembly
	{
		get
		{
			if (compiledAssembly == null && pathToAssembly != null)
			{
				compiledAssembly = Assembly.LoadFrom(pathToAssembly);
			}
			return compiledAssembly;
		}
		set
		{
			compiledAssembly = value;
		}
	}

	public CompilerErrorCollection Errors
	{
		get
		{
			if (errors == null)
			{
				errors = new CompilerErrorCollection();
			}
			return errors;
		}
	}

	public Evidence Evidence
	{
		get
		{
			return evidence;
		}
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"ControlEvidence\"/>\n</PermissionSet>\n")]
		set
		{
			evidence = value;
		}
	}

	public int NativeCompilerReturnValue
	{
		get
		{
			return nativeCompilerReturnValue;
		}
		set
		{
			nativeCompilerReturnValue = value;
		}
	}

	public StringCollection Output
	{
		get
		{
			if (output == null)
			{
				output = new StringCollection();
			}
			return output;
		}
		internal set
		{
			output = value;
		}
	}

	public string PathToAssembly
	{
		get
		{
			return pathToAssembly;
		}
		set
		{
			pathToAssembly = value;
		}
	}

	public TempFileCollection TempFiles
	{
		get
		{
			return tempFiles;
		}
		set
		{
			tempFiles = value;
		}
	}

	public CompilerResults(TempFileCollection tempFiles)
	{
		this.tempFiles = tempFiles;
	}
}
