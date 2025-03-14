using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeSnippetExpression : CodeExpression
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

	public CodeSnippetExpression()
	{
	}

	public CodeSnippetExpression(string value)
	{
		this.value = value;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
