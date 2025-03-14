using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeSnippetStatement : CodeStatement
{
	private string value;

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

	public CodeSnippetStatement()
	{
	}

	public CodeSnippetStatement(string value)
	{
		this.value = value;
	}
}
