using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;

namespace System.CodeDom.Compiler;

[Serializable]
[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class CompilerParameters
{
	private string compilerOptions;

	private Evidence evidence;

	private bool generateExecutable;

	private bool generateInMemory;

	private bool includeDebugInformation;

	private string mainClass;

	private string outputAssembly;

	private StringCollection referencedAssemblies;

	private TempFileCollection tempFiles;

	private bool treatWarningsAsErrors;

	private IntPtr userToken = IntPtr.Zero;

	private int warningLevel = -1;

	private string win32Resource;

	private StringCollection embedded_resources;

	private StringCollection linked_resources;

	public string CompilerOptions
	{
		get
		{
			return compilerOptions;
		}
		set
		{
			compilerOptions = value;
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

	public bool GenerateExecutable
	{
		get
		{
			return generateExecutable;
		}
		set
		{
			generateExecutable = value;
		}
	}

	public bool GenerateInMemory
	{
		get
		{
			return generateInMemory;
		}
		set
		{
			generateInMemory = value;
		}
	}

	public bool IncludeDebugInformation
	{
		get
		{
			return includeDebugInformation;
		}
		set
		{
			includeDebugInformation = value;
		}
	}

	public string MainClass
	{
		get
		{
			return mainClass;
		}
		set
		{
			mainClass = value;
		}
	}

	public string OutputAssembly
	{
		get
		{
			return outputAssembly;
		}
		set
		{
			outputAssembly = value;
		}
	}

	public StringCollection ReferencedAssemblies
	{
		get
		{
			if (referencedAssemblies == null)
			{
				referencedAssemblies = new StringCollection();
			}
			return referencedAssemblies;
		}
	}

	public TempFileCollection TempFiles
	{
		get
		{
			if (tempFiles == null)
			{
				tempFiles = new TempFileCollection();
			}
			return tempFiles;
		}
		set
		{
			tempFiles = value;
		}
	}

	public bool TreatWarningsAsErrors
	{
		get
		{
			return treatWarningsAsErrors;
		}
		set
		{
			treatWarningsAsErrors = value;
		}
	}

	public IntPtr UserToken
	{
		get
		{
			return userToken;
		}
		set
		{
			userToken = value;
		}
	}

	public int WarningLevel
	{
		get
		{
			return warningLevel;
		}
		set
		{
			warningLevel = value;
		}
	}

	public string Win32Resource
	{
		get
		{
			return win32Resource;
		}
		set
		{
			win32Resource = value;
		}
	}

	[ComVisible(false)]
	public StringCollection EmbeddedResources
	{
		get
		{
			if (embedded_resources == null)
			{
				embedded_resources = new StringCollection();
			}
			return embedded_resources;
		}
	}

	[ComVisible(false)]
	public StringCollection LinkedResources
	{
		get
		{
			if (linked_resources == null)
			{
				linked_resources = new StringCollection();
			}
			return linked_resources;
		}
	}

	public CompilerParameters()
	{
	}

	public CompilerParameters(string[] assemblyNames)
	{
		referencedAssemblies = new StringCollection();
		referencedAssemblies.AddRange(assemblyNames);
	}

	public CompilerParameters(string[] assemblyNames, string output)
	{
		referencedAssemblies = new StringCollection();
		referencedAssemblies.AddRange(assemblyNames);
		outputAssembly = output;
	}

	public CompilerParameters(string[] assemblyNames, string output, bool includeDebugInfo)
	{
		referencedAssemblies = new StringCollection();
		referencedAssemblies.AddRange(assemblyNames);
		outputAssembly = output;
		includeDebugInformation = includeDebugInfo;
	}
}
