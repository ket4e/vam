using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeSnippetCompileUnit : CodeCompileUnit
{
	private CodeLinePragma linePragma;

	private string value;

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

	public string Value
	{
		get
		{
			if (value == null)
			{
				return string.Empty;
			}
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public CodeSnippetCompileUnit()
	{
	}

	public CodeSnippetCompileUnit(string value)
	{
		this.value = value;
	}
}
