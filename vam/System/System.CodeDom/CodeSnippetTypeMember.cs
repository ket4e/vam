using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeSnippetTypeMember : CodeTypeMember
{
	private string text;

	public string Text
	{
		get
		{
			if (text == null)
			{
				return string.Empty;
			}
			return text;
		}
		set
		{
			text = value;
		}
	}

	public CodeSnippetTypeMember()
	{
	}

	public CodeSnippetTypeMember(string text)
	{
		this.text = text;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
