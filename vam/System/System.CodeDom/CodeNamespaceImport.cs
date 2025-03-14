using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeNamespaceImport : CodeObject
{
	private CodeLinePragma linePragma;

	private string nameSpace;

	public CodeLinePragma LinePragma
	{
		get
		{
			return linePragma;
		}
		set
		{
			linePragma = value;
		}
	}

	public string Namespace
	{
		get
		{
			if (nameSpace == null)
			{
				return string.Empty;
			}
			return nameSpace;
		}
		set
		{
			nameSpace = value;
		}
	}

	public CodeNamespaceImport()
	{
	}

	public CodeNamespaceImport(string nameSpace)
	{
		this.nameSpace = nameSpace;
	}
}
