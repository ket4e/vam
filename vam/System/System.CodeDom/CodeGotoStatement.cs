using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeGotoStatement : CodeStatement
{
	private string label;

	public string Label
	{
		get
		{
			return label;
		}
		set
		{
			if (value == null || value.Length == 0)
			{
				throw new ArgumentNullException("value");
			}
			label = value;
		}
	}

	public CodeGotoStatement()
	{
	}

	public CodeGotoStatement(string label)
	{
		Label = label;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
