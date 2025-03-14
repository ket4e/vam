using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeLabeledStatement : CodeStatement
{
	private string label;

	private CodeStatement statement;

	public string Label
	{
		get
		{
			if (label == null)
			{
				return string.Empty;
			}
			return label;
		}
		set
		{
			label = value;
		}
	}

	public CodeStatement Statement
	{
		get
		{
			return statement;
		}
		set
		{
			statement = value;
		}
	}

	public CodeLabeledStatement()
	{
	}

	public CodeLabeledStatement(string label)
	{
		this.label = label;
	}

	public CodeLabeledStatement(string label, CodeStatement statement)
	{
		this.label = label;
		this.statement = statement;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
