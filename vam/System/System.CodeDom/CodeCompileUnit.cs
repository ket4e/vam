using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeCompileUnit : CodeObject
{
	private CodeAttributeDeclarationCollection attributes;

	private CodeNamespaceCollection namespaces;

	private StringCollection assemblies;

	private CodeDirectiveCollection startDirectives;

	private CodeDirectiveCollection endDirectives;

	public CodeAttributeDeclarationCollection AssemblyCustomAttributes
	{
		get
		{
			if (attributes == null)
			{
				attributes = new CodeAttributeDeclarationCollection();
			}
			return attributes;
		}
	}

	public CodeNamespaceCollection Namespaces
	{
		get
		{
			if (namespaces == null)
			{
				namespaces = new CodeNamespaceCollection();
			}
			return namespaces;
		}
	}

	public StringCollection ReferencedAssemblies
	{
		get
		{
			if (assemblies == null)
			{
				assemblies = new StringCollection();
			}
			return assemblies;
		}
	}

	public CodeDirectiveCollection StartDirectives
	{
		get
		{
			if (startDirectives == null)
			{
				startDirectives = new CodeDirectiveCollection();
			}
			return startDirectives;
		}
	}

	public CodeDirectiveCollection EndDirectives
	{
		get
		{
			if (endDirectives == null)
			{
				endDirectives = new CodeDirectiveCollection();
			}
			return endDirectives;
		}
	}
}
