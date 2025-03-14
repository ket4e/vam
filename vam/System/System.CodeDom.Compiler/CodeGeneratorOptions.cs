using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.CodeDom.Compiler;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class CodeGeneratorOptions
{
	private IDictionary properties;

	public bool BlankLinesBetweenMembers
	{
		get
		{
			object obj = properties["BlankLinesBetweenMembers"];
			return obj == null || (bool)obj;
		}
		set
		{
			properties["BlankLinesBetweenMembers"] = value;
		}
	}

	public string BracingStyle
	{
		get
		{
			object obj = properties["BracingStyle"];
			return (obj != null) ? ((string)obj) : "Block";
		}
		set
		{
			properties["BracingStyle"] = value;
		}
	}

	public bool ElseOnClosing
	{
		get
		{
			object obj = properties["ElseOnClosing"];
			return obj != null && (bool)obj;
		}
		set
		{
			properties["ElseOnClosing"] = value;
		}
	}

	public string IndentString
	{
		get
		{
			object obj = properties["IndentString"];
			return (obj != null) ? ((string)obj) : "    ";
		}
		set
		{
			properties["IndentString"] = value;
		}
	}

	public object this[string index]
	{
		get
		{
			return properties[index];
		}
		set
		{
			properties[index] = value;
		}
	}

	[ComVisible(false)]
	public bool VerbatimOrder
	{
		get
		{
			object obj = properties["VerbatimOrder"];
			return obj != null && (bool)obj;
		}
		set
		{
			properties["VerbatimOrder"] = value;
		}
	}

	public CodeGeneratorOptions()
	{
		properties = new ListDictionary();
	}
}
