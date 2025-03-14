using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeAssignStatement : CodeStatement
{
	private CodeExpression left;

	private CodeExpression right;

	public CodeExpression Left
	{
		get
		{
			return left;
		}
		set
		{
			left = value;
		}
	}

	public CodeExpression Right
	{
		get
		{
			return right;
		}
		set
		{
			right = value;
		}
	}

	public CodeAssignStatement()
	{
	}

	public CodeAssignStatement(CodeExpression left, CodeExpression right)
	{
		this.left = left;
		this.right = right;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
